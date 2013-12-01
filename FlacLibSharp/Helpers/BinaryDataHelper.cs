using System;
using System.Collections.Generic;
using System.Text;

namespace FlacLibSharp.Helpers {
    /// <summary>
    /// The kinds of endianness that exist.
    /// </summary>
    public enum Endianness {
        /// <summary>
        /// Little endian.
        /// </summary>
        Little,
        /// <summary>
        /// Big endian.
        /// </summary>
        Big
    }

    /// <summary>
    /// A helper class for parsing byes and bits to actual numbers!
    /// </summary>
    public static class BinaryDataHelper {

        /// <summary>
        /// From a given data array, get a subset of items in a deep copied array.
        /// </summary>
        /// <param name="data">The input data (won't be altered)</param>
        /// <param name="offset">Where in the input data to start copying files.</param>
        /// <param name="length">The amount of bytes to copy.</param>
        /// <returns>A new array with a copy of the subset of data.</returns>
        public static byte[] GetDataSubset(byte[] data, int offset, int length) {
            byte[] newData = new byte[length];
            Array.Copy(data, offset, newData, 0, length);
            return newData;
        }

        /// <summary>
        /// From the data, reads an unsigned 16 bit integer starting from the offset.
        /// </summary>
        /// <param name="data">The source data</param>
        /// <param name="byteOffset">Offset from where to start reading the integer, in bytes.</param>
        /// <returns>The number that was read.</returns>
        public static UInt16 GetUInt16(byte[] data, int byteOffset) {
            return (UInt16)GetUInt(data, byteOffset, 16);
        }

        /// <summary>
        /// From the data, reads an unsigned 24 bit integer starting from the offset.
        /// </summary>
        /// <param name="data">The source data</param>
        /// <param name="byteOffset">Offset from where to start reading the integer, in bytes.</param>
        /// <returns>The number that was read (it reads 24 bits, but the actual type will be a 32 bit integer).</returns>
        public static UInt32 GetUInt24(byte[] data, int byteOffset) {
            return (UInt32)GetUInt(data, byteOffset, 24);
        }

        /// <summary>
        /// From the data, reads an unsigned 32 bit integer starting from the offset.
        /// </summary>
        /// <param name="data">The source data</param>
        /// <param name="byteOffset">Offset from where to start reading the integer, in bytes.</param>
        /// <returns>The number that was read.</returns>
        public static UInt32 GetUInt32(byte[] data, int byteOffset) {
            return (UInt32)GetUInt(data, byteOffset, 32);
        }

        /// <summary>
        /// From the data, reads an unsigned 64 bit integer starting from the offset.
        /// </summary>
        /// <param name="data">The source data</param>
        /// <param name="byteOffset">Offset from where to start reading the integer, in bytes.</param>
        /// <returns>The number that was read.</returns>
        public static UInt64 GetUInt64(byte[] data, int byteOffset) {
            return (UInt64)GetUInt(data, byteOffset, 64);
        }

        /// <summary>
        /// From the data, reads boolean at the offset.
        /// </summary>
        /// <param name="data">The source data</param>
        /// <param name="byteOffset">Offset from where to start reading the boolean, in bytes.</param>
        /// <param name="bitOffset">In the found byte, defines the bit that will represent the boolean.</param>
        /// <returns>The number that was read.</returns>
        public static bool GetBoolean(byte[] data, int byteOffset, int bitOffset) {
            return (GetUInt(data, byteOffset, 1, (byte)bitOffset) == 1);
        }

        /// <summary>
        /// From the data, reads an integer value starting from the offset.
        /// </summary>
        /// <param name="data">The source data</param>
        /// <param name="byteOffset">Offset from where to start reading the integer, in bytes.</param>
        /// <param name="bitCount">How many bits to read (16, 32, or something arbitrary but less than or equal to 64)</param>
        /// <returns>The number that was read.</returns>
        public static UInt64 GetUInt(byte[] data, int byteOffset, int bitCount) {
            return GetUInt(data, byteOffset, bitCount, 0);
        }

        /// <summary>
        /// Allows you to interpret part of a byte array as a number.
        /// </summary>
        /// <param name="data">The source data.</param>
        /// <param name="byteOffset">Where in the data to start reading (offset in bytes)</param>
        /// <param name="bitCount">How many bits to read (16, 32, or something arbitrary but less than or equal to 64)</param>
        /// <param name="bitOffset">In the first byte, at which bit to start reading the data from.</param>
        /// <returns></returns>
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

        /// <summary>
        /// For a given array of bytes, switch the endiannes of the length-bytes starting at byteOffset.
        /// </summary>
        /// <param name="data">The source data.</param>
        /// <param name="byteOffset">Where to start switching the endianness.</param>
        /// <param name="length">How many bytes to switch.</param>
        /// <returns></returns>
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
}
