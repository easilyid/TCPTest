using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;


namespace GenerateTools
{
    public class ProtobufTools
    {
        private static string PROTO_PATH = Application.dataPath + "/Protobuf/Proto/";
        private static string PROTOEXE_PATH = Application.dataPath + "/Protobuf/Proto/protoc.exe";

        private static string CSHARP_PATH = Application.dataPath + "/Protobuf/Proto/Csharp/";

        [MenuItem("ProtobufTool/生成C#代码")]
        private static void GeneratrCsharp()
        {
            DirectoryInfo info = Directory.CreateDirectory(PROTO_PATH);

            FileInfo[] files = info.GetFiles();
            foreach (var file in files)
            {
                if (file.Extension == ".proto")
                {
                    Debug.Log(file.Name);
                    //根据文件内容 生成C#代码
                    Process cmd = new Process();
                    cmd.StartInfo.FileName = PROTOEXE_PATH;
                    cmd.StartInfo.Arguments = $"-I={PROTO_PATH} --csharp_out={CSHARP_PATH} {file.Name}";
                    cmd.Start();
                    Debug.Log(file.Name + "生成结束");
                }
            }
        }
    }
}