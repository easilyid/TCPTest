using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

namespace Learn.Exercise
{
    public class SocketTest : MonoBehaviour
    {
        //寻址方式 套接字类型 传输控制协议
        private Socket _tcp = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //private Socket _udp = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        private void Test()
        {
            //_tcp.SocketType 类型
            //_tcp.ProtocolType 协议类型
            // _tcp.AddressFamily 寻址方式
            //_tcp.Available; //可读取的字节数 准备读取的数据量
            //_tcp.LocalEndPoint as IPEndPoint; //本地端点
            // _tcp.RemoteEndPoint as IPEndPoint; 远程端点

            //主要用于服务器端 这里是同步 都带有异步的方法
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 80);
            _tcp.Bind(ipPoint); //绑定IP.端口
            _tcp.Listen(99); //监听客户端连接请求 99表示最大连接数
            _tcp.Accept(); //接受客户端连接请求  

            //主要用于客户端
            _tcp.Connect(IPAddress.Parse("127.0.0.1"), 80); //连接服务器
            //都会用的
            //_tcp.Send(bytes) _tcp.Receive(bytes);//发送和接收数据
            _tcp.Shutdown(SocketShutdown.Both); //禁用套接字的发送和接收
            _tcp.Close(); //关闭套接字
        }

        private void Start()
        {
            try
            {
                IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8080); //因为连接是自己 所以用本地IP
                _tcp.Connect(ipEndPoint);
            }
            catch (SocketException e)
            {
                if (e.ErrorCode == 10086)
                {
                    print("连接被服务器拒绝");
                }
                else
                    print("连接服务器失败" + e.ErrorCode);
            }

            byte[] bytes = new byte[1024];
            int receiveNum = _tcp.Receive(bytes);
            
            //DataTest 解析演示
            int msgID = BitConverter.ToInt32(bytes, 0);//这个解析的就是标识ID
            switch (msgID)
            {
                case 10086:
                    PlayerMsg playerMsg = new PlayerMsg();
                    playerMsg.Reading(bytes, 4);
                    print(playerMsg.ID);
                    print(playerMsg.data.name);
                    print(playerMsg.data.atk);
                    print(playerMsg.data.lev);
                    break;
            }
            
            
            print("接收到了" + _tcp.RemoteEndPoint + "发来的消息:" + Encoding.UTF8.GetString(bytes, 0, receiveNum));
            _tcp.Send(Encoding.UTF8.GetBytes("你好服务器"));

            _tcp.Shutdown(SocketShutdown.Both);
            _tcp.Close();
        }
    }
}