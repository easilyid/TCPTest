using System.IO;
using Google.Protobuf;

namespace Protocol
{
    public class NetTools
    {
        public static byte[] GetProtoBytes(IMessage msg)
        {
            // byte[] bytes = null;
            // using (MemoryStream ms=new MemoryStream())
            // {
            //     msg.WriteTo(ms);
            //     bytes= ms.ToArray();
            // }
            //
            // return bytes;
            return msg.ToByteArray();
        }
    }
}