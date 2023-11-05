using System;
using System.Collections;
using Learn.Exercise.AsyncTest;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class AsyncTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // CountDownAsnc(5, () => { print("异步倒计时结束回调"); });
        // print("倒计时结束");
        //CountDownAsnc2(5, () => { print("异步倒计时回调2"); });
        StartCoroutine(CountDownAsnc3(5, () => { print("异步倒计时回调3"); }));
    }

    //
    public void CountDownAsnc(int second, UnityAction callBack)
    {
        Thread thread = new Thread(() =>
        {
            //通过开启一个线程 
            while (true)
            {
                print(second);
                --second;
                Thread.Sleep(1000);
                if (second < 0)
                    break;
            }

            callBack?.Invoke();
        });

        thread.Start();
        print("开始倒计时");
    }

    public async void CountDownAsnc2(int second, UnityAction callBcak)
    {
        print("倒计时开始");
        //这个写法就从上到下执行 得等 await 里的逻辑执行完 再执行下面的
        await Task.Run(() =>
        {
            while (true)
            {
                print(second);
                --second;
                Thread.Sleep(1000);
                if (second < 0)
                    break;
            }

            callBcak?.Invoke(); //回调 在外部施加逻辑
        });

        print("倒计时结束");
    }

    public IEnumerator CountDownAsnc3(int second, UnityAction callBack)
    {
        print("倒计时开始");
        while (true)
        {
            print(second);
            --second;
            yield return new WaitForSeconds(1);
            if (second < 0)
                break;
        }

        callBack?.Invoke();

        print("倒计时结束");
    }

    //TCP异步相关
    private byte[] resultBytes = new byte[1024];

    public void TcpAsync()
    {
        Socket socketTcp = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //服务器相关
        //BeginAccept
        //EndAccept
        socketTcp.BeginAccept(AcceptCallBack, socketTcp);

        //客户端相关
        //BeginConnect
        //EndConnect
        IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8080);
        socketTcp.BeginConnect(ipPoint, (result) =>
        {
            Socket s = result.AsyncState as Socket;
            try
            {
                s.EndConnect(result);
                print("连接成功");
            }
            catch (SocketException e)
            {
                print("连接出错" + e.SocketErrorCode + e.Message);
            }
        }, socketTcp);


        //服务器客户端通用
        //接收消息
        //BeginReceive
        //EndReceive
        socketTcp.BeginReceive(resultBytes, 0, resultBytes.Length, SocketFlags.None, ReceiveCallBack, socketTcp);

        //发送消息
        //BeginSend
        //EndSend
        byte[] bytes = Encoding.UTF8.GetBytes("1231231231223123123");
        socketTcp.BeginSend(bytes, 0, bytes.Length, SocketFlags.None, (result) =>
        {
            try
            {
                //这里就是发送消息 只要EndSend不报错 就发送成功
                socketTcp.EndSend(result);
                print("发送成功");
            }
            catch (SocketException e)
            {
                print("发送错误" + e.SocketErrorCode + e.Message);
            }
        }, socketTcp);


        //关键变量类型
        //SocketAsyncEventArgs
        //它会作为Async异步方法的传入值
        //我们需要通过它进行一些关键参数的赋值

        //服务器端
        //AcceptAsync
        SocketAsyncEventArgs e = new SocketAsyncEventArgs();
        e.Completed += (socket, args) =>
        {
            //首先判断是否成功
            if (args.SocketError == SocketError.Success)
            {
                //获取连入的客户端socket
                Socket clientSocket = args.AcceptSocket;

                (socket as Socket).AcceptAsync(args);
            }
            else
            {
                print("连入客户端失败" + args.SocketError);
            }
        };
        socketTcp.AcceptAsync(e);

        //客户端
        //ConnectAsync
        SocketAsyncEventArgs e2 = new SocketAsyncEventArgs();
        e2.Completed += (socket, args) =>
        {
            if (args.SocketError == SocketError.Success)
            {
                //连接成功
            }
            else
            {
                //连接失败
                print(args.SocketError);
            }
        };
        socketTcp.ConnectAsync(e2);

        //服务端和客户端
        //发送消息
        //SendAsync
        SocketAsyncEventArgs e3 = new SocketAsyncEventArgs();
        byte[] bytes2 = Encoding.UTF8.GetBytes("123123的就是拉法基萨克两地分居");
        e3.SetBuffer(bytes2, 0, bytes2.Length);
        e3.Completed += (socket, args) =>
        {
            if (args.SocketError == SocketError.Success)
            {
                print("发送成功");
            }
            else
            {
            }
        };
        socketTcp.SendAsync(e3);

        //接受消息
        //ReceiveAsync
        SocketAsyncEventArgs e4 = new SocketAsyncEventArgs();
        //设置接受数据的容器，偏移位置，容量
        e4.SetBuffer(new byte[1024 * 1024], 0, 1024 * 1024);
        e4.Completed += (socket, args) =>
        {
            if (args.SocketError == SocketError.Success)
            {
                //收取存储在容器当中的字节
                //Buffer是容器
                //BytesTransferred是收取了多少个字节
                Encoding.UTF8.GetString(args.Buffer, 0, args.BytesTransferred);

                args.SetBuffer(0, args.Buffer.Length);
                //接收完消息 再接收下一条
                (socket as Socket).ReceiveAsync(args);
            }
            else
            {
            }
        };
        socketTcp.ReceiveAsync(e4);
    }

    /// <summary>
    /// 接收回调
    /// </summary>
    /// <param name="result">回调函数中通过result参数获取到了异步操作的状态。</param>
    private void AcceptCallBack(IAsyncResult result)
    {
        try
        {
            //获取传入的参数
            Socket s = result.AsyncState as Socket;
            //通过调用EndAccept就可以得到连入的客户端Socket  clientSocket 获取到的客户端Socket
            Socket clientSocket = s.EndAccept(result);

            //为了实现一个循环的异步接受客户端连接
            s.BeginAccept(AcceptCallBack, s);
        }
        catch (SocketException e)
        {
            print(e.SocketErrorCode);
        }
    }

    private void ReceiveCallBack(IAsyncResult result)
    {
        try
        {
            Socket s = result.AsyncState as Socket;
            //这个返回值是你受到了多少个字节
            int num = s.EndReceive(result);
            //进行消息处理
            Encoding.UTF8.GetString(resultBytes, 0, num);

            //我还要继续接受
            s.BeginReceive(resultBytes, 0, resultBytes.Length, SocketFlags.None, ReceiveCallBack, s);
        }
        catch (SocketException e)
        {
            print("接受消息处问题" + e.SocketErrorCode + e.Message);
        }
    }
}