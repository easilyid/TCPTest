using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Learn.FTP.Exercise
{
    public class FtpMgr
    {
        private static FtpMgr _instance = new FtpMgr();
        public static FtpMgr Instance => _instance;

        private string FTP_PATH = "ftp://127.0.0.1/FTPTest/";
        private string USER_NAME = "Heart";
        private string PASS_WORD = "123456";

        /// <summary>
        /// 上传到FTP      
        /// </summary>
        /// <param name="fileName">远端想要的文件名</param>
        /// <param name="localPath">本地路径+本地的文件名</param>
        /// <param name="callBack">回调函数</param>
        public async void UpLoad(string fileName, string localPath, UnityAction callBack = null)
        {
            await Task.Run(() =>
            {
                try
                {
                    //通过线程处理逻辑 不会阻塞主线程
                    FtpWebRequest request = FtpWebRequest.Create(new Uri(FTP_PATH + fileName)) as FtpWebRequest;
                    NetworkCredential n = new NetworkCredential(USER_NAME, PASS_WORD);
                    request.Credentials = n;
                    request.KeepAlive = false;
                    request.Method = WebRequestMethods.Ftp.UploadFile;
                    request.UseBinary = true;
                    request.Proxy = null;
                    var uploadStream = request.GetRequestStream();
                    using (FileStream fs = File.OpenRead(localPath))
                    {
                        byte[] bytes = new byte[1024];
                        int len = fs.Read(bytes, 0, bytes.Length);
                        while (len != 0)
                        {
                            uploadStream.Write(bytes, 0, len);
                            len = fs.Read(bytes, 0, bytes.Length);
                        }

                        fs.Close();
                        uploadStream.Close();
                        Debug.Log("上传完成");
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError("上传失败" + e.Message);
                }
            });

            callBack?.Invoke();
        }

        /// <summary>
        /// 下载到本地
        /// </summary>
        /// <param name="fileName">远端的文件名</param>
        /// <param name="localPath">本地的路径 + 想要的文件名字</param>
        /// <param name="callBack"></param>
        public async void DownLoad(string fileName, string localPath, UnityAction callBack = null)
        {
            await Task.Run(() =>
            {
                try
                {
                    FtpWebRequest request = FtpWebRequest.Create(new Uri(FTP_PATH + fileName)) as FtpWebRequest;
                    NetworkCredential n = new NetworkCredential(USER_NAME, PASS_WORD);
                    request.Credentials = n;
                    request.KeepAlive = false;
                    request.Method = WebRequestMethods.Ftp.DownloadFile;
                    request.UseBinary = true;
                    request.Proxy = null;
                    var downLoadStream = request.GetResponse().GetResponseStream();
                    FileStream fs = new FileStream(localPath, FileMode.Create);
                    byte[] bytes = new byte[1024];
                    int len = downLoadStream.Read(bytes, 0, bytes.Length);
                    while (len != 0)
                    {
                        fs.Write(bytes, 0, len);
                        len = downLoadStream.Read(bytes, 0, bytes.Length);
                    }

                    Debug.Log("下载完成");
                    fs.Close();
                    downLoadStream.Close();
                }
                catch (Exception e)
                {
                    Debug.LogError("下载失败" + e.Message);
                }
            });

            callBack?.Invoke();
        }

        public async void DeleteFile(string fileName, UnityAction<bool> action = null)
        {
            try
            {
                FtpWebRequest request = FtpWebRequest.Create(new Uri(FTP_PATH + fileName)) as FtpWebRequest;
                NetworkCredential n = new NetworkCredential(USER_NAME, PASS_WORD);
                request.Credentials = n;
                request.KeepAlive = false;
                request.Method = WebRequestMethods.Ftp.DeleteFile;
                request.UseBinary = true;
                request.Proxy = null;
                var response = request.GetResponse() as FtpWebResponse;
                response.Close();
                action?.Invoke(true);
            }
            catch (Exception e)
            {
                action?.Invoke(false);
                Debug.LogError("删除失败" + e.Message);
            }
        }

        /// <summary>
        /// 获取FTP服务器上某个文件的大小 （单位 是 字节）
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="action">获取成功后传递给外部 具体的大小</param>
        public async void GetFileSize(string fileName, UnityAction<long> action = null)
        {
            await Task.Run(() =>
            {
                try
                {
                    //通过一个线程执行这里面的逻辑 那么就不会影响主线程了
                    //1.创建一个Ftp连接
                    FtpWebRequest req = FtpWebRequest.Create(new Uri(FTP_PATH + fileName)) as FtpWebRequest;
                    //2.进行一些设置
                    //凭证
                    req.Credentials = new NetworkCredential(USER_NAME, PASS_WORD);
                    //是否操作结束后 关闭 控制连接
                    req.KeepAlive = false;
                    //传输类型
                    req.UseBinary = true;
                    //操作类型
                    req.Method = WebRequestMethods.Ftp.GetFileSize;
                    //代理设置为空
                    req.Proxy = null;
                    //3.真正的获取
                    FtpWebResponse res = req.GetResponse() as FtpWebResponse;
                    //把大小传递给外部
                    action?.Invoke(res.ContentLength);

                    res.Close();
                }
                catch (Exception e)
                {
                    Debug.Log("获取大小失败" + e.Message);
                    action?.Invoke(0);
                }
            });
        }


        /// <summary>
        /// 创建一个文件夹 在FTP服务器上
        /// </summary>
        /// <param name="directoryName">文件夹名字</param>
        /// <param name="action">创建完成后的回调</param>
        public async void CreateDirectory(string directoryName, UnityAction<bool> action = null)
        {
            await Task.Run(() =>
            {
                try
                {
                    //通过一个线程执行这里面的逻辑 那么就不会影响主线程了
                    //1.创建一个Ftp连接
                    FtpWebRequest req = FtpWebRequest.Create(new Uri(FTP_PATH + directoryName)) as FtpWebRequest;
                    //2.进行一些设置
                    //凭证
                    req.Credentials = new NetworkCredential(USER_NAME, PASS_WORD);
                    //是否操作结束后 关闭 控制连接
                    req.KeepAlive = false;
                    //传输类型
                    req.UseBinary = true;
                    //操作类型
                    req.Method = WebRequestMethods.Ftp.MakeDirectory;
                    //代理设置为空
                    req.Proxy = null;
                    //3.真正的创建
                    FtpWebResponse res = req.GetResponse() as FtpWebResponse;
                    res.Close();

                    action?.Invoke(true);
                }
                catch (Exception e)
                {
                    Debug.Log("创建文件夹失败" + e.Message);
                    action?.Invoke(false);
                }
            });
        }

        /// <summary>
        /// 过去所有文件名
        /// </summary>
        /// <param name="directoryName">文件夹路径</param>
        /// <param name="action">返回给外部使用的 文件名列表</param>
        public async void GetFileList(string directoryName, UnityAction<List<string>> action = null)
        {
            await Task.Run(() =>
            {
                try
                {
                    //通过一个线程执行这里面的逻辑 那么就不会影响主线程了
                    //1.创建一个Ftp连接
                    FtpWebRequest req = FtpWebRequest.Create(new Uri(FTP_PATH + directoryName)) as FtpWebRequest;
                    //2.进行一些设置
                    //凭证
                    req.Credentials = new NetworkCredential(USER_NAME, PASS_WORD);
                    //是否操作结束后 关闭 控制连接
                    req.KeepAlive = false;
                    //传输类型
                    req.UseBinary = true;
                    //操作类型
                    req.Method = WebRequestMethods.Ftp.ListDirectory;
                    //代理设置为空
                    req.Proxy = null;
                    //3.真正的创建
                    FtpWebResponse res = req.GetResponse() as FtpWebResponse;
                    //把下载的信息流 转换成StreamReader对象 方便我们一行一行的读取信息
                    StreamReader streamReader = new StreamReader(res.GetResponseStream());

                    //用于存储文件名的列表
                    List<string> nameStrs = new List<string>();
                    //一行行的读取
                    string line = streamReader.ReadLine();
                    while (line != null)
                    {
                        nameStrs.Add(line);
                        line = streamReader.ReadLine();
                    }

                    res.Close();

                    action?.Invoke(nameStrs);
                }
                catch (Exception e)
                {
                    Debug.Log("获取文件列表失败" + e.Message);
                    action?.Invoke(null);
                }
            });
        }
    }
}