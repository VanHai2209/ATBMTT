using MySql.Data.MySqlClient;
using System;
using System.Windows.Forms;

namespace Masking_Data_End
{
    public partial class loginForm : Form
    {
        public string connectionString = "server=localhost;port=3306;database=account;uid=root;password=;";
        public loginForm()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }
        private void NewForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            // hiển thị lại form hiện tại
            this.Show();
            txtUser.Text = null;
            txtPass.Text = null;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            
            object selectedItem = comboBox1.SelectedItem;
            if(selectedItem == null)
            {
                MessageBox.Show("Please Choose Admin or Client");
            }
            else
            {
                string stringKind = selectedItem.ToString();
                if(stringKind == "Admin")
                {
                    using (MySqlConnection connection = new MySqlConnection(connectionString))
                    {
                        string username = txtUser.Text;
                        string password = txtPass.Text;
                        string query = "SELECT COUNT(*) FROM admin WHERE Username = @Username AND Password = @Password";
                        MySqlCommand command = new MySqlCommand(query, connection);
                        command.Parameters.AddWithValue("@Username", username);
                        command.Parameters.AddWithValue("@Password", password);

                        connection.Open();
                        int count = Convert.ToInt32(command.ExecuteScalar());

                        if (count > 0)
                        {
                            Form1 form = new Form1();
                            form.FormClosed += NewForm_FormClosed;
                            form.Show();
                            this.Hide();
                            Console.WriteLine("Login successful!");
                        }
                        else
                        {
                            MessageBox.Show("Invalid username or password!");
                        }
                    }
                }
                else if(stringKind == "Client")
                {
                    using (MySqlConnection connection = new MySqlConnection(connectionString))
                    {
                        string username = txtUser.Text;
                        string password = txtPass.Text;
                        string query = "SELECT COUNT(*) FROM client WHERE Username = @Username AND Password = @Password";
                        MySqlCommand command = new MySqlCommand(query, connection);
                        command.Parameters.AddWithValue("@Username", username);
                        command.Parameters.AddWithValue("@Password", password);

                        connection.Open();
                        int count = Convert.ToInt32(command.ExecuteScalar());


                        if (count >0)
                        {
                            Console.WriteLine("Login successful!");
                            

                            ClientForm form = new ClientForm();
                            form.FormClosed += NewForm_FormClosed;
                            form.Show();
                            this.Hide();
                        }
                        else
                        {
                            MessageBox.Show("Invalid username or password!");
                        }
                    }
                }
            }

        }
    }
}
