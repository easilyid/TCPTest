using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Learn.Http
{
    public class HttpMgr
    {
        private static HttpMgr _instance = new HttpMgr();
        public static HttpMgr Instance => _instance;

        private string HTTP_PATH = "http://192.168.145.227:8000/Http_Server/";
        private string USER_NAME = "Heart";
        private string PASS_WORD = "123456";

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="localPath">本地路径</param>
        /// <param name="callBack">毁掉函数</param>
        public async void DownLoadFile(string fileName, string localPath, UnityAction<HttpStatusCode> callBack)
        {
            HttpStatusCode result = HttpStatusCode.OK;
            //先判断文件是否存在 再下载
            await Task.Run(() =>
            {
                try
                {
                    HttpWebRequest req = HttpWebRequest.Create(HTTP_PATH + fileName) as HttpWebRequest;
                    req.Method = WebRequestMethods.Http.Head;
                    req.Timeout = 2000; //超时时间
                    HttpWebResponse res = req.GetResponse() as HttpWebResponse;
                    if (res.StatusCode == HttpStatusCode.OK)
                    {
                        res.Close();
                        //下载
                        req = HttpWebRequest.Create(HTTP_PATH + fileName) as HttpWebRequest;
                        req.Method = WebRequestMethods.Http.Get;
                        req.Timeout = 3000;
                        res = req.GetResponse() as HttpWebResponse;
                        if (res.StatusCode == HttpStatusCode.OK)
                        {
                            using (FileStream fs = File.Create(localPath))
                            {
                                Stream stream = res.GetResponseStream();
                                byte[] bytes = new byte[1024];
                                int len = stream.Read(bytes, 0, bytes.Length);
                                while (len != 0)
                                {
                                    fs.Write(bytes, 0, len);
                                    len = stream.Read(bytes, 0, bytes.Length);
                                }

                                fs.Close();
                                stream.Close();
                            }

                            result = HttpStatusCode.OK;
                        }
                        else
                        {
                            result = res.StatusCode;
                        }
                    }
                    else
                    {
                        result = res.StatusCode;
                    }
                }
                catch (WebException e)
                {
                    result = HttpStatusCode.InternalServerError;
                    Debug.Log("下载出错" + e.Message + e.Status);
                }

                //回调 给外部通知下载状态
                callBack?.Invoke(result);
            });
        }


        public async void UpLoadFile(string fileName, string localPath, UnityAction<HttpStatusCode> callBack)
        {
            HttpStatusCode result = HttpStatusCode.BadRequest;
            await Task.Run(() =>
            {
                try
                {
                    HttpWebRequest request =
                        HttpWebRequest.Create(HTTP_PATH) as HttpWebRequest;
                    request.Method = WebRequestMethods.Http.Post;
                    request.Timeout = 500000;
                    request.ContentType = "multipart/form-data;boundary=Heart";
                    request.Credentials = new NetworkCredential(USER_NAME, PASS_WORD);
                    request.PreAuthenticate = true;

                    string head = "--Heart\r\n" +
                                  "Content-Disposition:form-data;name=\"file\";filename=\"{0}\"\r\n" +
                                  "Content-Type:application/octet-stream\r\n\r\n";
                    //替换文件名
                    head = string.Format(head, fileName);
                    byte[] headBytes = Encoding.UTF8.GetBytes(head);
                    byte[] endBytes = Encoding.UTF8.GetBytes("\r\n--Heart--\r\n");

                    using (FileStream localStream = File.OpenRead(localPath))
                    {
                        request.ContentLength = headBytes.Length + localStream.Length + endBytes.Length;

                        Stream upStream = request.GetRequestStream();
                        upStream.Write(headBytes, 0, headBytes.Length);
                        byte[] bytes = new byte[2048];
                        int len = localStream.Read(bytes, 0, bytes.Length);
                        while (len != 0)
                        {
                            upStream.Write(bytes, 0, len);
                            len = localStream.Read(bytes, 0, bytes.Length);
                        }

                        upStream.Write(endBytes, 0, endBytes.Length);
                        upStream.Close();
                        localStream.Close();
                    }

                    HttpWebResponse res = request.GetResponse() as HttpWebResponse;
                    result = res.StatusCode;
                    res.Close();
                }
                catch (WebException e)
                {
                    Debug.Log("上传失败" + e.Message + e.Status);
                }
            });

            callBack?.Invoke(result);
        }
    }
}