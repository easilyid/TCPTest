using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class UnityWebRequestTest : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(LoadText());
    }

    IEnumerator LoadText()
    {//get 操作
        UnityWebRequest req = UnityWebRequest.Get("http://192.168.145.227:8000/Http_Server/111.txt");

        yield return req.SendWebRequest();
        if (req.result == UnityWebRequest.Result.Success)
        {
            print(req.downloadHandler.text);
            byte[] bytes = req.downloadHandler.data;
            print("字节数组长度" + bytes.Length);
        }
        else
            print("获取失败" + req.result + req.error + req.responseCode);
    }
    
    
    private void Interface()
    {
        //数据相关类
        List<IMultipartFormSection> dataList = new List<IMultipartFormSection>();
        //子类数据  两个子类 一个是普通的数据 一个是文件数据 看重载
        dataList.Add(new MultipartFormDataSection(Encoding.UTF8.GetBytes("1212121")));
        dataList.Add(new MultipartFormFileSection("file", Encoding.UTF8.GetBytes("1212121"), "111.txt", "text/plain"));
    }
}


class TestT : DownloadHandlerScript
{
    public TestT(byte[] buffer) : base(buffer)
    {
    }

    protected override byte[] GetData()
    {
        throw new NotImplementedException();
    }

    protected override bool ReceiveData(byte[] data, int dataLength)
    {
        throw new NotImplementedException();
    }

    protected override float GetProgress()
    {
        throw new NotImplementedException();
    }
}
