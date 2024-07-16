using System.Net;
using System.Net.Sockets;
using System.Text;

namespace UDP_Chat
{
    internal class Chat
    {
        private UdpClient? client;

        public void UDP_Chat(string ip, int port)
        {
            client = new UdpClient();
            client.Connect(ip, port);
        }

        public static void Server()
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
            UdpClient client = new UdpClient(12345);
            Console.WriteLine("Waiting incomming messages...");
            while (true)
            {
                try
                {
                    byte[] buffer = client.Receive(ref endPoint);
                    string str = Encoding.UTF8.GetString(buffer);

                    Message? msg = Message.ConvertFromJSON(str);
                    if (msg != null)
                    {
                        Console.WriteLine(msg.ToString());
                        Message msgToClient = new Message("server", "message delivered", DateTime.Now);
                        string js = msgToClient.ConvertToJSON();
                        byte[] bytes = Encoding.UTF8.GetBytes(js);
                        client.Send(bytes, endPoint);
                    }
                    else
                    {
                        Console.WriteLine("Huston, we have a problem!!!");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public static void Client(string nickName)
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 12345);
            UdpClient client = new UdpClient();

            while (true)
            {
                Console.WriteLine("Input massage:");
                string? text = Console.ReadLine();
                if (String.IsNullOrEmpty(text))
                {
                    continue;
                }

                Message msg = new Message(nickName, text, DateTime.Now);
                string js = msg.ConvertToJSON();
                byte[] bytes = Encoding.UTF8.GetBytes(js);
                client.Send(bytes, endPoint);

                byte[] buffer = client.Receive(ref endPoint);
                string str = Encoding.UTF8.GetString(buffer);
                Message? msgFromServer = Message.ConvertFromJSON(str);
                Console.WriteLine(msgFromServer);
            }
        }

    }
}
