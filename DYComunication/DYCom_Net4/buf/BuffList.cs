using System;
using System.Collections.Generic;

namespace DYCom_Net4
{
    internal class BuffList
    {
        internal List<byte> ByteList { get; set; }

        internal int Current { get; set; }

        internal BuffList(int BufferCount)
        {
            ByteList = new List<byte>(BufferCount);
        }

        internal void Reset()
        {
            Current = 0;
        }

        internal void InsertByteArray(byte[] Data)
        {
            ByteList.AddRange(Data);
        }

        byte[] dt;
        internal byte[] GetData()
        {
            try
            {
                dt = ByteList.ToArray();
                if (dt.Length >= 4)
                {
                    int len = BitConverter.ToInt32(dt, 0);
                    //if (len < 0 || len >ByteList.Count)
                    //{
                    //    Reset();
                    //    ByteList.Clear();
                    //    return null;
                    //}

                    if (len == ByteList.Count)
                    {
                        Reset();
                        ByteList.Clear();
                        return dt;
                    }
                    else
                    {
                        return null;
                    }
                }
                else { return null; }
            }
            catch
            {
                Reset();
                ByteList.Clear();
                return null;
            }
        }

    }
}
