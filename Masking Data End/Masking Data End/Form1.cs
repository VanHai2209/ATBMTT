using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace Masking_Data_End
{
    public partial class Form1 : Form
    {
        public string connectionString = "server=localhost;port=3306;database=encrypted;uid=root;password=;";
        
        string[] arrayInformation = new string[6];
        string[] arrayInforEncrypt = new string[6];
        public Form1()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }
        public static string EncryptedAes(string data, string keyString)
        {
            byte[] encrypted;
            byte[] key = hextoByte(keyString);
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;

                // Sử dụng chế độ CBC để mã hóa
                aes.Mode = CipherMode.CBC;

                // Tạo một vector khởi tạo ngẫu nhiên
                aes.GenerateIV();
                byte[] iv = aes.IV;

                // Mã hóa chuỗi
                using (ICryptoTransform encryptor = aes.CreateEncryptor())
                {
                    byte[] plainTextBytes = Encoding.UTF8.GetBytes(data);
                    encrypted = encryptor.TransformFinalBlock(plainTextBytes, 0, plainTextBytes.Length);
                }

                // Ghép vector khởi tạo và dữ liệu mã hóa thành một mảng byte
                byte[] result = new byte[iv.Length + encrypted.Length];
                Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
                Buffer.BlockCopy(encrypted, 0, result, iv.Length, encrypted.Length);

                return BitConverter.ToString(result).Replace("-", ""); ;
            }
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

        private void btnEncrypted_Click(object sender, EventArgs e)
        {
  
            arrayInformation[0] = txtId.Text;
            arrayInformation[1] = txtName.Text;
            arrayInformation[2] = txtAddress.Text;
            arrayInformation[3] = txtGmail.Text;
            arrayInformation[4] = txtPhone.Text;
            arrayInformation[5] = txtCccd.Text;
            for(int i = 0;i < arrayInformation.Length;i++)
            {
                arrayInforEncrypt[i]=EncryptedAes(arrayInformation[i], txtKey.Text);
                
            }
            txtId1.Text = arrayInforEncrypt[0];
            txtName1.Text = arrayInforEncrypt[1];
            txtAddress1.Text = arrayInforEncrypt[2];
            txtGmail1.Text = arrayInforEncrypt[3];
            txtPhone1.Text = arrayInforEncrypt[4];
            txtCccd1.Text = arrayInforEncrypt[5];
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            object selectedItem = comboBox1.SelectedItem;
            if(selectedItem == null)
            {
                MessageBox.Show("Please Choose Table", "Message");
            }
            else
            {
                string stringTable = selectedItem.ToString();
                // tạo kết nối đến database
                MySqlConnection conn = new MySqlConnection("server=localhost;user id=root;password=;database=encrypted");

                // mở kết nối
                conn.Open();
                if (stringTable == "Teacher")
                {
                    // tạo câu truy vấn SQL
                    string sql = "INSERT INTO information_teacher (Id, Name, Address, Gmail, Phone, CCCD) VALUES (@Id, @Name, @Address, @Gmail, @Phone, @CCCD)";

                    // tạo đối tượng MySqlCommand để thực hiện câu truy vấn
                    MySqlCommand cmd = new MySqlCommand(sql, conn);

                    // thêm các tham số vào câu truy vấn
                    cmd.Parameters.AddWithValue("@Id", arrayInforEncrypt[0]);
                    cmd.Parameters.AddWithValue("@Name", arrayInforEncrypt[1]);
                    cmd.Parameters.AddWithValue("@Address", arrayInforEncrypt[2]);
                    cmd.Parameters.AddWithValue("@Gmail", arrayInforEncrypt[3]);
                    cmd.Parameters.AddWithValue("@Phone", arrayInforEncrypt[4]);
                    cmd.Parameters.AddWithValue("@CCCD", arrayInforEncrypt[5]);

                    // thực hiện câu truy vấn
                    cmd.ExecuteNonQuery();

                    // đóng kết nối
                    conn.Close();
                    MessageBox.Show("Send encrypted data to table Teacher successfully", "Message");
                }
                else if (stringTable == "Student")
                {
                    // tạo câu truy vấn SQL
                    string sql = "INSERT INTO information_student (Id, Name, Address, Gmail, Phone, CCCD) VALUES (@Id, @Name, @Address, @Gmail, @Phone, @CCCD)";

                    // tạo đối tượng MySqlCommand để thực hiện câu truy vấn
                    MySqlCommand cmd = new MySqlCommand(sql, conn);

                    // thêm các tham số vào câu truy vấn
                    cmd.Parameters.AddWithValue("@Id", arrayInforEncrypt[0]);
                    cmd.Parameters.AddWithValue("@Name", arrayInforEncrypt[1]);
                    cmd.Parameters.AddWithValue("@Address", arrayInforEncrypt[2]);
                    cmd.Parameters.AddWithValue("@Gmail", arrayInforEncrypt[3]);
                    cmd.Parameters.AddWithValue("@Phone", arrayInforEncrypt[4]);
                    cmd.Parameters.AddWithValue("@CCCD", arrayInforEncrypt[5]);

                    // thực hiện câu truy vấn
                    cmd.ExecuteNonQuery();

                    // đóng kết nối
                    conn.Close();
                    MessageBox.Show("Send encrypted data to table Student successfully", "Message");
                }
            }
            
        }

        private void btnShow_Click(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("ID", typeof(string));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Adress", typeof(string));
            dt.Columns.Add("Gmail", typeof(string));
            dt.Columns.Add("Phone", typeof(string));
            dt.Columns.Add("CCCD", typeof(string));
            object selectedItem = comboBox2.SelectedItem;
            if (selectedItem == null)
            {
                MessageBox.Show("Please Choose Table", "Message");
            }
            else
            {
                string stringTable = selectedItem.ToString();
                if(stringTable == "Teacher")
                {
                    string query = "select *from information_teacher";
                    MySqlConnection connection = new MySqlConnection(connectionString);

                    try
                    {
                        connection.Open();
                        MySqlCommand cmd = new MySqlCommand(query, connection);
                        MySqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {

                            dt.Rows.Add(DecryptedAes(reader["Id"].ToString(), txtKey.Text)
                                , DecryptedAes(reader["Name"].ToString(), txtKey.Text)
                                , DecryptedAes(reader["Address"].ToString(), txtKey.Text)
                                , DecryptedAes(reader["Gmail"].ToString(), txtKey.Text)
                                , DecryptedAes(reader["Phone"].ToString(), txtKey.Text)
                                , DecryptedAes(reader["CCCD"].ToString(), txtKey.Text));
                        }
                        dataGridView1.DataSource = dt;
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
                else if (stringTable=="Student")
                {
                    string query = "select *from information_student";
                    MySqlConnection connection = new MySqlConnection(connectionString);

                    try
                    {
                        connection.Open();
                        MySqlCommand cmd = new MySqlCommand(query, connection);
                        MySqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {

                            dt.Rows.Add(DecryptedAes(reader["Id"].ToString(), txtKey.Text)
                                , DecryptedAes(reader["Name"].ToString(), txtKey.Text)
                                , DecryptedAes(reader["Address"].ToString(), txtKey.Text)
                                , DecryptedAes(reader["Gmail"].ToString(), txtKey.Text)
                                , DecryptedAes(reader["Phone"].ToString(), txtKey.Text)
                                , DecryptedAes(reader["CCCD"].ToString(), txtKey.Text));
                        }
                        dataGridView1.DataSource = dt;
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
            }
        }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
          string key = Generate256BitKey();
            txtKey.Text = key;
        }
        //Create key 256 bit random
        public static string Generate256BitKey()
        {
            byte[] key = new byte[32];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(key);
            }
            string keyString = BitConverter.ToString(key).Replace("-", "");
            return keyString;
        }
        //Convert Byte to String Hex

        //Convert String Hex to Byte
        public static byte[] hextoByte(string stringHex)
        {
            byte[] bytes = new byte[stringHex.Length / 2];
            for (int i = 0; i < stringHex.Length; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(stringHex.Substring(i, 2), 16);
            }
            return bytes;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
