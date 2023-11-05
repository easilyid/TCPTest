using System.Text;

namespace Learn.Exercise.AsyncTest.AsyncData
{
    public class PlayerData : BaseData
    {
        public string name;
        public int atk;
        public int lev;

        public override int GetByteNum()
        {
            return sizeof(int) + sizeof(int) + 4 + Encoding.UTF8.GetBytes(name).Length;
        }

        public override byte[] WriteToBytes()
        {
            int index = 0;
            int num = GetByteNum();
            byte[] bytes = new byte[num];
            WriteString(bytes, name, ref index);
            WriteInt(bytes, atk, ref index);
            WriteInt(bytes, lev, ref index);
            return bytes;
        }

        public override int ReadFromBytes(byte[] bytes, int startIndex = 0)
        {
            int index = startIndex;
            name = ReadString(bytes, ref index);
            atk = ReadInt(bytes, ref index);
            lev = ReadInt(bytes, ref index);
            return index - startIndex;
        }
    }
}