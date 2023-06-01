using System;
using System.Windows.Forms;


namespace HttpServerLocal
{

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Turn On Local Server");
            var serverLocal = new Class1(new string[] {"http://*:8080/"});
            await serverLocal.StartAsync();
            
        }
 
    }
}
