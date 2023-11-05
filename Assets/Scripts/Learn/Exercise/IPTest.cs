using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Learn.Exercise
{
    public class IPTest : MonoBehaviour
    {
        private void Start()
        {
            print(Dns.GetHostName());
            //同步获取  域名的IP信息 会阻塞当前线程
            IPHostEntry ipHostEntry = Dns.GetHostEntry("www.fussen.top");
            for (int i = 0; i < ipHostEntry.AddressList.Length; i++)
            {
                print("IP地址" + ipHostEntry.AddressList[i]);
            }

            for (int i = 0; i < ipHostEntry.Aliases.Length; i++)
            {
                print("主机别名" + ipHostEntry.Aliases[i]);
            }

            print("DNS服务器名称" + ipHostEntry.HostName);

            //异步获取
            GetHostEntry();
        }

        private async void GetHostEntry()
        {
            Task<IPHostEntry> hostEntryAsync = Dns.GetHostEntryAsync("www.baidu.com");
            await hostEntryAsync;
            for (int i = 0; i < hostEntryAsync.Result.AddressList.Length; i++)
            {
                print("IP地址" + hostEntryAsync.Result.AddressList[i]);
            }

            for (int i = 0; i < hostEntryAsync.Result.Aliases.Length; i++)
            {
                print("主机别名" + hostEntryAsync.Result.Aliases[i]);
            }

            print("DNS服务器名称" + hostEntryAsync.Result.HostName);
        }
    }

    class IPTest2
    {
        void Ip()
        {
            byte[] bytes = new byte[] { 127, 0, 0, 1 };
            IPAddress ipAddress1 = new IPAddress(bytes);
            ipAddress1 = new IPAddress(0X7F000001); //long 十六进制初始化
            ipAddress1 = IPAddress.Parse("127.0.0.1"); //推荐使用方式 使用字符串转换
            //IPAddress.IPv6Any 获取IPV6的任意地址
            //IPEndPoint类将网络端点表示为IP地址和端口号，表现为IP地址和端口号的组合
            IPEndPoint ipe = new IPEndPoint(0X7F000001, 80);
            ipe = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 80);
        }

        void IPParse()
        {
            //域名解析 域名 DNS 解析域名能得到IP地址  
            //IPHostEntry 主要作用：域名解析后的返回值 可以通过该对象获取IP地址、主机名等等信息
            //该类不会自己声明，都是作为某些方法的返回值返回信息，我们主要通过该类对象获取返回的信息
            //获取关联IP       成员变量:AddressList
            //获取主机别名列表  成员变量:Aliases
            //获取DNS名称      成员变量:HostName
            IPHostEntry ipHostEntry = Dns.GetHostEntry("www.baidu.com");
            Dns.GetHostName();
        }

        void StrConvert()
        {
            //非字符串转换成字节数组
            byte[] bytes = BitConverter.GetBytes(127);
        
            var i = BitConverter.ToInt32(bytes, 0);
        
            //字符串转换成字节数组
            byte[] bytes1 = Encoding.UTF8.GetBytes("哈哈哈哈");


            PlayerInfo p = new PlayerInfo();
            p.lev = 1;
            p.name = "哈哈哈";
            p.atk = 88;
            p.isOk = true;
            //装载前要明确数据类型的长度 int 4个字节  string类型我们在他的前面先放一个int大小的 然后加上 目的是告诉从现在开始多少位后面都是字符串数组方便后续获取
            //short 2个字节 bool 1个字节
            int indexNum = sizeof(int) + sizeof(int) + Encoding.UTF8.GetBytes(p.name).Length + sizeof(short) + sizeof(bool);
            byte[] bytes3 = new byte[indexNum];
            int index = 0;//记录当前装载的位置
            BitConverter.GetBytes(p.lev).CopyTo(bytes3,index);
            index += sizeof(int);
            var strbytes = Encoding.UTF8.GetBytes(p.name);
            //这个存的是姓名转为字符串数组后 的长度
            BitConverter.GetBytes(strbytes.Length).CopyTo(bytes3,index);
            index += sizeof(int);
            strbytes.CopyTo(bytes3,index);
            index+=strbytes.Length;
            BitConverter.GetBytes(p.atk).CopyTo(bytes3,index);
            index += sizeof(short);
            BitConverter.GetBytes(p.isOk).CopyTo(bytes3,index);
            index += sizeof(bool); //现在这个bytes3就是我们要传输的数据了
            //解析
        }

        class PlayerInfo
        {
            public int lev;
            public string name;
            public short atk;
            public bool isOk;
        }
    }

    public class TestInfo : BaseGetData
    {
        public short lev;
        public Player p;
        public int hp;
        public string name;
        public bool sex;

        public override int GetBytesNum()
        {
            return sizeof(short) +//2 
                   p.GetBytesNum() + //4
                   sizeof(int) +//4
                   4 + Encoding.UTF8.GetBytes(name).Length + //4+n
                   sizeof(bool);//1
        }

        public override byte[] Writing()
        {
            int index = 0;
            byte[] bytes = new byte[GetBytesNum()];
            WriteShort(bytes, lev, ref index);
            WriteData(bytes, p, ref index);
            WriteInt(bytes, hp, ref index);
            WriteString(bytes, name, ref index);
            WriteBool(bytes, sex, ref index);
            return bytes;
        }

        public override int Reading(byte[] bytes, int index)
        {
            throw new NotImplementedException();
        }
    }

    public class Player : BaseGetData
    {
        public int atk;
        public override int GetBytesNum()
        {
            return 4;
        }

        public override byte[] Writing()
        {
            int index = 0;
            byte[] bytes = new byte[GetBytesNum()];
            WriteInt(bytes, atk, ref index);
            return bytes;
        }

        public override int Reading(byte[] bytes, int index)
        {
            throw new NotImplementedException();
        }
    }
}