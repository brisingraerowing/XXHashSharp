using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using System.Linq;

namespace XXHashSharp
{
    public sealed class XXHash32 : HashAlgorithm
    {

        List<byte> bytes = new List<byte>();

        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            byte[] subArray = new byte[cbSize - ibStart];

            Array.Copy(array, ibStart, subArray, 0, subArray.Length);

            bytes.AddRange(subArray);
        }

        protected override byte[] HashFinal()
        {
            UInt32 res;
            UInt32 seed;
            byte[] sb = new byte[sizeof(UInt32)];

            new Random().NextBytes(sb);

            seed = BitConverter.ToUInt32(sb, 0);

            unsafe
            {

                byte[] b = bytes.ToArray();
                
                fixed(byte* p = b)
                {
                    res = XXHashNative.XXH32(p, new UIntPtr((uint)b.Length), seed);
                }

            }

            return BitConverter.GetBytes(res);
        }

        public override void Initialize()
        {
        }
    }

    public sealed class XXHash64 : HashAlgorithm
    {
        List<byte> bytes = new List<byte>();

        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            byte[] subArray = new byte[cbSize - ibStart];

            Array.Copy(array, ibStart, subArray, 0, subArray.Length);

            bytes.AddRange(subArray);
        }

        protected override byte[] HashFinal()
        {
            UInt64 res;
            UInt64 seed;
            byte[] sb = new byte[sizeof(UInt64)];

            new Random().NextBytes(sb);

            seed = BitConverter.ToUInt64(sb, 0);

            unsafe
            {

                byte[] b = bytes.ToArray();

                fixed (byte* p = b)
                {
                    res = XXHashNative.XXH64(p, new UIntPtr((uint)b.Length), seed);
                }

            }

            return BitConverter.GetBytes(res);
        }

        public override void Initialize()
        {
        }
    }

    internal static class XXHashNative
    {
        private const string XXHASH_LIB = "xxhash.dll";

        [DllImport(XXHASH_LIB)]
        internal static unsafe extern UInt32 XXH32(void* input, UIntPtr length, UInt32 seed);

        [DllImport(XXHASH_LIB)]
        internal static unsafe extern UInt64 XXH64(void* input, UIntPtr length, UInt64 seed);

    }
}
