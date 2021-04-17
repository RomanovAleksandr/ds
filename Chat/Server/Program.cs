using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;

namespace Server
{
    class Program
    {
        public static void StartListening(int port)
        {

            // Разрешение сетевых имён
            
            // Привязываем сокет ко всем интерфейсам на текущей машинe
            IPAddress ipAddress = IPAddress.Any; 
            
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);

            // CREATE
            Socket listener = new Socket(
                ipAddress.AddressFamily,
                SocketType.Stream,
                ProtocolType.Tcp);

            List<string> messages = new List<string>();

            try
            {
                // BIND
                listener.Bind(localEndPoint);

                // LISTEN
                listener.Listen(10);

                while (true)
                {
                    // ACCEPT
                    Socket handler = listener.Accept();

                    byte[] buf = new byte[1024];
                    string data = null;
                    do
                    {
                        int bytesRec = handler.Receive(buf);
                        data += Encoding.UTF8.GetString(buf, 0, bytesRec);
                    }
                    while (handler.Available>0);

                    Console.WriteLine(data);
                    messages.Add(data);

                    // Отправляем текст обратно клиенту
                    var result = String.Join("\n", messages.ToArray());
                    byte[] msg = Encoding.UTF8.GetBytes(result);

                    // SEND
                    handler.Send(msg);

                    // RELEASE
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        static void Main(string[] args)
        {
            StartListening(Int32.Parse(args[0]));
        }
    }
}
