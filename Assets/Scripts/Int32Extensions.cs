using System;
using System.Linq;

namespace MyLibrary.Extensions
{
    /// <summary>
    /// Int32 �g�����\�b�h�N���X
    /// </summary>
    public static class Int32Extensions
    {
        /// <summary>
        /// 2�i���\�L�̕�����ɕϊ����܂��B
        /// </summary>
        /// <param name="value">�ϊ��Ώۂ̒l</param>
        /// <returns>16�i���\�L�̕�����i�`���F11111111 ... 11111111�j</returns>
        public static string ToBin(this int value) => value.ToAnyBase(2, "11111111".Length);

        /// <summary>
        /// 16�i���\�L�̕�����ɕϊ����܂��B
        /// </summary>
        /// <param name="value">�ϊ��Ώۂ̒l</param>
        /// <returns>16�i���\�L�̕�����i�`���FFF FF FF FF�j</returns>
        public static string ToHex(this int value) => value.ToAnyBase(16, "FF".Length);

        /// <summary>
        /// 8�i���\�L�̕�����ɕϊ����܂��B
        /// </summary>
        /// <param name="value"></param>
        /// <returns>8�i���\�L�̕�����i�`���F037 777 777 777�j</returns>
        public static string ToOct(this int value) => value.ToAnyBase(8, "777".Length);

        /// <summary>
        /// 10�i���̒l���A4����؂�̊�\�L�̕�����ɕϊ����܂��B
        /// </summary>
        /// <param name="value">�ϊ��Ώۂ̒l</param>
        /// <param name="toBase">�ϊ��Ώۂ̊</param>
        /// <param name="length">�ϊ��Ώۂ̊�ɂ�����A1�o�C�g��\���\�Ȍ���(��: 16�i�� = "FF" = 2 )</param>
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
