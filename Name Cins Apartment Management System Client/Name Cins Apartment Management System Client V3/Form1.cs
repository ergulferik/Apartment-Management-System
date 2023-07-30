using System;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace Name_Cins_Apartment_Management_System_Client_V3
{
    public partial class Form1 : Form
    {
        private Socket client;
        private byte[] data = new byte[1024];
        private int size = 1024;

        private String[] usersInfo = new String[9];

        public Form1()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
            for (int i = 1; i < 9; i++)
            {
                usersInfo[i] = "Family " + i.ToString();
            }           
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ButtonSendOnClick();
        }

        void ButtonSendOnClick()
        {
            byte[] message = Encoding.ASCII.GetBytes(comboBox1.SelectedItem.ToString() + "*-*" + label2.Text + "*-*" + textBox1.Text);
            textBox1.Clear();
            client.BeginSend(message, 0, message.Length, SocketFlags.None,
                         new AsyncCallback(SendData), client);
        }

        void ButtonConnectOnClick()
        {
            textBox1.Text = "Connecting...";
            Socket newsock = new Socket(AddressFamily.InterNetwork,
                                  SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint iep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9050);
            newsock.BeginConnect(iep, new AsyncCallback(Connected), newsock);
        }

        void Connected(IAsyncResult iar)
        {
            textBox1.Text = "";
            client = (Socket)iar.AsyncState;
            try
            {
                Thread receiver = new Thread(new ThreadStart(ReceiveData));
                receiver.Start();

                Thread sender = new Thread(new ThreadStart(sendData));
                sender.Start();

                Thread colorThread = new Thread(color);
                colorThread.Start();

                client.EndConnect(iar);
                client.BeginReceive(data, 0, size, SocketFlags.None,
                              new AsyncCallback(ReceiveData), client);
            }
            catch (SocketException)
            {
                textBox1.Text = "Error connecting";
            }
        }

        void color()
        {
            while (true)
            {

                for (int i = 0; i < 200; i++)
                {
                    Thread.Sleep(5);
                    this.BackColor = Color.FromArgb(0, 50+i, 0);
                }
                for (int i = 0; i < 200; i++)
                {
                    Thread.Sleep(5);
                    this.BackColor = Color.FromArgb(0, 250 - i, 0);
                }

            }

        }


        void sendData()
        {
            byte[] message = Encoding.ASCII.GetBytes(comboBox1.SelectedItem.ToString() + "*-*" + label2.Text + "*-*");
            client.BeginSend(message, 0, message.Length, SocketFlags.None,
             new AsyncCallback(SendData), client);
        }

        void ReceiveData()
        {
            int recv;
            string stringData;
            string[] currency;
            string[] weather;
            while (true)
            {
                recv = client.Receive(data);
                stringData = Encoding.ASCII.GetString(data, 0, recv);

                if (stringData.StartsWith("$/$"))
                {
                    currency = stringData.Split("$/$");
                    label1.Text = "Dolar : " + currency[1] + " ₺";
                    label3.Text = "Euro : " + currency[2] + " ₺";
                }
                else if (stringData.StartsWith("!$#$!"))
                {
                    weather = stringData.Split("!$#$!");
                    label7.Text = "Weather :" + weather[1];
                    label6.Text = "Temperature :" + weather[2] + "°C";
                }
                else
                {
                    listBox1.Items.Add(stringData);
                }
            }
        }

        void ReceiveData(IAsyncResult iar)
        {
            Socket remote = (Socket)iar.AsyncState;
            int recv = remote.EndReceive(iar);
            string stringData = Encoding.ASCII.GetString(data, 0, recv);

            string[] weather;
            string[] currency;
            if (stringData.StartsWith("$/$"))
            {
                currency = stringData.Split("$/$");
                label1.Text = "Dolar : " + currency[1] + " ₺";
                label3.Text = "Euro : " + currency[2] + " ₺";
            }
            else if (stringData.StartsWith("!$#$!"))
            {
                weather = stringData.Split("!$#$!");
                label7.Text = "Weather :" + weather[1];
                label6.Text = "Temperature :" + weather[2] + "°C";
            }
            else
            {
                listBox1.Items.Add(stringData);
            }
        }
        void SendData(IAsyncResult iar)
        {
            Socket remote = (Socket)iar.AsyncState;
            int sent = remote.EndSend(iar);
            remote.BeginReceive(data, 0, size, SocketFlags.None,
                          new AsyncCallback(ReceiveData), remote);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            String cardId = textBox2.Text;

            for (int i = 0; i < 9; i++)
            {
                if ("Family "+cardId[5].ToString() == usersInfo[i])
                {
                    label1.Visible = true;
                    label2.Visible = true;
                    label3.Visible = true;
                    textBox1.Visible = true;
                    listBox1.Visible = true;
                    comboBox1.Visible = true;
                    button1.Visible = true;
                    label6.Visible = true;
                    label7.Visible = true;
                    label8.Visible = true;

                    textBox2.Visible = false;
                    button2.Visible = false;
                    button3.Visible = false;

                    label2.Text = "Family " + cardId[5].ToString();
                    comboBox1.Items.Remove("Family " + cardId[5].ToString());
                    ButtonConnectOnClick();
                }
            }

        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                ButtonSendOnClick();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                openFileDialog1.FileName = "*";
                openFileDialog1.Filter= "Text File (*txt)|*.txt";
                openFileDialog1.ShowDialog();
                StreamReader read = new StreamReader(openFileDialog1.FileName);
                textBox2.Text = read.ReadToEnd();
                read.Close();
            }
            catch { 
            
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox2.Enabled = false;
            this.BackColor = Color.Red;
        }
    }
}