using System;
using System.Collections.Generic;
using System.Text;

namespace FlacLibSharp.Helpers {
    public enum Endianness {
        Little,
        Big
    }

    public static class BinaryDataHelper {
        // 0     1     2
        // BYTEA-BYTEB-BYTEC

        public static byte[] GetDataSubset(byte[] data, int offset, int length) {
            byte[] newData = new byte[length];
            Array.Copy(data, offset, newData, 0, length);
            return newData;
        }

        public static UInt16 GetUInt16(byte[] data, int byteOffset) {
            return (UInt16)GetUInt(data, byteOffset, 16);
        }

        public static UInt32 GetUInt24(byte[] data, int byteOffset) {
            return (UInt32)GetUInt(data, byteOffset, 24);
        }

        public static UInt32 GetUInt32(byte[] data, int byteOffset) {
            return (UInt32)GetUInt(data, byteOffset, 32);
        }

        public static UInt64 GetUInt64(byte[] data, int byteOffset) {
            return (UInt64)GetUInt(data, byteOffset, 64);
        }

        public static bool GetBoolean(byte[] data, int byteOffset, int bitOffset) {
            return (GetUInt(data, byteOffset, 1, (byte)bitOffset) == 1);
        }

        public static UInt64 GetUInt(byte[] data, int byteOffset, int bitCount) {
            return GetUInt(data, byteOffset, bitCount, 0);
        }

        public static UInt64 GetUInt(byte[] data, int byteOffset, int bitCount, byte bitOffset) {
            // TODO: Allow to span multiple bytes when offset and uneven bitcount is used... (like offset = 7, bitcount = 5)
            UInt64 result = 0;
            byte byteCount;
            byte temp_value;
            byte maskedBitCount;

            byteCount = (byte)(Math.Ceiling((bitCount+bitOffset) / 8.0));

            maskedBitCount = (byte)((byteCount * 8) - bitCount - bitOffset);

            for (int i = 0; i < byteCount; i++) {
                temp_value = data[byteOffset + i];
                if (i == 0) {
                    temp_value = (byte)(temp_value << bitOffset);
                    temp_value = (byte)(temp_value >> bitOffset);
                }

                result = result + temp_value;
                if (i == (byteCount - 1)) {
                    result = result >> maskedBitCount;
                } else if (bitCount < 8) {
                    result = result << (8 - maskedBitCount);
                } else {
                    result = result << 8;
                }
            }

            return result;
        }

        public static byte[] SwitchEndianness(byte[] data, int byteOffset, int length)
        {
            byte[] result = new byte[length];

            for (int i = 0; i < length; i++)
            {
                result[length - (i + 1)] = data[i + byteOffset];
            }

            return result;
        }

    }

    public enum BitShiftDirection {
        Left,
        Right
    }
}
