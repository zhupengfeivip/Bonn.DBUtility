using System;

namespace Bonn.Helper
{
    /// <summary>
    /// KeeLoq加密算法
    /// </summary>
    public class KeeLoq
    {
        /// <summary>
        /// 固定公共密钥
        /// </summary>
        private const UInt64 KeeLoq_NLF = 0x3A5C742E;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        private static UInt64 bit(UInt64 x, int n)
        {
            return (x >> n) & 1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        private static UInt64 g5(UInt64 x, byte a, byte b, byte c, byte d, byte e)
        {
            return bit(x, a) + bit(x, b) * 2 + bit(x, c) * 4 + bit(x, d) * 8 + bit(x, e) * 16;
        }

        /// <summary>
        /// 加密算法
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static byte[] KeeLoq_Encrypt(byte[] data, byte[] key)
        {
            Array.Reverse(data);
            Array.Reverse(key);
            UInt32 userData = BitConverter.ToUInt32(data, 0);
            UInt32 uKey = BitConverter.ToUInt32(key, 0);
            UInt64 uResult = KeeLoq_Encrypt(userData, uKey);
            byte[] resultBytes = BitConverter.GetBytes(uResult);
            byte[] outBytes = new byte[4];
            Buffer.BlockCopy(resultBytes, 0, outBytes, 0, 4);
            Array.Reverse(outBytes);
            return outBytes;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static UInt64 KeeLoq_Encrypt(UInt32 data, UInt32 key)
        {
            UInt64 x = data;
            int r;

            for (r = 0; r < 528; r++)
            {
                x = (x >> 1) ^ ((bit(x, 0) ^ bit(x, 16) ^ bit(key, r & 63) ^ bit(KeeLoq_NLF, (int)(g5(x, 1, 9, 20, 26, 31)))) << 31);
            }
            return x;
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="data"></param>
        ///// <param name="key"></param>
        ///// <returns></returns>
        //public UInt64 KeeLoq_Decrypt(UInt32 data, UInt32 key)
        //{
        //    UInt64 x = data;
        //    int r;

        //    for (r = 0; r < 528; r++)
        //    {
        //        x = (x << 1) ^ bit(x, 31) ^ bit(x, 15) ^ bit(key, (15 - r) & 63) ^ bit(KeeLoq_NLF, (int)g5(x, 0, 8, 19, 25, 30));
        //    }
        //    return x;
        //}
    }
}
