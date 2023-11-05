namespace Learn.Exercise
{
    public class PlayerMsg : BaseMsg
    {
        public int ID;
        public PlayerData data;

        public override byte[] Writing()
        {
            int index = 0;
            int bytesNum = GetBytesNum();
            byte[] bytes = new byte[bytesNum];
            WriteInt(bytes, GetID(), ref index); //这个是标识ID
            WriteInt(bytes, bytesNum - 8, ref index); //这个是消息体的长度
            WriteInt(bytes, ID, ref index);
            WriteData(bytes, data, ref index);
            return bytes;
        }

        public override int Reading(byte[] bytes, int beginIndex = 0)
        {
            int index = beginIndex;
            ID = ReadInt(bytes, ref index);
            data = ReadData<PlayerData>(bytes, ref index);
            return index - beginIndex;
        }

        public override int GetBytesNum()
        {
            return 4 + //消息ID的长度
                   4 + //data 字节数组的长度
                   4+ //ID的字节数组长度
                   data.GetBytesNum();
        }

        /// <summary>
        /// 自定义ID 用于区分不同的消息
        /// </summary>
        /// <returns></returns>
        public override int GetID()
        {
            return 1001; //这个标识ID 可以随便修改的 表示这个ID之后的数据都是这个类的
        }
    }
}