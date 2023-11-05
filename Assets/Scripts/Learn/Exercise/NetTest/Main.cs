using System;
using UnityEngine;

namespace Learn.Exercise.NetTest
{
    public class Main: MonoBehaviour
    {
        private void Start()
        {
            if (NetMgr.Instance==null)
            {
                GameObject obj = new GameObject("Net");
                obj.AddComponent<NetMgr>();
            }
            
            NetMgr.Instance.Connect("127.0.0.1",8081);
        }
    }
}