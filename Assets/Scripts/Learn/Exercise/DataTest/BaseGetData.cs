using System;
using System.Text;

namespace Learn.Exercise
{
    public abstract class BaseGetData
    {
        /// <summary>
        /// 计算字节数组容器的大小
        /// </summary>
        /// <returns></returns>
        public abstract int GetBytesNum();

        /// <summary>
        /// 把成员变量序列化为 对应的字节数组
        /// </summary>
        /// <returns></returns>
        public abstract byte[] Writing();

        /// <summary>
        /// 把二进制数据反序列化到成员变量
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="index"></param>
        public abstract int Reading(byte[] bytes, int beginIndex = 0);

        protected void WriteInt(byte[] bytes, int value, ref int index)
        {
            BitConverter.GetBytes(value).CopyTo(bytes, index);
            index += sizeof(int); //int 4个字节 写ref就是为了改变index的值
        }

        protected void WriteString(byte[] bytes, string value, ref int index)
        {
            var bytes1 = Encoding.UTF8.GetBytes(value);
            int num = bytes1.Length;
            BitConverter.GetBytes(num).CopyTo(bytes, index);
            index += sizeof(int); //int 4个字节 写ref就是为了改变index的值
            bytes1.CopyTo(bytes, index);
            index += bytes1.Length;
        }

        protected void WriteShort(byte[] bytes, short value, ref int index)
        {
            BitConverter.GetBytes(value).CopyTo(bytes, index);
            index += sizeof(short);
        }

        protected void WirteFloat(byte[] bytes, float value, ref int index)
        {
            BitConverter.GetBytes(value).CopyTo(bytes, index);
            index += sizeof(float);
        }

        protected void WriteBool(byte[] bytes, bool value, ref int index)
        {
            BitConverter.GetBytes(value).CopyTo(bytes, index);
            index += sizeof(bool);
        }

        protected void WriteData(byte[] bytes, BaseGetData value, ref int index)
        {
            value.Writing().CopyTo(bytes, index);
            index += value.GetBytesNum();
        }

        /// <summary>
        /// 根据字节数组 读取整形
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <param name="index">开始读取的索引数</param>
        /// <returns></returns>
        protected int ReadInt(byte[] bytes, ref int index)
        {
            int value = BitConverter.ToInt32(bytes, index);
            index += sizeof(int);
            return value;
        }
        protected short ReadShort(byte[] bytes, ref int index)
        {
            short value = BitConverter.ToInt16(bytes, index);
            index += sizeof(short);
            return value;
        }
        protected long ReadLong(byte[] bytes, ref int index)
        {
            long value = BitConverter.ToInt64(bytes, index);
            index += sizeof(long);
            return value;
        }
        protected float ReadFloat(byte[] bytes, ref int index)
        {
            float value = BitConverter.ToSingle(bytes, index);
            index += sizeof(float);
            return value;
        }
        protected byte ReadByte(byte[] bytes, ref int index)
        {
            byte value = bytes[index];
            index += sizeof(byte);
            return value;
        }
        protected bool ReadBool(byte[] bytes, ref int index)
        {
            bool value = BitConverter.ToBoolean(bytes, index);
            index += sizeof(bool);
            return value;
        }
        protected string ReadString(byte[] bytes, ref int index)
        {
            //首先读取长度
            int length = ReadInt(bytes, ref index);
            //再读取string
            string value = Encoding.UTF8.GetString(bytes, index, length);
            index += length;
            return value;
        }
        protected T ReadData<T>(byte[] bytes, ref int index) where T:BaseGetData,new()
        {
            T value = new T();
            index += value.Reading(bytes, index);
            return value;
        }
    }
}