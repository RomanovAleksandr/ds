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
                    Console.WriteLine("Ожидание соединения клиента...");
                    // ACCEPT
                    Socket handler = listener.Accept();

                    Console.WriteLine("Получение данных...");
                    byte[] buf = new byte[1024];
                    string data = null;
                    do
                    {
                        int bytesRec = handler.Receive(buf);
                        data += Encoding.UTF8.GetString(buf, 0, bytesRec);
                    }
                    while (handler.Available>0);

                    Console.WriteLine("Полученный текст: {0}", data);
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
            if (args.Length != 1)
            {
                System.Console.WriteLine("Usage: Server <port>");
                return;
            }
            
            if (Int32.TryParse(args[0], out int port))
            {
                Console.WriteLine("Запуск сервера...");
                StartListening(11000);
            }
            else
            {
                Console.WriteLine("Invalid port");
                return;
            }
            
            Console.WriteLine("\nНажмите ENTER чтобы выйти...");
            Console.Read();
        }
    }
}
