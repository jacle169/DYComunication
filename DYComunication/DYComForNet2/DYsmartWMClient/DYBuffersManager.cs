using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;

namespace DYsmartWMClient
{
    /// <summary>
    /// 数据包格式化类
    /// </summary>
    internal static class DYWriter
    {
        /// <summary>
        /// 将1个2维数据包整合成以个一维数据包
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        static public Byte[] Merge(params Byte[][] args)
        {
            Int32 length = 0;
            foreach (byte[] tempbyte in args)
            {
                length += tempbyte.Length;  //计算数据包总长度
            }
            Byte[] bytes = new Byte[length]; //建立新的数据包
            Int32 tempLength = 0;
            foreach (byte[] tempByte in args)
            {
                tempByte.CopyTo(bytes, tempLength);
                tempLength += tempByte.Length;  //复制数据包到新数据包
            }
            return bytes;
        }

        /// <summary>
        /// 将一个32位整形转换成一个BYTE[]4字节
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        static public Byte[] GetDYBytes(Int32 data)
        {
            return BitConverter.GetBytes(data);
        }

        /// <summary>
        /// 将一个浮点型转换成一个BYTE[]4字节
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        static public byte[] GetDYBytes(float data)
        {
            return BitConverter.GetBytes(data);
        }

        /// <summary>
        /// 将一个16位整形转换成一个BYTE[]2字节
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        static public Byte[] GetDYBytes(Int16 data)
        {
            return BitConverter.GetBytes(data);
        }

        /// <summary>
        /// 将一个Double值转换成一个BYTE[]4字节
        /// </summary>
        static public byte[] GetDYBytes(double data)
        {
            return BitConverter.GetBytes(data);
        }

        /// <summary>
        /// 将一个DateTime值转换成一个BYTE[]8字节
        /// </summary>
        static public byte[] GetDYBytes(DateTime data)
        {
            return BitConverter.GetBytes(data.Ticks);
        }

        /// <summary>
        /// 将一个Bool值转换成一个BYTE(True:1,False:0)
        /// </summary>
        static public Byte[] GetDYBytes(bool data)
        {
            return BitConverter.GetBytes(data);
        }

        /// <summary>
        /// 将一个64位整型转换成以个BYTE[] 8字节
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        static public Byte[] GetDYBytes(Int64 data)
        {
            return BitConverter.GetBytes(data);
        }

        /// <summary>
        /// 将一个 1位CHAR转换成1位的BYTE
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        static public Byte[] GetDYBytes(byte data)
        {
            return new Byte[] { (Byte)data };
        }

        /// <summary>
        /// 将一个BYTE[]数据包添加首位长度
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        static public Byte[] GetDYBytes(Byte[] data)
        {
            return Merge(
                GetDYBytes(data.Length),
                data
                );
        }

        /// <summary>
        /// 将一个字符串转换成BYTE[]，BYTE[]的首位是字符串的长度
        /// </summary>
        /// <param name="data"></param>
        /// <param name="encoder"></param>
        /// <returns></returns>
        static public Byte[] GetDYBytes(String data, Encoding encoder)
        {
            Byte[] bytes = encoder.GetBytes(data);

            return Merge(
                GetDYBytes(bytes.Length),
                bytes
                );
        }
    }

    /// <summary>
    /// 数据包格式化类
    /// </summary>
    internal class DYReader
    {

        private int Current;

        private byte[] Data;

        /// <summary>
        /// 内存流长度
        /// </summary>
        public int Length { get; set; }


        /// <summary>
        /// 重置内存流指针
        /// </summary>
        public void Reset()
        {
            Current = 0;
        }

        /// <summary>
        /// DYCom消息读取器
        /// </summary>
        /// <remarks>读取消息中的内容</remarks>
        public DYReader(Byte[] data)
        {
            if (data != null)
            {
                Data = data;
                this.Length = Data.Length;
                Current = 0;
            }
        }

        /// <summary>
        /// 读取内存流中的4位并转换成整型
        /// </summary>
        /// <param name="values">内存流</param>
        /// <returns></returns>
        public bool ReadInt32(out int values)
        {
            try
            {
                values = BitConverter.ToInt32(Data, Current);
                for (int i = 0; i < 4; i++)
                {
                    Current = Interlocked.Increment(ref Current);
                }
                return true;
            }
            catch
            {
                values = 0;                
                return false;
            }           
        }

        /// <summary>
        /// 读取内存流中的4位并转换浮点型
        /// </summary>
        /// <param name="values">内存流</param>
        /// <returns></returns>
        public bool ReadFloat(out float values)
        {
            try
            {
                values = BitConverter.ToSingle(Data, Current);
                for (int i = 0; i < 4; i++)
                {
                    Current = Interlocked.Increment(ref Current);
                }
                return true;
            }
            catch
            {
                values = 0;
                return false;
            }
        }

        /// <summary>
        /// 读取内存流中的Double值
        /// </summary>
        public bool ReadDouble(out double values)
        {
            try
            {
                values = BitConverter.ToDouble(Data, Current);
                for (int i = 0; i < 8; i++)
                {
                    Current = Interlocked.Increment(ref Current);
                }
                return true;
            }
            catch
            {
                values = 0;
                return false;
            }     
        }

        /// <summary>
        /// 读取内存流中的Long值
        /// </summary>
        public bool ReadInt64(out long values)
        {
            try
            {
                values = BitConverter.ToInt64(Data, Current);
                for (int i = 0; i < 8; i++)
                {
                    Current = Interlocked.Increment(ref Current);
                }
                return true;
            }
            catch
            {
                values = 0;
                return false;
            }
        }

        /// <summary>
        /// 读取内存流中的Bool值
        /// </summary>
        public bool ReadBool(out bool values)
        {
            try
            {
                values = BitConverter.ToBoolean(Data, Current);
                Current = Interlocked.Increment(ref Current);
                return true;
            }
            catch
            {
                values = false;
                return false;
            }  
        }

        /// <summary>
        /// 读取内存流中的DataTime值
        /// </summary>
        public bool ReadDateTime(out DateTime values)
        {
            try
            {
                long ticks = BitConverter.ToInt64(Data, Current);
                values = new DateTime(ticks);
                for (int i = 0; i < 8; i++)
                {
                    Current = Interlocked.Increment(ref Current);
                }
                return true;
            }
            catch
            {
                values = DateTime.MinValue;
                return false;
            }
        }

        /// <summary>
        /// 读取内存流中的首位
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public  bool ReadByte(out byte values)
        {
            try
            {
                values = (byte)Data[Current];
                Current = Interlocked.Increment(ref Current);
                return true;
            }
            catch
            {
                values = 0;
                return false;
            }           
        }


        /// <summary>
        /// 读取内存流中的2位并转换成整型
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public bool ReadInt16(out short values)
        {
            try
            {
                values = BitConverter.ToInt16(Data, Current);
                for (int i = 0; i < 2; i++)
                {
                    Current = Interlocked.Increment(ref Current);
                }
                return true;
            }
            catch
            {
                values = 0;
                return false;
            }     
        }


        /// <summary>
        /// 读取内存流中一段字符串
        /// </summary>
        /// <param name="values">读出变量</param>
        /// <param name="encoder">解码器</param>
        /// <returns></returns>
        public bool ReadString(out string values, Encoding encoder)
        {
            int lengt;
            try
            {
                if (ReadInt32(out lengt))
                {
                    Byte[] buf = new Byte[lengt];
                    Buffer.BlockCopy(Data, Current, buf, 0, buf.Length);
                    values = encoder.GetString(buf, 0, buf.Length);
                    for (int i = 0; i < lengt; i++)
                    {
                        Current = Interlocked.Increment(ref Current);       
                    }
                    return true;
                }
                else
                {
                    values = "";
                    return false;
                }
            }
            catch
            {
                values ="";
                return false;
            }     
        }


        /// <summary>
        /// 读取内存流中一段数据
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public bool ReadByteArray(out byte[] values)
        {
            int lengt;
            try
            {
                if (ReadInt32(out lengt))
                {
                    values = new Byte[lengt];
                    Buffer.BlockCopy(Data, Current, values, 0, values.Length);
                    for (int i = 0; i < lengt; i++)
                    {
                        Current = Interlocked.Increment(ref Current);      
                    }
                    return true;

                }
                else
                {
                    values = null;
                    return false;
                }
            }
            catch
            {
                values = null;
                return false;
            }

        }

        /// <summary>
        /// 读取内存流中剩余部份
        /// </summary>
        public byte[] GetCurrentToEnd()
        {
            byte[] m = new byte[Data.Length - Current];
            Buffer.BlockCopy(Data, Current, m, 0, Data.Length - Current);
            return m;
        }


    }




}
