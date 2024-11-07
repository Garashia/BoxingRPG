using System;
using System.Linq;

namespace MyLibrary.Extensions
{
    /// <summary>
    /// Int32 拡張メソッドクラス
    /// </summary>
    public static class Int32Extensions
    {
        /// <summary>
        /// 2進数表記の文字列に変換します。
        /// </summary>
        /// <param name="value">変換対象の値</param>
        /// <returns>16進数表記の文字列（形式：11111111 ... 11111111）</returns>
        public static string ToBin(this int value) => value.ToAnyBase(2, "11111111".Length);

        /// <summary>
        /// 16進数表記の文字列に変換します。
        /// </summary>
        /// <param name="value">変換対象の値</param>
        /// <returns>16進数表記の文字列（形式：FF FF FF FF）</returns>
        public static string ToHex(this int value) => value.ToAnyBase(16, "FF".Length);

        /// <summary>
        /// 8進数表記の文字列に変換します。
        /// </summary>
        /// <param name="value"></param>
        /// <returns>8進数表記の文字列（形式：037 777 777 777）</returns>
        public static string ToOct(this int value) => value.ToAnyBase(8, "777".Length);

        /// <summary>
        /// 10進数の値を、4桁区切りの基数表記の文字列に変換します。
        /// </summary>
        /// <param name="value">変換対象の値</param>
        /// <param name="toBase">変換対象の基数</param>
        /// <param name="length">変換対象の基数における、1バイトを表現可能な桁数(例: 16進数 = "FF" = 2 )</param>
        /// <returns></returns>
        private static string ToAnyBase(this int value, int toBase, int length)
        {
            var strs = Convert.ToString(value, toBase).ToUpper()
                              .PadLeft(length * sizeof(int), '0')
                              .Select(ch => ch.ToString());

            return string.Join(string.Empty, strs.Select((s, i) =>
                                  (i + 1) % length == 0 ? s + " " : s)).TrimEnd();
        }
    }
}
