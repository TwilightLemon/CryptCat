using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CryptCat
{
    internal class CharacterCodeCore
    {
        private readonly string[] _selector;
        public CharacterCodeCore()
        {
            _selector = new string[16];
            for (int i = 0; i < 16; i++)
            {
                _selector[i] = Convert.ToString(i, 2).PadLeft(4, '0');
            }
        }
        public byte[] GenerateCode(byte[] data,out byte[] code)
        {
            // 将byte[]转换为4位二进制组，并统计0000, 0001, ..., 1111的频率
            var frequencyMap = new int[16];

            foreach (var b in data)
            {
                frequencyMap[(b >> 4) & 0xF]++;
                frequencyMap[b & 0xF]++;
            }

            // 按照频率从高到低排序
            var sortedIndices = Enumerable.Range(0, 16).OrderByDescending(i => frequencyMap[i]).ToArray();

            // 替换比特流
            StringBuilder result = new StringBuilder();
            foreach (var b in data)
            {
                result.Append(_selector[sortedIndices[(b >> 4) & 0xF]]);
                result.Append(_selector[sortedIndices[b & 0xF]]);
            }
            var res = result.ToString();
            //Console.WriteLine("混淆后："+res);
            // 将01字符串转换为byte[]
            byte[] resultBytes = new byte[result.Length / 8];
            for (int i = 0; i < result.Length; i += 8)
            {
                resultBytes[i / 8] = Convert.ToByte(res.Substring(i, 8), 2);
            }
            code = CodeToBytes(sortedIndices);
            return resultBytes;
        }

        public byte[] CodeToBytes(int[] code)
        {
            //code仅为16个0~15的整数，每2个整数存进一个byte
            byte[] byteArray = new byte[8];
            for (int i = 0; i < code.Length; i+=2)
            {
                byteArray[i / 2] = (byte)((code[i] << 4) | code[i + 1]);
            }
            return byteArray;
        }

        public int[] ConvertCodeBytes(byte[] byteArray)
        {
            int[] code = new int[byteArray.Length * 2];
            for (int i = 0; i < byteArray.Length; i++)
            {
                code[i * 2] = (byteArray[i] >> 4) & 0xF;
                code[i * 2 + 1] = byteArray[i] & 0xF;
            }
            return code;
        }


        public byte[] SolveCode(byte[] data, byte[] code)
        {
            var sortedIndices=ConvertCodeBytes(code).ToList();
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < data.Length; i ++)
            {
                int index1 = (data[i] >> 4) & 0xF;
                int index2 = data[i] & 0xF;
                result.Append(_selector[sortedIndices.IndexOf(index1)]);
                result.Append(_selector[sortedIndices.IndexOf(index2)]);
            }
            var res = result.ToString();
            //Console.WriteLine("解混淆："+res);
            byte[] resultBytes = new byte[result.Length / 8];
            for (int i = 0; i < result.Length; i += 8)
            {
                resultBytes[i / 8] = Convert.ToByte(res.Substring(i, 8), 2);
            }
            return resultBytes;
        }
    }
}
