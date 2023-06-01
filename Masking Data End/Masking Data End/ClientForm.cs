using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Net.Http;
using System.Windows.Forms;

namespace Masking_Data_End
{
    public partial class ClientForm : Form
    {
        public ClientForm()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            object selectedItem = comboBox1.SelectedItem;
            if (selectedItem == null)
            {
                MessageBox.Show("Please Choose Table", "Message");
            }
            else
            {
                if(selectedItem.ToString()=="Teacher"){
                    using (HttpClient client = new HttpClient())
                    {
                        string apiUrl = "http://localhost:8080/teacher";
                        Uri uri = new Uri(apiUrl);
                        // Gửi yêu cầu HTTP GET đến địa chỉ url của server
                        HttpResponseMessage response = await client.GetAsync(uri);

                        // Đọc nội dung JSON được trả về từ server
                        string jsonString = await response.Content.ReadAsStringAsync();

                        // Phân tích chuỗi JSON và gán giá trị vào các thành phần trên form của bạn
                        List<DataMasked> data = JsonConvert.DeserializeObject<List<DataMasked>>(jsonString);
                        dataGridView1.DataSource = data;
                    }
                }
                else
                {
                    using (HttpClient client = new HttpClient())
                    {
                        string apiUrl = "http://localhost:8080/student";
                        Uri uri = new Uri(apiUrl);
                        // Gửi yêu cầu HTTP GET đến địa chỉ url của server
                        HttpResponseMessage response = await client.GetAsync(uri);

                        // Đọc nội dung JSON được trả về từ server
                        string jsonString = await response.Content.ReadAsStringAsync();

                        // Phân tích chuỗi JSON và gán giá trị vào các thành phần trên form của bạn
                        List<DataMasked> data = JsonConvert.DeserializeObject<List<DataMasked>>(jsonString);
                        dataGridView1.DataSource = data;

                    }
                }
            }
        }
    }
}
