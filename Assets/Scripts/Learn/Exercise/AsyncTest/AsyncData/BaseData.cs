using System;
using System.Text;

namespace Learn.Exercise.AsyncTest.AsyncData
{
    public abstract class BaseData
    {
        /// <summary>
        /// 获取数据的字节数
        /// </summary>
        /// <returns></returns>
        public abstract int GetByteNum();

        /// <summary>
        /// 将数据写入字节数组
        /// </summary>
        /// <returns></returns>
        public abstract byte[] WriteToBytes();

        /// <summary>
        /// 将字节数组转换为数据
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="startIndex">开始索引</param>
        /// <returns></returns>
        public abstract int ReadFromBytes(byte[] bytes, int startIndex = 0);

        /// <summary>
        /// 将int写入字节数组
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="value"></param>
        /// <param name="index"></param>
        protected void WriteInt(byte[] bytes, int value, ref int index)
        {
            BitConverter.GetBytes(value).CopyTo(bytes, index);
            index += sizeof(int);
        }

        protected void WriteShort(byte[] bytes, short value, ref int index)
        {
            BitConverter.GetBytes(value).CopyTo(bytes, index);
            index += sizeof(short);
        }

        protected void WriteLong(byte[] bytes, long value, ref int index)
        {
            BitConverter.GetBytes(value).CopyTo(bytes, index);
            index += sizeof(long);
        }

        protected void WriteFloat(byte[] bytes, float value, ref int index)
        {
            BitConverter.GetBytes(value).CopyTo(bytes, index);
            index += sizeof(float);
        }

        protected void WriteByte(byte[] bytes, byte value, ref int index)
        {
            bytes[index] = value;
            index += sizeof(byte);
        }
        
        protected void WriteBool(byte[] bytes, bool value, ref int index)
        {
            BitConverter.GetBytes(value).CopyTo(bytes, index);
            index += sizeof(bool);
        }

       
        protected void WriteString(byte[] bytes, string value, ref int index)
        {
            byte[] bytes1 = Encoding.UTF8.GetBytes(value);
            WriteInt(bytes, bytes1.Length, ref index);
            bytes1.CopyTo(bytes, index);
            index += bytes1.Length;
        }

       

        protected void WriteData(byte[] bytes, BaseData value, ref int index)
        {
            value.WriteToBytes().CopyTo(bytes, index);
            index += value.GetByteNum();
        }

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

        protected bool ReadBool(byte[] bytes, ref int index)
        {
            bool value = BitConverter.ToBoolean(bytes, index);
            index += sizeof(bool);
            return value;
        }

        protected string ReadString(byte[] bytes, ref int index)
        {
            //首先读取长度
            int count = ReadInt(bytes, ref index);
            //再读取string
            string value = Encoding.UTF8.GetString(bytes, index, count);
            index += count;
            return value;
        }

        protected byte ReadByte(byte[] bytes, ref int index)
        {
            byte value = bytes[index];
            index += sizeof(byte);
            return value;
        }

        protected T ReadData<T>(byte[] bytes, ref int index) where T : BaseData, new()
        {
            T value = new T();
            index += value.ReadFromBytes(bytes, index);
            return value;
        }
    }
}