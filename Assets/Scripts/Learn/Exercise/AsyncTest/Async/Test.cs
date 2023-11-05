using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Learn.Exercise.AsyncTest.AsyncData;

namespace Learn.Exercise.AsyncTest.Async
{
    public class Test : MonoBehaviour
    {
        public Text txt;
        public Text txt2;
        public Button btn;
        public Button btn2;
        public Button btn3;
        public Button btn4;

        void Start()
        {
            btn.onClick.AddListener(() =>
            {
                AsyncData.PlayerMsg ms = new AsyncData.PlayerMsg();
                ms.ID = 1111;
                ms.Data = new AsyncData.PlayerData();
                ms.Data.name = "客户端发送的信息";
                ms.Data.atk = 22;
                ms.Data.lev = 10;
                NetAsyncMgr.Instance.Send(ms);
            });
            //分包测试
            btn2.onClick.AddListener(async () =>
            {
                AsyncData.PlayerMsg msg = new AsyncData.PlayerMsg();
                msg.ID = 1003;
                msg.Data = new AsyncData.PlayerData();
                msg.Data.name = "1";
                msg.Data.atk = 3;
                msg.Data.lev = 3;

                byte[] bytes = msg.WriteToBytes();
                //分包
                byte[] bytes1 = new byte[10];
                byte[] bytes2 = new byte[bytes.Length - 10];
                //分成第一个包
                Array.Copy(bytes, 0, bytes1, 0, 10);
                //第二个包
                Array.Copy(bytes, 10, bytes2, 0, bytes.Length - 10);

                NetAsyncMgr.Instance.SendTest(bytes1);
                await Task.Delay(500);
                NetAsyncMgr.Instance.SendTest(bytes2);
            });
            //黏包测试
            btn3.onClick.AddListener(() =>
            {
                AsyncData.PlayerMsg msg = new AsyncData.PlayerMsg();
                msg.ID = 1001;
                msg.Data = new AsyncData.PlayerData();
                msg.Data.name = "1";
                msg.Data.atk = 1;
                msg.Data.lev = 1;

                AsyncData.PlayerMsg msg2 = new AsyncData.PlayerMsg();
                msg2.ID = 1002;
                msg2.Data = new AsyncData.PlayerData();
                msg2.Data.name = "2";
                msg2.Data.atk = 2;
                msg2.Data.lev = 2;
                //黏包
                byte[] bytes = new byte[msg.GetByteNum() + msg2.GetByteNum()];
                msg.WriteToBytes().CopyTo(bytes, 0);
                msg2.WriteToBytes().CopyTo(bytes, msg.GetByteNum());
                NetAsyncMgr.Instance.SendTest(bytes);
            });

            //分包、黏包测试
            btn4.onClick.AddListener(async () =>
            {
                AsyncData.PlayerMsg msg = new AsyncData.PlayerMsg();
                msg.ID = 1001;
                msg.Data = new AsyncData.PlayerData();
                msg.Data.name = "1";
                msg.Data.atk = 1;
                msg.Data.lev = 1;

                AsyncData.PlayerMsg msg2 = new AsyncData.PlayerMsg();
                msg2.ID = 1002;
                msg2.Data = new AsyncData.PlayerData();
                msg2.Data.name = "2";
                msg2.Data.atk = 2;
                msg2.Data.lev = 2;

                byte[] bytes1 = msg.WriteToBytes(); //消息A
                byte[] bytes2 = msg2.WriteToBytes(); //消息B

                byte[] bytes2_1 = new byte[10];
                byte[] bytes2_2 = new byte[bytes2.Length - 10];
                //分成第一个包
                Array.Copy(bytes2, 0, bytes2_1, 0, 10);
                //第二个包
                Array.Copy(bytes2, 10, bytes2_2, 0, bytes2.Length - 10);

                //消息A和消息B前一段的 黏包
                byte[] bytes = new byte[bytes1.Length + bytes2_1.Length];
                bytes1.CopyTo(bytes, 0);
                bytes2_1.CopyTo(bytes, bytes1.Length);

                NetAsyncMgr.Instance.SendTest(bytes);
                await Task.Delay(500);
                NetAsyncMgr.Instance.SendTest(bytes2_2);
            });
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}