using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class WWWTest : MonoBehaviour
{
    public RawImage image;


    private void Start()
    {
        if (WWWMgr.Instance == null)
        {
            GameObject obj = new GameObject("WWWMgr");
            obj.AddComponent<WWWMgr>();
        }

        // WWWMgr.Instance.LoadRes<Texture>("http://192.168.145.227:8000/Http_Server/IU.png", (obj) =>
        // {
        //     image.texture = obj;
        // });
        //
        //
        // WWWMgr.Instance.LoadRes<byte[]>("http://192.168.145.227:8000/Http_Server/IU.png", (obj) =>
        // {//也可以直接下载到本地
        //     print(Application.persistentDataPath);
        //     File.WriteAllBytes(Application.persistentDataPath+ "/IU.png", obj);
        // });

        //StartCoroutine(DownLoadHttp());

        WWWMgr.Instance.UploadFile("UnityWebRequest上传.jpg", Application.streamingAssetsPath + "/12.jpg", (code) =>
        {
            if (code == UnityWebRequest.Result.Success)
                print("上传成功");
            else
                print("上传失败" + code);
        });
    }

    IEnumerator DownLoadHttp()
    {
        //1.创建WWW对象
        WWW www = new WWW(
            "http://gimg2.baidu.com/image_search/src=http%3A%2F%2Fsafe-img.xhscdn.com%2Fbw%2Fbc593313-2728-41e1-9596-4871745494b0%3FimageView2%2F2%2Fw%2F1080%2Fformat%2Fjpg&refer=http%3A%2F%2Fsafe-img.xhscdn.com&app=2002&size=f9999,10000&q=a80&n=0&g=0n&fmt=auto?sec=1700554141&t=510fb2cf2c39f52daca552bd92d40708");

        //2.就是等待加载结束
        while (!www.isDone)
        {
            print(www.bytesDownloaded);
            print(www.progress);
            yield return null;
        }

        print(www.bytesDownloaded);
        print(www.progress);

        //3.使用加载结束后的资源
        if (www.error == null)
        {
            image.texture = www.texture;
        }
        else
            print(www.error);
    }

    IEnumerator DownLoadFtp()
    {
        //1.创建WWW对象
        WWW www = new WWW("ftp://127.0.0.1/Test.jpg");

        //2.就是等待加载结束
        while (!www.isDone)
        {
            print(www.bytesDownloaded);
            print(www.progress);
            yield return null;
        }

        print(www.bytesDownloaded);
        print(www.progress);

        //3.使用加载结束后的资源
        if (www.error == null)
        {
            image.texture = www.texture;
        }
        else
            print(www.error);
    }

    IEnumerator DownLoadLocal()
    {
        //1.创建WWW对象
        WWW www = new WWW("file://" + Application.streamingAssetsPath + "/test.png");

        //2.就是等待加载结束
        while (!www.isDone)
        {
            print(www.bytesDownloaded);
            print(www.progress);
            yield return null;
        }

        print(www.bytesDownloaded);
        print(www.progress);

        //3.使用加载结束后的资源
        if (www.error == null)
        {
            image.texture = www.texture;
        }
        else
            print(www.error);
    }
}