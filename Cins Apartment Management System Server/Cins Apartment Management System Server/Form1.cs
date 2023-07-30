using System;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using Newtonsoft.Json;
using static System.Net.WebRequestMethods;



namespace Cins_Apartment_Management_System_Server
{
    public partial class Form1 : Form
    {
        private Socket server;
        private byte[] data = new byte[1024];
        private int size = 1024;
        Socket Tempclient;
        Socket client;
        bool flag = false;

        

        List <Socket> clients = new List<Socket>();

        Dictionary<string, Socket> users = new Dictionary<string, Socket>();

        XmlDocument xmlFile = new XmlDocument();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
                ///Dictionary assignment///
            for (int i = 1; i < 9; i++)
            {
                users.Add("Family " + i.ToString(), Tempclient);
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            ///Starting server///

            server = new Socket(AddressFamily.InterNetwork,SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint iep = new IPEndPoint(IPAddress.Any, 9050);
            server.Bind(iep);
            server.Listen(5);

            Thread threadBeginAccept = new Thread(beginAccept);
            threadBeginAccept.Start();

            label1.Text = "Server On";
        }

            ///Function for threaded server///
        public void beginAccept()
        {
            for (int i = 0; i < 10; i++)
            {            
                server.BeginAccept(new AsyncCallback(AcceptConn), server);
            }
        }

        ///Connetion accept///
        void AcceptConn(IAsyncResult iar)
        {   
            Socket oldserver = (Socket)iar.AsyncState;
            client = oldserver.EndAccept(iar);

            clients.Add(client);

            listBox2.Items.Add(client.RemoteEndPoint.ToString()  + " Date & Time : " + DateTime.Now);

                ///Sending data to client///
            textBox1.Text = "Connected to: " + client.RemoteEndPoint.ToString();
            string stringData = "Welcome to Cins Apertment Management System";
            byte[] message1 = Encoding.ASCII.GetBytes(stringData);

                ///Begin send///

            client.BeginSend(message1, 0, message1.Length, SocketFlags.None,
                        new AsyncCallback(SendData), client);
        }

        ///Continue sending///
        void SendData(IAsyncResult iar)
        {
            Socket client = (Socket)iar.AsyncState;
            int sent = client.EndSend(iar);
            client.BeginReceive(data, 0, size, SocketFlags.None,
                        new AsyncCallback(ReceiveData), client);
        }

        void ReceiveData(IAsyncResult iar)
        {
            ///Receive Data///

            Socket client = (Socket)iar.AsyncState;
            int recv = client.EndReceive(iar);
            if (recv == 0)
            {
                client.Close();
                MessageBox.Show("No received message. Waiting to connect...");
                server.BeginAccept(new AsyncCallback(AcceptConn), server);
                return;
            }

            ///Received data///
 
            string receivedData = Encoding.ASCII.GetString(data, 0, recv);
            String msg = DateTime.Now.ToString() + " -- ";
            
            ///Substring for sender///

            String[] messageMetas = receivedData.Split("*-*");
            
            ///Adding sender info in the message ///

            foreach (KeyValuePair<string, Socket> item in users)
            {
                if (item.Key.Equals(messageMetas[1]))
                {
                    users[item.Key] = client;
                    msg += item.Key;
                }
            }

            msg += " : " + messageMetas[2];
            listBox1.Items.Add(msg);

            ///Echo server to all clients

            byte[] message2 = Encoding.ASCII.GetBytes(msg);
            
            if (messageMetas[0] == "Me")
            {
                client.BeginSend(message2, 0, message2.Length, SocketFlags.None,
                                            new AsyncCallback(SendData), client);
            }

            else if (messageMetas[0] == "Family 1")
            {
                client.BeginSend(message2, 0, message2.Length, SocketFlags.None,
                            new AsyncCallback(SendData), client);
                users["Family 1"].BeginSend(message2, 0, message2.Length, SocketFlags.None,
                                                        new AsyncCallback(SendData), client);
            }

            else if (messageMetas[0] == "Family 2")
            {
                client.BeginSend(message2, 0, message2.Length, SocketFlags.None,
                            new AsyncCallback(SendData), client);
                users["Family 2"].BeginSend(message2, 0, message2.Length, SocketFlags.None,
                                                        new AsyncCallback(SendData), client);
            }

            else if (messageMetas[0] == "Family 3")
            {
                client.BeginSend(message2, 0, message2.Length, SocketFlags.None,
                                            new AsyncCallback(SendData), client);
                users["Family 3"].BeginSend(message2, 0, message2.Length, SocketFlags.None,
                                                        new AsyncCallback(SendData), client);
            }

            else if (messageMetas[0] == "Family 4")
            {
                client.BeginSend(message2, 0, message2.Length, SocketFlags.None,
                            new AsyncCallback(SendData), client);
                users["Family 4"].BeginSend(message2, 0, message2.Length, SocketFlags.None,
                                                        new AsyncCallback(SendData), client);
            }

            else if (messageMetas[0] == "Family 5")
            {
                client.BeginSend(message2, 0, message2.Length, SocketFlags.None,
                            new AsyncCallback(SendData), client);
                users["Family 5"].BeginSend(message2, 0, message2.Length, SocketFlags.None,
                                                        new AsyncCallback(SendData), client);
            }
            else if (messageMetas[0] == "Family 6")
            {
                client.BeginSend(message2, 0, message2.Length, SocketFlags.None,
                            new AsyncCallback(SendData), client);
                users["Family 6"].BeginSend(message2, 0, message2.Length, SocketFlags.None,
                                                        new AsyncCallback(SendData), client);
            }
            else if (messageMetas[0] == "Family 7")
            {
                client.BeginSend(message2, 0, message2.Length, SocketFlags.None,
                            new AsyncCallback(SendData), client);
                users["Family 7"].BeginSend(message2, 0, message2.Length, SocketFlags.None,
                                                        new AsyncCallback(SendData), client);
            }
            else if (messageMetas[0] == "Family 8")
            {
                client.BeginSend(message2, 0, message2.Length, SocketFlags.None,
                            new AsyncCallback(SendData), client);
                users["Family 8"].BeginSend(message2, 0, message2.Length, SocketFlags.None,
                                                        new AsyncCallback(SendData), client);
            }
            else if (messageMetas[0] == "All Family")
            {
                for (int i = 0; i < clients.Count; i++)
                {
                    clients[i].BeginSend(message2, 0, message2.Length, SocketFlags.None,
                                                        new AsyncCallback(SendData), client);
                }

            }

        }

        private void timer1_Tick_1(object sender, EventArgs e)
        {

            string today = "https://www.tcmb.gov.tr/kurlar/today.xml";
            xmlFile.Load(today);

            string dolar = xmlFile.SelectSingleNode("Tarih_Date/Currency[@Kod='USD']/BanknoteSelling").InnerXml;
            string euro = xmlFile.SelectSingleNode("Tarih_Date/Currency[@Kod='EUR']/BanknoteSelling").InnerXml;

            byte[] currency = Encoding.ASCII.GetBytes("$/$" + dolar + "$/$" + euro);

            for (int i = 0; i < clients.Count; i++)
            {
                clients[i].BeginSend(currency, 0, currency.Length, SocketFlags.None,
                        new AsyncCallback(SendData), client);
            }


            string APIKey = "a1e27152d07c199e4dbf29b0827e119d";
            WebClient web = new WebClient();
            string url = string.Format("https://api.openweathermap.org/data/2.5/weather?q=manisa&units=metric&appid={0}", APIKey);

            var json = web.DownloadString(url);
            WeatherInfo.root Info = JsonConvert.DeserializeObject<WeatherInfo.root>(json);
            
            byte[] message1 = Encoding.ASCII.GetBytes("!$#$!" + Info.weather[0].main + "!$#$!" + Info.main.temp.ToString());
            for (int i = 0; i < clients.Count; i++)
            {
                clients[i].BeginSend(message1, 0, message1.Length, SocketFlags.None,
                                                        new AsyncCallback(SendData), client);
            }          
        }
    }
    internal class WeatherInfo
    {
        public class coord
        {
            public double lon { get; set; }
            public double lat { get; set; }
        }

        public class weather
        {
            public string main { get; set; }
            public string description { get; set; }
            public string icon { get; set; }

        }

        public class main
        {
            public double temp { get; set; }

            public double pressure { get; set; }
            public double humidity { get; set; }

        }

        public class wind
        {
            public double speed { get; set; }

        }

        public class sys
        {
            public long sunrise { get; set; }
            public long sunset { get; set; }
        }
        public class root
        {
            public coord coord { get; set; }
            public List<weather> weather { get; set; }
            public main main { get; set; }

            public wind wind { get; set; }
            public sys sys { get; set; }

        }
    }

}