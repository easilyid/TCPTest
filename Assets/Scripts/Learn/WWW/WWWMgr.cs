using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;


public class WWWMgr : MonoBehaviour
{
    //配合协程 所以继承MonoBehaviour
    private static WWWMgr _instance;
    public static WWWMgr Instance => _instance;

    private void Awake()
    {
        _instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    /// <summary>
    /// 提供给外部调用的加载资源的方法
    /// </summary>
    /// <typeparam name="T">资源的类型</typeparam>
    /// <param name="path">资源的路径 http ftp file都支持</param>
    /// <param name="action">加载结束后的回调函数 因为WWW是通过结合协同程序异步加载的 所以不能马上获取结果 需要回调获取</param>
    public void LoadRes<T>(string path, UnityAction<T> callBack) where T : class
    {
        StartCoroutine(LoadResAsync(path, callBack));
    }

    private IEnumerator LoadResAsync<T>(string path, UnityAction<T> action) where T : class
    {
        WWW www = new WWW(path);
        yield return www;
        if (www.error == null)
        {
            //根据类型 来传递不同的资源
            if (typeof(T) == typeof(AssetBundle))
                action(www.assetBundle as T);
            else if (typeof(T) == typeof(AudioClip))
                action(www.GetAudioClip() as T);
            else if (typeof(T) == typeof(Texture2D))
                action(www.texture as T);
            else if (typeof(T) == typeof(Texture))
                action(www.texture as T);
            else if (typeof(T) == typeof(string))
                action(www.text as T);
            else if (typeof(T) == typeof(byte[]))
                action(www.bytes as T);
            //自定义类型 就需要用bytes去转换
            else
                Debug.LogError("不支持的类型" + typeof(T));
        }
        else
        {
            Debug.LogError("资源加载失败" + www.error);
        }
    }

    /// <summary>
    /// 上传文件
    /// </summary>
    /// <param name="fileName">要上传文件名</param>
    /// <param name="localPath">本地想要上传的文件的路径</param>
    /// <param name="action">回调</param>
    public void UploadFile(string fileName, string localPath, UnityAction<UnityWebRequest.Result> action)
    {
        StartCoroutine(UploadFileAsync(fileName, localPath, action));
    }

    private IEnumerator UploadFileAsync(string fileName, string localPath, UnityAction<UnityWebRequest.Result> action)
    {
        List<IMultipartFormSection> dataList = new List<IMultipartFormSection>();
        //添加文件数据 
        dataList.Add(new MultipartFormFileSection(fileName, File.ReadAllBytes(localPath)));

        UnityWebRequest req = UnityWebRequest.Post("http://192.168.145.227:8000/Http_Server/", dataList);

        yield return req.SendWebRequest();
        action?.Invoke(req.result);
        if (req.result != UnityWebRequest.Result.Success)
        {
            Debug.LogWarning("上传出现问题" + req.result + req.error);
        }
    }
}