using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json.Linq;

namespace ABVaccineQR
{
    public class QRManager
    {
        private static readonly string QR_FILENAME = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "qr.dat");

        private class QRFileHeader
        {
            private static readonly char[] HEADER_VALUE = { (char)0x11, (char)0x08, (char)0x21, (char)0x02 };

            public char[] Header { get; private set; } = new char[4];

            public static void Write(StreamWriter writer)
            {
                for(int  i = 0; i < HEADER_VALUE.Length; ++i)
                {
                    writer.BaseStream.WriteByte(Convert.ToByte(HEADER_VALUE[i]));
                }
            }

            public bool Verify()
            {
                for(int i = 0; i < HEADER_VALUE.Length; ++i)
                {
                    if(Header[i] != HEADER_VALUE[i])
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        private static readonly char[] DATA_HEADER_VALUE = { (char)0x11, (char)0x12, (char)0x13, (char)0x14 };
        private static readonly byte[] SALT = { 0x2c, 0x00, 0x9c, 0xba, 0x2d, 0xe5, 0x44, 0x70, 0x89, 0xfe, 0x90, 0xa3, 0xb1, 0xcc, 0x32, 0xca };

        public enum ELoadResult
        {
            Success,
            FileDoesNotExist,
            InvalidFile,
            InvalidPassword,
            NeedPassword,
            ReadError
        }

        public bool Save()
        {
            if(QR == null)
            {
                return false;
            }

            using (var file_writer = new StreamWriter(new FileStream(QR_FILENAME, FileMode.Create), Encoding.ASCII))
            {
                QRFileHeader.Write(file_writer);

                if(m_password != null)
                {
                    file_writer.BaseStream.WriteByte(Convert.ToByte(0x01));
                }
                else
                {
                    file_writer.BaseStream.WriteByte(Convert.ToByte(0x00));
                }

                using(var ms = new MemoryStream())
                {
                    using(var sw = new StreamWriter(ms, Encoding.ASCII))
                    {
                        for(int i = 0; i < DATA_HEADER_VALUE.Length; ++i)
                        {
                            sw.BaseStream.WriteByte(Convert.ToByte(DATA_HEADER_VALUE[i]));
                        }

                        sw.WriteLine("{");
                        sw.WriteLine("\"QR\": \"" + QR + "\"");
                        sw.WriteLine("}");
                    }

                    if(m_password != null)
                    {
                        using(var aes = Aes.Create())
                        {
                            aes.Key = m_password;

                            // write the IV
                            file_writer.BaseStream.Write(aes.IV, 0, aes.IV.Length);

                            // encrypt the data
                            byte[] encrypted_data = null;
                            try
                            {
                                encrypted_data = EncryptData(Encoding.ASCII.GetString(ms.ToArray()), aes.Key, aes.IV);
                            }
                            catch(Exception)
                            {
                                return false;
                            }

                            if(encrypted_data == null)
                            {
                                return false;
                            }
                            file_writer.BaseStream.Write(encrypted_data, 0, encrypted_data.Length);
                        }
                    }
                    else
                    {
                        var data_array = ms.ToArray();
                        file_writer.BaseStream.Write(data_array, 0, data_array.Length);
                    }
                }
            }

            return true;
        }

        public ELoadResult Load()
        {
            if(!File.Exists(QR_FILENAME))
            {
                return ELoadResult.FileDoesNotExist;
            }

            using(var reader = new StreamReader(new FileStream(QR_FILENAME, FileMode.Open), Encoding.ASCII))
            {
                var header = new QRFileHeader();
                header.Header[0] = Convert.ToChar(reader.BaseStream.ReadByte());
                header.Header[1] = Convert.ToChar(reader.BaseStream.ReadByte());
                header.Header[2] = Convert.ToChar(reader.BaseStream.ReadByte());
                header.Header[3] = Convert.ToChar(reader.BaseStream.ReadByte());
                if(!header.Verify())
                {
                    return ELoadResult.InvalidFile;
                }

                var has_password = Convert.ToChar(reader.BaseStream.ReadByte()) == 0x01;
                if(has_password && m_password == null)
                {
                    return ELoadResult.NeedPassword;
                }

                byte[] iv = null;
                if(has_password)
                {
                    iv = new byte[16];
                    if(reader.BaseStream.Read(iv, 0, 16) != 16)
                    {
                        return ELoadResult.ReadError;
                    }
                }

                using(var ms = new MemoryStream())
                {
                    byte[] buffer = new byte[256];
                    var read = reader.BaseStream.Read(buffer, 0, 256);
                    while(read != 0)
                    {
                        ms.Write(buffer, 0, read);
                        read = reader.BaseStream.Read(buffer, 0, 256);
                    }

                    byte[] raw_data = ms.ToArray();
                    string data = null;
                    if(has_password)
                    {
                        try
                        {
                            data = DecryptData(raw_data, m_password, iv);
                            if(data == null)
                            {
                                return ELoadResult.ReadError;
                            }
                        }
                        catch(Exception)
                        {
                            return ELoadResult.ReadError;
                        }

                        for(int i = 0; i < DATA_HEADER_VALUE.Length; ++i)
                        {
                            if(data[i] != DATA_HEADER_VALUE[i])
                            {
                                return ELoadResult.InvalidPassword;
                            }
                        }
                        data = data.Substring(4);
                    }
                    else
                    {
                        data = Encoding.ASCII.GetString(raw_data, 4, raw_data.Length - 4);
                    }

                    try
                    {
                        var json_obj = JObject.Parse(data);
                        if(json_obj["QR"] != null)
                        {
                            QR = json_obj["QR"].ToString();
                        }
                        else
                        {
                            return ELoadResult.ReadError;
                        }
                    }
                    catch (Exception)
                    {
                        return ELoadResult.ReadError;
                    }
                }
            }

            return ELoadResult.Success;
        }

        public bool SetPassword(string password)
        {
            if (password == null || password.Length == 0 || password.Length > 1024)
            {
                return false;
            }

            Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(password, SALT, 100000);
            m_password = pbkdf2.GetBytes(16);

            return true;
        }

        public string QR
        {
            get
            {
                return m_qr;
            }

            set
            {
                m_qr = value;
            }
        }

        static private byte[] EncryptData(string data, byte[] key, byte[] iv)
        {
            if (data == null || data.Length <= 0)
            {
                throw new ArgumentException("Invalid data");
            }
            if (key == null || key.Length <= 0)
            {
                throw new ArgumentException("Invalid key");
            }
            if (iv == null || iv.Length <= 0)
            {
                throw new ArgumentException("Invalid iv");
            }

            byte[] ret = null;
            using (var aes_alg = Aes.Create())
            {
                aes_alg.Key = key;
                aes_alg.IV = iv;
                aes_alg.Mode = CipherMode.CBC;
                aes_alg.Padding = PaddingMode.PKCS7;

                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, aes_alg.CreateEncryptor(aes_alg.Key, aes_alg.IV), CryptoStreamMode.Write))
                    {
                        using (var sw = new StreamWriter(cs, Encoding.ASCII))
                        {
                            sw.Write(data);
                        }

                        ret = ms.ToArray();
                    }
                }
            }

            return ret;
        }

        static private string DecryptData(byte[] data, byte[] key, byte[] iv)
        {
            if (data == null || data.Length <= 0)
            {
                throw new ArgumentException("Invalid data");
            }
            if (key == null || key.Length <= 0)
            {
                throw new ArgumentException("Invalid key");
            }
            if (iv == null || iv.Length <= 0)
            {
                throw new ArgumentException("Invalid iv");
            }

            string ret = null;
            using (var aes_alg = Aes.Create())
            {
                aes_alg.Key = key;
                aes_alg.IV = iv;
                aes_alg.Mode = CipherMode.CBC;
                aes_alg.Padding = PaddingMode.PKCS7;

                using (var ms = new MemoryStream(data))
                {
                    using (var cs = new CryptoStream(ms, aes_alg.CreateDecryptor(aes_alg.Key, aes_alg.IV), CryptoStreamMode.Read))
                    {
                        using (var sr = new StreamReader(cs, Encoding.ASCII))
                        {
                            ret = sr.ReadToEnd();
                        }
                    }
                }
            }

            return ret;
        }

        private byte[] m_password = null;
        private string m_qr = null;
    }
}
