namespace Learn.Exercise.AsyncTest.AsyncData
{
    public class QuitMsg : BaseMsg
    {
        public override int GetByteNum()
        {
            return 8;
        }

        public override byte[] WriteToBytes()
        {
            int index = 0;
            byte[] bytes = new byte[GetByteNum()];
            WriteInt(bytes, GetID(), ref index);
            WriteInt(bytes, 0, ref index);
            return bytes;
        }

        public override int ReadFromBytes(byte[] bytes, int startIndex = 0)
        {
            return 0;
        }

        public override int GetID()
        {
            return 10087;
        }
    }
}