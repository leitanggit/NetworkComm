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
using System.Net.Sockets;

// test branch commit

namespace AsyncMultiClient
{
    public partial class MainForm : Form
    {
        private Socket clientSocket;
        private byte[] buffer;
        
        public MainForm()
        {
            InitializeComponent();
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            try
            {
                string str_ip = textBoxIP.Text;
                clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(str_ip), 3333);
                clientSocket.BeginConnect(endPoint, ConnectCallback, null);
                int temp=1;   // test code
                string ip;    // test 2
				int a=2;      // test 3
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                clientSocket.EndConnect(ar);
                buffer = new byte[clientSocket.ReceiveBufferSize];
                clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, ReceiveCallback, null);
                UpdateGui(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateGui(false);
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                int received = clientSocket.EndReceive(ar);
                if (received == 0) return;
                clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, ReceiveCallback, null);
            }
            catch (SocketException ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (ObjectDisposedException ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch ( Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                clientSocket.EndSend(ar);
            }
            catch (SocketException ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (ObjectDisposedException ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateGui(bool connected)
        {
            Invoke((Action)delegate
            {
                if (connected)
                {
                    buttonConnect.Enabled = false;
                    buttonDisconnect.Enabled = true;
                    buttonSend.Enabled = true;
                }
                else
                {
                    buttonConnect.Enabled = true;
                    buttonDisconnect.Enabled = false;
                    buttonSend.Enabled = false;
                }
            });
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            try
            {
                PersonPackage person = new PersonPackage(radioButtonMale.Checked, (ushort)numericUpDownAge.Value, textBoxName.Text);
                byte[] buffer = person.ToByteArray();
                clientSocket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, SendCallback, null);
            }
            catch (SocketException ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch ( ObjectDisposedException ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonDisconnect_Click(object sender, EventArgs e)
        {
            try
            {
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Disconnect(false);
               // clientSocket.Close();              
                UpdateGui(false);
            }
            catch( SocketException ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    class PersonPackage
    {
        public bool IsMale { get; set; }
        public ushort Age { get; set; }
        public string Name { get; set; }

        public PersonPackage(bool male, ushort age, string name)
        {
            IsMale = male;
            Age = age;
            Name = name;
        }

        public byte[] ToByteArray()
        {
            List<byte> byteList = new List<byte>();
            byteList.AddRange(BitConverter.GetBytes(IsMale));
            byteList.AddRange(BitConverter.GetBytes(Age));
            byteList.AddRange(BitConverter.GetBytes(Name.Length));
            byteList.AddRange(Encoding.ASCII.GetBytes(Name));
            return byteList.ToArray();
        }

        public PersonPackage(byte[] data)
        {
            IsMale = BitConverter.ToBoolean(data, 0);
            Age = BitConverter.ToUInt16(data, 1);
            int nameLength = BitConverter.ToInt32(data, 3);
            Name = Encoding.ASCII.GetString(data, 7, nameLength);
        }
    }
}
