namespace Learn.Exercise.AsyncTest.AsyncData
{

    public class PlayerMsg : BaseMsg
    {
        public int ID;
        public PlayerData Data;

        public override int GetByteNum()
        {
            return 4 + //消息ID的长度
                   4 + // 消息体的长度
                   sizeof(int) + //ID 的长度
                   Data.GetByteNum(); //Data的长度
        }

        public override byte[] WriteToBytes()
        {
            int index = 0;
            int num = GetByteNum();
            byte[] bytes = new byte[num];
            WriteInt(bytes, GetID(), ref index);
            WriteInt(bytes, num - 8, ref index);
            WriteInt(bytes, ID, ref index);
            WriteData(bytes, Data, ref index);
            return bytes;
        }

        public override int ReadFromBytes(byte[] bytes, int startIndex = 0)
        {
            int index = startIndex;
            ID = ReadInt(bytes, ref index);
            Data = ReadData<PlayerData>(bytes, ref index);
            return index - startIndex;
        }

        /// <summary>
        /// 消息ID
        /// </summary>
        /// <returns>ID是什么</returns>
        public override int GetID()
        {
            return 10086;
        }
    }
}
