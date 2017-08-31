using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;

namespace AsyncMultiClient
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            try
            {
                string str_ip = textBoxIP.Text;
               // IPAddress.Parse(str_ip);
                MessageBox.Show(str_ip);

                buttonConnect.Enabled = false;
                buttonDisconnect.Enabled = true;
                buttonSend.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
