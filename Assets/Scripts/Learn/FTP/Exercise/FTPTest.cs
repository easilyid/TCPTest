using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Learn.FTP.Exercise;
using UnityEngine;

public class FTPTest : MonoBehaviour
{
    void Start()
    {
       // UpLoad();
        //DownLoad();
        
        FtpMgr.Instance.UpLoad("测试.jpg", Application.streamingAssetsPath + "/05.jpg", () =>
        {
            print("上传结束 调用的委托函数");

        });
        FtpMgr.Instance.DownLoad("6.jpg", Application.persistentDataPath + "/test2222.jpg",()=>
        {
            print("下载结束 调用的委托函数");
        });
    }

    private void DownLoad()
    {
        try
        {
            FtpWebRequest request = WebRequest.Create(new Uri("ftp://127.0.0.1/FTPTest/6.jpg")) as FtpWebRequest;
            request.Credentials = new NetworkCredential("Heart", "123456");
            request.KeepAlive = false;
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            request.UseBinary = true;
            request.Proxy = null;
            var response = request.GetResponse() as FtpWebResponse;
            Stream downLoadStream = response.GetResponseStream();
            print(Application.persistentDataPath);
            using (FileStream fs = File.Create(Application.persistentDataPath + "/test.jpg"))
            {
                byte[] bytes = new byte[1024];
                int len = downLoadStream.Read(bytes, 0, bytes.Length);
                while (len != 0)
                {
                    fs.Write(bytes, 0, len);
                    len = downLoadStream.Read(bytes, 0, bytes.Length);
                }

                print("下载完成");
                fs.Close();
                downLoadStream.Close();
            }
        }
        catch (Exception e)
        {
            print("下载失败" + e.Message);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void UpLoad()
    {
        try
        {
            FtpWebRequest request = WebRequest.Create(new Uri("ftp://127.0.0.1/FTPTest/test.txt")) as FtpWebRequest;
            request.Proxy = null;
            NetworkCredential n = new NetworkCredential("Heart", "123456");
            request.Credentials = n;
            request.KeepAlive = false;
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.UseBinary = true;
            // var requestStream = request.GetRequestStream();
            // using (FileStream fs = File.OpenRead(Application.streamingAssetsPath + "/test.txt"))
            // {
            //     byte[] bytes = new byte[1024];
            //     int len = fs.Read(bytes, 0, bytes.Length);
            //     while (len != 0)
            //     {
            //         requestStream.Write(bytes, 0, len);
            //         len = fs.Read(bytes, 0, bytes.Length);
            //     }
            //
            //     print("上传完成");
            //     fs.Close();
            //     requestStream.Close();
            // }

            var reqasync = request.GetRequestStreamAsync();
            reqasync.ContinueWith(t =>
            {
                using (FileStream fs = File.OpenRead(Application.streamingAssetsPath + "/test.txt"))
                {
                    byte[] bytes = new byte[1024];
                    int len = fs.Read(bytes, 0, bytes.Length);
                    while (len != 0)
                    {
                        t.Result.Write(bytes, 0, len);
                        len = fs.Read(bytes, 0, bytes.Length);
                    }

                    print("上传完成");
                    fs.Close();
                    t.Result.Close();
                }
            });
        }
        catch (Exception e)
        {
            print("上传失败" + e.Message);
        }
    }
}