using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Learn.Http;
using UnityEngine;

public class HttpTest : MonoBehaviour
{
    private void Start()
    {
        //DownFile();
        // HttpMgr.Instance.DownLoadFile("6.jpg", Application.persistentDataPath + "/httpDownloadTest.jpg", (code) =>
        // {
        //     if (code == HttpStatusCode.OK)
        //     {
        //         print("下载成功");
        //     }
        //     else
        //     {
        //         print("下载失败");
        //     }
        // });

        //UpFile();
        
        HttpMgr.Instance.UpLoadFile("封装后上传.jpg", Application.streamingAssetsPath + "/12.jpg", (code) =>
        {
            if (code == HttpStatusCode.OK)
                print("上传指令成功");
            else
                print("上传指令失败" + code);
        });
       
        
    }


    public void DownFile()
    {
        try
        {
            HttpWebRequest request =
                WebRequest.Create(new Uri("http://192.168.12.227:8000/Http_Server/6.jpg")) as HttpWebRequest;
            //request.Method = WebRequestMethods.Http.Head; //获取文件信息
            request.Method = WebRequestMethods.Http.Get; //下载文件
            request.Timeout = 3000;
            var response = request.GetResponse() as HttpWebResponse;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                print("文件存在");
                print(response.ContentLength); //文件大小
                print(response.ContentType); //文件类型
                //下载资源
                using (FileStream fs = File.Create(Application.persistentDataPath + "/httpDownload.jpg"))
                {
                    var downLoadStream = response.GetResponseStream();
                   var bytes = new byte[1024];
                    int len = downLoadStream.Read(bytes, 0, bytes.Length);
                    while (len != 0)
                    {
                        fs.Write(bytes, 0, len);
                        len = downLoadStream.Read(bytes, 0, bytes.Length);
                    }

                    fs.Close();
                    downLoadStream.Close();
                    response.Close();
                }

                print("下载完成");
            }
            else
                // print("文件不存在" + response.StatusCode);
                print("下载失败");
        }
        catch (WebException e)
        {
            // print("获取文件信息失败" + e.Message+e.Status);
            print("下载出错" + e.Message + e.Status);
        }
    }

    //上传文件时内容的必备规则
    //  1:ContentType = "multipart/form-data; boundary=边界字符串";
    //  2:上传的数据必须按照格式写入流中
    //  --边界字符串
    //  Content-Disposition: form-data; name="字段名字，之后写入的文件2进制数据和该字段名对应";filename="传到服务器上使用的文件名"
    //  Content-Type:application/octet-stream（由于我们传2进制文件 所以这里使用2进制）
    //  空一行
    //  （这里直接写入传入的内容）
    //  --边界字符串--
    public void UpFile()
    {
        print("开始上传");
        HttpWebRequest req = HttpWebRequest.Create("http://192.168.102.227:8000/Http_Server/") as HttpWebRequest;
        req.Method = WebRequestMethods.Http.Post;
        req.ContentType =
            "multipart/form-data;boubdary=Heart"; //文件类型 这是必须的规则 还有一些其他的类型 如application/x-www-form-urlencoded
        req.Timeout = 5000;
        //设置权限是很重要的
        req.Credentials = new NetworkCredential("Heart", "123456");
        req.PreAuthenticate = true; //这两个属性是必须的 先验证身份 再上传

        //头部信息
        string head = "--Heart\r\n" +
                      "Content-Disposition: form-data;name=\"file\";filename=\"http上传的文件.jpg\"\r\n" +
                      "Content-Type:application/octet-stream\r\n\r\n";
        byte[] headBytes = Encoding.UTF8.GetBytes(head);
        byte[] endBytes = Encoding.UTF8.GetBytes("\r\n--Heart--\r\n"); //边界字符串

        using (FileStream fs = File.OpenRead(Application.streamingAssetsPath + "/05.jpg"))
        {
            //设置上传的长度 头部+文件+尾部
            req.ContentLength = headBytes.Length + fs.Length + endBytes.Length;
            //获取请求流
            Stream upStream = req.GetRequestStream();
            //写入头部
            upStream.Write(headBytes, 0, headBytes.Length);
            //写入文件
            byte[] bytes = new byte[1024 * 1024];
            int len = fs.Read(bytes, 0, bytes.Length);
            while (len != 0)
            {
                upStream.Write(bytes, 0, len);
                len = fs.Read(bytes, 0, bytes.Length);
            }

            //写入尾部
            upStream.Write(endBytes, 0, endBytes.Length);
            fs.Close();
            upStream.Close();
        }

        //获取响应
        HttpWebResponse res = req.GetResponse() as HttpWebResponse;
        if (res.StatusCode == HttpStatusCode.OK)
        {
            print("上传成功");
        }
        else
        {
            print("上传失败" + res.StatusCode);
        }
    }
}