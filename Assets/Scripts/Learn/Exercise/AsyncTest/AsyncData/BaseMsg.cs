using System;

namespace Learn.Exercise.AsyncTest.AsyncData
{
    public class BaseMsg : BaseData
    {
        public override int GetByteNum()
        {
            throw new NotImplementedException();
        }

        public override byte[] WriteToBytes()
        {
            throw new NotImplementedException();
        }

        public override int ReadFromBytes(byte[] bytes, int startIndex = 0)
        {
            throw new NotImplementedException();
        }

        public virtual int GetID()
        {
            return 0;
        }
    }
}
