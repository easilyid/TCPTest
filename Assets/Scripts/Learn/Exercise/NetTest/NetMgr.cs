using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

namespace Learn.Exercise.NetTest
{
    public class NetMgr : MonoBehaviour
    {
        private static NetMgr _instance;
        public static NetMgr Instance => _instance;
        private Socket _socket;

        // 存放发送的消息队列 存入主线程里的消息 拿出来在子线程里发送
        //private Queue<string> _sendMsgQue = new Queue<string>();
        private Queue<BaseMsg> _sendMsgQue = new Queue<BaseMsg>();

        // 存放接收的消息队列 存入子线程里的消息 拿出来在主线程里处理
        //public Queue<string> _receiveMsgQue = new Queue<string>();
        public Queue<BaseMsg> _receiveMsgQue = new Queue<BaseMsg>();

       // private byte[] receiveBytes = new byte[1024 * 1024];
        //private int receiveNum = 0;

        private byte[] cacheBytes = new byte[1024 * 1024];
        private int cacheNum = 0;

        //是否连接
        private bool isConnected = false;

    //发送心跳消息的间隔时间
    private int SEND_HEART_MSG_TIME = 2;
    private HeartMsg hearMsg = new HeartMsg();

    void Awake()
    {
        _instance = this;
        DontDestroyOnLoad(this.gameObject);
        //客户端循环定时给服务端发送心跳消息
        InvokeRepeating("SendHeartMsg", 0, SEND_HEART_MSG_TIME);
    }

    private void SendHeartMsg()
    {
        if (isConnected)
            Send(hearMsg);
    }

    // Update is called once per frame
    void Update()
    {
        if(_receiveMsgQue.Count > 0)
        {
            BaseMsg msg = _receiveMsgQue.Dequeue();
            if(msg is PlayerMsg)
            {
                PlayerMsg playerMsg = (msg as PlayerMsg);
                print(playerMsg.ID);
                print(playerMsg.data.name);
                print(playerMsg.data.lev);
                print(playerMsg.data.atk);
            }
        }
    }

        //链接服务器的Socket
        public void Connect(string ip, int port)
        {
            if (isConnected)
            {
                return;
            }

            if (_socket == null)
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            try
            {
                _socket.Connect(ipEndPoint);
                isConnected = true;
                //开启子线程   使用线程池可以避免频繁创建线程 
                ThreadPool.QueueUserWorkItem(SendMsg);
                ThreadPool.QueueUserWorkItem(ReceiveMsg);
            }
            catch (SocketException e)
            {
                if (e.ErrorCode == 10061)
                    print("连接被服务器拒绝" + e.ErrorCode);
                else
                {
                    print("连接失败" + e.ErrorCode + e.Message);
                }
            }
        }


        //发送消息
        // public void Send(string info)
        // {
        //     _sendMsgQue.Enqueue(info);
        // } 

        public void Send(BaseMsg info)
        {
            _sendMsgQue.Enqueue(info);
        }

        /// <summary>
        /// 用于测试 直接发字节数组的方法
        /// 测试分包 粘包
        /// </summary>
        /// <param name="bytes"></param>
        public void SendTest(byte[] bytes)
        {
            _socket.Send(bytes);
        }

        private void SendMsg(object state)
        {
            while (isConnected)
            {
                if (_sendMsgQue.Count > 0)
                {
                    //_socket.Send(Encoding.UTF8.GetBytes(_sendMsgQue.Dequeue()));
                    _socket.Send(_sendMsgQue.Dequeue().Writing());
                }
            }
        }

        //接收消息
        private void ReceiveMsg(object state)
        {
            while (isConnected)
            {
                if (_socket.Available > 0)
                {
                    // receiveNum = _socket.Receive(receiveBytes);
                    // _receiveMsgQue.Enqueue(Encoding.UTF8.GetString(receiveBytes, 0, receiveNum));

                    //区分消息Test
                    /*receiveNum = _socket.Receive(receiveBytes);
                    int msgID = BitConverter.ToInt32(receiveBytes, 0);
                    BaseMsg baseMsg = null;
                    switch (msgID)
                    {
                        case 10086:
                            PlayerMsg playerMsg = new PlayerMsg();
                            playerMsg.Reading(receiveBytes, 4);
                            baseMsg = playerMsg;
                            break;
                    }

                    if (baseMsg==null)
                    {
                        continue;
                    }
                    _receiveMsgQue.Enqueue(baseMsg);*/

                    byte[] receiveBytes = new byte[1024 * 1024];
                    int receiveNum = _socket.Receive(receiveBytes);
                    //分包 粘包
                    receiveNum = _socket.Receive(receiveBytes);
                    HandleReceiveMsg(receiveBytes, receiveNum);
                }
            }
        }

        /// <summary>
        /// 处理分包 粘包 问题的方法
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="index"></param>
        private void HandleReceiveMsg(byte[] receiveBytes, int receiveNum)
        {
            //同時處理
            int msgID = 0; //消息ID
            int msglength = 0;
            int nowIndex = 0;

            receiveBytes.CopyTo(cacheBytes, cacheNum);
            cacheNum += receiveNum;
            while (true)
            {
                //每次将长度设置为-1 避免上次的解析的数据影响这次的判断
                msglength = -1;
                if (cacheNum - nowIndex >= 8)
                {
                    msgID = BitConverter.ToInt32(cacheBytes, nowIndex);
                    nowIndex += 4;
                    msglength = BitConverter.ToInt32(cacheBytes, nowIndex);
                    nowIndex += 4;
                }

                if (cacheNum - nowIndex >= msglength && msglength != -1)
                {
                    BaseMsg baseMsg = null;
                    switch (msgID)
                    {
                        case 1001:
                            PlayerMsg playerMsg = new PlayerMsg();
                            playerMsg.Reading(cacheBytes, nowIndex);
                            baseMsg = playerMsg;
                            break;
                    }

                    if (baseMsg != null)
                    {
                        _receiveMsgQue.Enqueue(baseMsg);
                    }

                    nowIndex += msglength;
                    if (nowIndex == cacheNum)
                    {
                        cacheNum = 0;
                        break;
                    }
                }
                else
                {
                    //如果不满足 证明有分包 
                    //那么我们需要把当前收到的内容 记录下来
                    //有待下次接受到消息后 再做处理
                    //receiveBytes.CopyTo(cacheBytes, 0);
                    //cacheNum = receiveNum;
                    //如果进行了 id和长度的解析 但是 没有成功解析消息体 那么我们需要减去nowIndex移动的位置
                    if (msglength != -1)
                        nowIndex -= 8;
                    //就是把剩余没有解析的字节数组内容 移到前面来 用于缓存下次继续解析
                    Array.Copy(cacheBytes, nowIndex, cacheBytes, 0, cacheNum - nowIndex);
                    cacheNum = cacheNum - nowIndex;
                    break;
                }
            }
        }

    public void Close()
    {
        if(_socket != null)
        {
            print("客户端主动断开连接");

            //主动发送一条断开连接的消息给服务端
            //QuitMsg msg = new QuitMsg();
            //socket.Send(msg.Writing());
            //socket.Shutdown(SocketShutdown.Both);
            //socket.Disconnect(false);
            //socket.Close();
            _socket = null;

            isConnected = false;
        }
    }

        private void OnDestroy()
        {
            Close();
        }
    }
}