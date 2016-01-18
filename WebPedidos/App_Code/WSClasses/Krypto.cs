using System;
using System.Text;

namespace WebPedidos.WSClasses
{
    public class Krypto
    {
        const String sKey = "!@#$%&*()+{}?><[]";

        AcaoKrypto ENCRYPT { get; set; }
        AcaoKrypto DECRYPT { get; set; }

        public String EncryptString(String Text, AcaoKrypto Action)
        {
            String rtn = "";

            String UserKey = sKey;
            int n;
            int j = -1;
            int Temp;

            n = UserKey.Length;

            int[] UserKeyAscIIs = new int[n];
            for (int i = 0; i < n; i++)
            {
                UserKeyAscIIs[i] = Convert.ToInt32((char)UserKey[i]);
            }

            int[] TEXTAscIIs = new int[Text.Length];
            for (int i = 0; i < Text.Length; i++)
            {
                char[] chrBuffer = { Convert.ToChar(Text[i]) };
                byte[] bytBuffer = Encoding.GetEncoding(1252).GetBytes(chrBuffer);
                TEXTAscIIs[i] = (int)bytBuffer[0];
            }

            if (Action == AcaoKrypto.cnENCRYPT)
            {
                for (int i = 0; i < Text.Length; i++)
                {
                    j = (j + 1 >= n) ? 0 : j + 1;
                    Temp = TEXTAscIIs[i] + UserKeyAscIIs[j];
                    if (Temp > 255)
                    {
                        Temp -= 255;
                    }
                    rtn += Encoding.GetEncoding(1252).GetString(new byte[] { (byte)Temp });
                }
            }
            else
            {
                for (int i = 0; i < Text.Length; i++)
                {
                    j = j + 1 >= n ? 0 : j + 1;
                    Temp = TEXTAscIIs[i] - UserKeyAscIIs[j];
                    if (Temp < 0)
                    {
                        Temp += 255;
                    }
                    byte[] bytBuffer = new byte[] { (byte)Temp };
                    rtn += Encoding.GetEncoding(1252).GetString(bytBuffer);
                }
            }

            return rtn;
        }
    }
    public enum AcaoKrypto
    {
        cnENCRYPT = 1,
        cnDECRYPT = 2
    }
}
