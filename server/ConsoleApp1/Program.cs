using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace SocketTcpServer
{

    class Program
    {
        
        static int port = 8005; // порт для приема входящих запросов
        static void Main(string[] args)
        {
            string[] word = new string[7] { "понедельник", "вторник", "среда", "четверг", "пятница", "суббота", "воскресенье"};
            string sen="",err="";
            int count = 0;
            // получаем адреса для запуска сокета
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);

            // создаем сокет
            Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                // связываем сокет с локальной точкой, по которой будем принимать данные
                listenSocket.Bind(ipPoint);

                // начинаем прослушивание
                listenSocket.Listen(10);

                Console.WriteLine("Сервер запущен. Ожидание подключений...");

                while (true)
                {
                    Socket handler = listenSocket.Accept();
                    // получаем сообщение
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0; // количество полученных байтов
                    byte[] data = new byte[256]; // буфер для получаемых данных

                    do
                    {
                        bytes = handler.Receive(data);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (handler.Available > 0);

                    
                    count = Convert.ToInt32(builder.ToString());
                    int i = 0;
                    // отправляем ответ
                    if (count <= 7 && count>0)
                    {
                        while (count != 0)
                        {
                            sen += word[i] + " ";
                            count--;
                            i++;
                        }
                        string message = "кол. слов= " + builder.ToString()+"\n";
                        Console.WriteLine(DateTime.Now.ToShortTimeString() + ": " + builder.ToString());
                        data = Encoding.Unicode.GetBytes(message + "\n Слова: " + sen);
                        handler.Send(data);
                        handler.Shutdown(SocketShutdown.Both);
                        handler.Close();
                    }
                    else err = ("Число введено неверно");
                    data = Encoding.Unicode.GetBytes(err);
                    handler.Send(data);

                    // закрываем сокет
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}