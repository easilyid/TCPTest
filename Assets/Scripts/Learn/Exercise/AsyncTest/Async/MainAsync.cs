using UnityEngine;

namespace Learn.Exercise.AsyncTest.Async
{
    public class MainAsync: MonoBehaviour
    {
        private void Start()
        {
            if (NetAsyncMgr.Instance==null)
            {
                GameObject obj = new GameObject("NetAsync");
                obj.AddComponent<NetAsyncMgr>();
            }
            NetAsyncMgr.Instance.Connect("127.0.0.1",8081);
        }
    }
}