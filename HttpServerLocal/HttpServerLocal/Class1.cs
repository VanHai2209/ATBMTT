using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HttpServerLocal
{
    class Class1
    {
        private HttpListener listener;
        public Class1(params string[] prefixes)
        {
            if (!HttpListener.IsSupported)
                throw new Exception("Do not support HttpListener.");

            if (prefixes == null || prefixes.Length == 0)
                throw new ArgumentException("prefixes");

            // Khởi tạo HttpListener
            listener = new HttpListener();
            foreach (string prefix in prefixes)
                listener.Prefixes.Add(prefix);

        }
        public async Task StartAsync()
        {
            // Bắt đầu lắng nghe kết nối HTTP
            listener.Start();
            do
            {

                try
                {
                    Console.WriteLine($"{DateTime.Now.ToLongTimeString()} : waiting a client connect");

                    // Một client kết nối đến
                    HttpListenerContext context = await listener.GetContextAsync();
                    await ProcessRequest(context);

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                Console.WriteLine("...");

            }
            while (listener.IsListening);
        }
        async Task ProcessRequest(HttpListenerContext context)
        {
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;
            Console.WriteLine($"{request.HttpMethod} {request.RawUrl} {request.Url.AbsolutePath}");

            // Lấy stream / gửi dữ liệu về cho client
            var outputstream = response.OutputStream;


            switch (request.Url.AbsolutePath)
            {
                case "/":
                    {
                        byte[] buffer = System.Text.Encoding.UTF8.GetBytes("Access:\n /teacher\n /student");
                        response.ContentLength64 = buffer.Length;
                        await outputstream.WriteAsync(buffer, 0, buffer.Length);
                    }
                    break;
                case "/teacher":
                    {
                        string connectionString = "server=localhost;port=3306;database=encrypted;uid=root;password=;";
                        MySqlConnection connection = new MySqlConnection(connectionString);
                        string query = "SELECT * FROM information_teacher";
                        string queryKey = "02DE2CCEFAE50FF8C4ED318AA8264DF3C2DE41BF09D5BF21AC040E1307D05F5A";
                        try
                        {
                            connection.Open();
                            MySqlCommand cmd = new MySqlCommand(query, connection);
                            MySqlDataReader reader = cmd.ExecuteReader();
                            ArrayList objArr = new ArrayList();
                            while (reader.Read())
                            {
                                //sb.Append(string.Format("Id:{0}, Name:{1}, Address:{2}, Gmail:{3}, Phone:{4}, CCCD: {5} \n",


                                //    reader["Id"].ToString(), reader["Name"].ToString(), reader["Address"].ToString(), reader["Gmail"].ToString(), reader["Phone"].ToString(), reader["CCCD"].ToString()));
                                objArr.Add(new Class2
                                {
                                    id = MaskId(DecryptedAes(reader["Id"].ToString(), queryKey)),
                                    name = DecryptedAes(reader["Name"].ToString(), queryKey),
                                    address = MaskAddress(DecryptedAes(reader["Address"].ToString(), queryKey)),
                                    gmail = MaskGmail(DecryptedAes(reader["Gmail"].ToString(), queryKey)),
                                    phone = MaskPhone(DecryptedAes(reader["Phone"].ToString(), queryKey)),
                                    cccd = MaskCccd(DecryptedAes(reader["CCCD"].ToString(), queryKey)),
                                });
                            }
                            // Trả về response cho client
                            //string responseString = sb.ToString();
                            string responseString = JsonConvert.SerializeObject(objArr);
                            byte[] buffer = Encoding.UTF8.GetBytes(responseString);
                            response.ContentLength64 = buffer.Length;
                            Stream output = response.OutputStream;
                            output.Write(buffer, 0, buffer.Length);
                            output.Close();

                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        finally
                        {
                            connection.Close();
                        }
                    }
                    break;
                case "/student":
                    {
                        string connectionString = "server=localhost;port=3306;database=encrypted;uid=root;password=;";
                        MySqlConnection connection = new MySqlConnection(connectionString);
                        string query = "SELECT * FROM information_student";
                        string queryKey = "9D6EDC72A91322114EBF7E21AB4C5170CE825D66AAC6C2B0FD9C0A966CF922F3";
                        try
                        {
                            connection.Open();
                            MySqlCommand cmd = new MySqlCommand(query, connection);
                            MySqlDataReader reader = cmd.ExecuteReader();
                            ArrayList objArr = new ArrayList();
                            while (reader.Read())
                            {
                                //sb.Append(string.Format("Id:{0}, Name:{1}, Address:{2}, Gmail:{3}, Phone:{4}, CCCD: {5} \n",


                                //    reader["Id"].ToString(), reader["Name"].ToString(), reader["Address"].ToString(), reader["Gmail"].ToString(), reader["Phone"].ToString(), reader["CCCD"].ToString()));
                                objArr.Add(new Class2
                                {
                                    id = MaskId(DecryptedAes(reader["Id"].ToString(),queryKey)),
                                    name = DecryptedAes(reader["Name"].ToString(),queryKey),
                                    address = MaskAddress(DecryptedAes(reader["Address"].ToString(), queryKey)),
                                    gmail = MaskGmail(DecryptedAes(reader["Gmail"].ToString(), queryKey)),
                                    phone = MaskPhone(DecryptedAes(reader["Phone"].ToString(),queryKey)),
                                    cccd = MaskCccd(DecryptedAes(reader["CCCD"].ToString(), queryKey)),
                                });
                            }
                            // Trả về response cho client
                            //string responseString = sb.ToString();
                            string responseString = JsonConvert.SerializeObject(objArr);
                            byte[] buffer = Encoding.UTF8.GetBytes(responseString);
                            response.ContentLength64 = buffer.Length;
                            Stream output = response.OutputStream;
                            output.Write(buffer, 0, buffer.Length);
                            output.Close();

                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        finally
                        {
                            connection.Close();
                        }
                    }
                    break;

                default:
                    {
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        byte[] buffer = System.Text.Encoding.UTF8.GetBytes("NOT FOUND!");
                        response.ContentLength64 = buffer.Length;
                        await outputstream.WriteAsync(buffer, 0, buffer.Length);
                    }
                    break;
            }

            // switch (request.Url.AbsolutePath)


            // Đóng stream để hoàn thành gửi về client
            outputstream.Close();
        }
        public static string DecryptedAes(string encryptedData, string keyString)
        {
            byte[] key = hextoByte(keyString);
            byte[] encryptedDataBytes = hextoByte(encryptedData);
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;

                // Sử dụng chế độ CBC để giải mã
                aes.Mode = CipherMode.CBC;

                // Tách vector khởi tạo và dữ liệu mã hóa từ mảng byte đầu vào
                byte[] iv = new byte[16];
                byte[] encrypted = new byte[encryptedDataBytes.Length - 16];
                Buffer.BlockCopy(encryptedDataBytes, 0, iv, 0, iv.Length);
                Buffer.BlockCopy(encryptedDataBytes, iv.Length, encrypted, 0, encrypted.Length);
                aes.IV = iv;

                // Giải mã chuỗi
                using (ICryptoTransform decryptor = aes.CreateDecryptor())
                {
                    byte[] decryptedBytes = decryptor.TransformFinalBlock(encrypted, 0, encrypted.Length);
                    return Encoding.UTF8.GetString(decryptedBytes);
                }
            }
        }
        public static byte[] hextoByte(string stringHex)
        {
            byte[] bytes = new byte[stringHex.Length / 2];
            for (int i = 0; i < stringHex.Length; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(stringHex.Substring(i, 2), 16);
            }
            return bytes;
        }
        public static string MaskId(string id)
        {
            int len = id.Length;

            // If the id is shorter than 3 characters, show the first character only
            if (len < 3)
            {
                return id.Substring(0, 1) + new String('x', len - 1);
            }

            // If the id is longer than 2 characters, show the first and last character
            return id.Substring(0, 1) + new String('x', len - 2) + id.Substring(len - 1, 1);
        }

        public static string MaskCccd(string input)
        {
            // Kiểm tra input có null hoặc rỗng không
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            // Kiểm tra độ dài input, chỉ che giấu nếu độ dài input lớn hơn 7
            if (input.Length <= 7)
            {
                return input;
            }

            // Lấy ký tự đầu và ký tự cuối của input
            string firstChars = input.Substring(0, 4);
            string lastChars = input.Substring(input.Length - 3, 3);

            // Tạo chuỗi kí tự x có độ dài bằng phần giữa của input
            int xLength = input.Length - 7;
            string xChars = new string('x', xLength);

            // Trả về chuỗi kết quả đã che giấu
            return $"{firstChars}-{xChars}-{lastChars}";
        }
        public static string MaskPhone(string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber) || phoneNumber.Length < 10) // Kiểm tra số điện thoại hợp lệ
            {
                return phoneNumber; // Nếu không hợp lệ thì không mask và trả về số điện thoại ban đầu
            }

            string maskedPhoneNumber = phoneNumber.Substring(0, phoneNumber.Length - 6) + new string('*', 6); // Thay thế 6 ký tự cuối bằng dấu sao
            return maskedPhoneNumber;
        }
        public static string MaskGmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return string.Empty;
            }

            int index = email.IndexOf('@');
            if (index <= 0)
            {
                return email;
            }

            string masked = email.Substring(0, 1) + "***" + email.Substring(index - 1);
            return masked;
        }
        public static string MaskAddress(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return "";
            }

            string[] parts = input.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string lastPart = parts[parts.Length - 1];

            // Replace all but the last part with asterisks
            string maskedString = "";
            for (int i = 0; i < parts.Length - 1; i++)
            {
                maskedString += new string('*', parts[i].Length) + " ";
            }

            // Append the last part without masking
            maskedString += lastPart;

            return maskedString;
        }
    }
}
