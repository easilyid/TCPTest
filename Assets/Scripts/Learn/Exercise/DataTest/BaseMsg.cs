﻿namespace Learn.Exercise
{
    /// <summary>
    /// 
    /// </summary>
    public class BaseMsg : BaseGetData
    {
        public override int GetBytesNum()
        {
            throw new System.NotImplementedException();
        }

        public override byte[] Writing()
        {
            throw new System.NotImplementedException();
        }

        public override int Reading(byte[] bytes, int beginIndex = 0)
        {
            throw new System.NotImplementedException();
        }

        public virtual int GetID()
        {
            return 0;
        }
    }
}