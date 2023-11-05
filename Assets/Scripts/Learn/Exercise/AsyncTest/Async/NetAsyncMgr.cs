using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Learn.Exercise.AsyncTest.AsyncData;
using UnityEngine;

namespace Learn.Exercise.AsyncTest.Async
{
    public class NetAsyncMgr : MonoBehaviour
    {
        private static NetAsyncMgr _instance;
        public static NetAsyncMgr Instance => _instance;

        //和服务器进行连接的socket
        private Socket _socket;

        private byte[] cachaBytes = new byte[1024 * 1024];
        private int cacheNum = 0;

        public Queue<AsyncData.BaseMsg> receiveQue = new Queue<AsyncData.BaseMsg>();

        //发送心跳消息的间隔时间
        private int SEND_HEART_MSG_TIME = 5;
        private  AsyncData.HeartMsg _heartMsg = new AsyncData.HeartMsg();

        private void Awake()
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);

            InvokeRepeating("SendHeartMsg", 0, SEND_HEART_MSG_TIME);
        }

        private void Update()
        {
            if (receiveQue.Count > 0)
            {
                AsyncData.BaseMsg baseMsg = receiveQue.Dequeue();
                switch (baseMsg)
                {
                    case AsyncData.PlayerMsg msg:
                        print(msg.ID);
                        print(msg.Data.name);
                        print(msg.Data.lev);
                        print(msg.Data.atk);
                        break;
                }
            }
        }

        public void SendHeartMsg()
        {
            if (_socket != null && _socket.Connected)
            {
                Send(_heartMsg);
            }
        }

        public void Connect(string ip, int port)
        {
            if (_socket != null && _socket.Connected)
            {
                return;
            }

            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.RemoteEndPoint = ipEndPoint;
            args.Completed += (_socket, args) =>
            {
                if (args.SocketError == SocketError.Success)
                {
                    print("连接成功");
                    //收消息
                    SocketAsyncEventArgs args2 = new SocketAsyncEventArgs();
                    args2.SetBuffer(cachaBytes, 0, cachaBytes.Length);
                    args2.Completed += ReceiveCallBack;
                    this._socket.ReceiveAsync(args2);
                }
                else
                {
                    print("连接失败" + args.SocketError);
                }
            };
            _socket.ConnectAsync(args);
        }

        public void ReceiveCallBack(object obj, SocketAsyncEventArgs args)
        {
            if (args.SocketError == SocketError.Success)
            {
                //print(Encoding.UTF8.GetString(args.Buffer, 0, args.BytesTransferred));
                HandleReceiveMsg(args.BytesTransferred);
                //继续收消息
                args.SetBuffer(cacheNum, args.Buffer.Length - cacheNum);
                if (this._socket != null && this._socket.Connected)
                    //继续异步接收消息
                    _socket.ReceiveAsync(args);
                else
                    Close();
            }
            else
            {
                print("接收失败" + args.SocketError);
                //关闭连接
                Close();
            }
        }

        private void Close()
        {
            if (_socket != null)
            {
                QuitMsg msg = new QuitMsg();
                _socket.Send(msg.WriteToBytes());
                _socket.Shutdown(SocketShutdown.Both);
                _socket.Disconnect(false);
                _socket.Close();
                _socket = null;
            }
        }

        public void SendTest(byte[] bytes)
        {
            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.SetBuffer(bytes, 0, bytes.Length);
            args.Completed += (socket, args) =>
            {
                if (args.SocketError != SocketError.Success)
                {
                    print("发送消息失败" + args.SocketError);
                    Close();
                }
            };
            this._socket.SendAsync(args);
        }

        public void Send(AsyncData.BaseMsg info)
        {
            if (this._socket != null && this._socket.Connected)
            {
                //byte[] bytes = Encoding.UTF8.GetBytes(info);
                byte[] bytes = info.WriteToBytes();
                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                args.SetBuffer(bytes, 0, bytes.Length);
                args.Completed += (_socket, args) =>
                {
                    if (args.SocketError != SocketError.Success)
                    {
                        print("发送消息失败: " + args.SocketError);
                        Close();
                    }
                };
                this._socket.SendAsync(args);
            }
            else
            {
                Close();
            }
        }


        public void HandleReceiveMsg(int receiveNum)
        {
            int msgID = 0;
            int msgLength = 0;
            int nowIndex = 0;
            cacheNum += receiveNum;
            while (true)
            {
                msgLength = -1;
                if (cacheNum - nowIndex >= 8)
                {
                    msgID = BitConverter.ToInt32(cachaBytes, nowIndex);
                    nowIndex += 4;
                    msgLength = BitConverter.ToInt32(cachaBytes, nowIndex);
                    nowIndex += 4;
                }

                if (cacheNum - nowIndex >= msgLength && msgLength != -1)
                {
                    AsyncData.BaseMsg msg = null;
                    switch (msgID)
                    {
                        case 10086:
                            msg = new AsyncData.PlayerMsg();
                            msg.ReadFromBytes(cachaBytes, nowIndex);
                            break;
                    }

                    if (msg != null)
                        receiveQue.Enqueue(msg);
                    nowIndex += msgLength;
                    if (nowIndex == cacheNum)
                    {
                        cacheNum = 0;
                        break;
                    }
                }
                else
                {
                    if (msgLength != -1)
                    {
                        nowIndex -= 8;
                    }

                    //就是把剩余没有解析的字节数组内容 移到前面来 用于缓存下次继续解析
                    Array.Copy(cachaBytes, nowIndex, cachaBytes, 0, cacheNum - nowIndex);
                    cacheNum = cacheNum - nowIndex;
                    break;
                }
            }
        }

        private void OnDestroy()
        {
            Close();
        }
    }
}