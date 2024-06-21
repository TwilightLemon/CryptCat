using System.Diagnostics;
using System.Text;

namespace CryptCat
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Stopwatch sw = new();
            sw.Start();
            List<double> rates = new();
            int count = 200;
            while (--count!=0)
            {
                string key = "nihaoTWLM";
                string data = RandomText(800*count);
                #region 加密解密过程
                byte[] keyBytes = StrCryptCore.GetHash128(key);
                byte[] dataBytes = Encoding.ASCII.GetBytes(data);
                byte[] encryptedData = StrCryptCore.Encrypt(dataBytes, keyBytes);

                var ctc = new CharacterCodeCore();
                var co = ctc.GenerateCode(encryptedData, out byte[] code);

                string resultCode = Convert.ToBase64String(code);
             //   Console.Out.WriteLineAsync("特征码：" + resultCode);
                string resultData = Convert.ToBase64String(co);
             //   Console.Out.WriteLineAsync("数据：" + resultData);

                var solved = ctc.SolveCode(Convert.FromBase64String(resultData), Convert.FromBase64String(resultCode));
                byte[] decryptedData = StrCryptCore.Decrypt(solved, keyBytes);
               // Console.Out.WriteLineAsync(Encoding.ASCII.GetString(decryptedData));
                #endregion
                double v = (double)resultData.Length / data.Length;
                rates.Add(v);
               // Console.Out.WriteLineAsync($"体积变化：{v * 100}%");
            }
            sw.Stop();
            Console.Out.WriteLineAsync($"总耗时：{sw.ElapsedMilliseconds}ms");
            Console.Out.WriteLineAsync($"平均体积变化：{rates.Average() * 100}%");
        }

        static string RandomText(int length)
        {
            var random = new Random();
            var characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringBuilder = new StringBuilder(length);

            for (int i = 0; i < length; i++)
            {
                var index = random.Next(characters.Length);
                stringBuilder.Append(characters[index]);
            }

            return stringBuilder.ToString();
        }
    }
}
