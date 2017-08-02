using System;
using System.Text;
using System.Security.Cryptography;

namespace SEPAFileManager
{
    internal class Cryption
    {
        private static string m_PWKey = ",k,kj*E5";

        static internal string Decrypt(string Plaintext)
        {
            try
            {
                byte[] keyArray = System.Text.ASCIIEncoding.ASCII.GetBytes(m_PWKey);
                byte[] toEncryptArray = Convert.FromBase64String(Plaintext);

                DESCryptoServiceProvider tdes = new DESCryptoServiceProvider();
                tdes.Key = keyArray;
                tdes.Mode = CipherMode.ECB;
                tdes.Padding = PaddingMode.PKCS7;

                ICryptoTransform cTransform = tdes.CreateDecryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

                string test = UTF8Encoding.UTF8.GetString(resultArray);
                return test;
            }
            catch (Exception Ex)
            {
                throw new Exception("1437", Ex);
            }
        }
    }
}