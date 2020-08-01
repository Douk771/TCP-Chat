using WPFChatClient.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ChatServer
{
    class Program
    {
        static TcpListener listener = new TcpListener(IPAddress.Any, 1975);
        static List<ConnectedClient> clients = new List<ConnectedClient>();

        static void Main(string[] args)
        {
            try
            {
                listener.Start();
            }
            catch (SocketException)
            {
                Environment.Exit(0);
            }
            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                Task.Factory.StartNew(() =>
                {
                    StreamReader sr = new StreamReader(client.GetStream());
                    while (client.Connected)
                    {
                        clients.Add(new ConnectedClient(client));
                        break;
                    }

                    while (client.Connected)
                    {
                        List<string> linesLog = new List<string>();
                        string line = sr.ReadLine();
                        Console.WriteLine(line);
                        try
                        {
                            linesLog = File.ReadAllLines("Log.txt").ToList();
                        }
                        catch (FileNotFoundException) { }
                        if (line.ToLower() != "showlog")
                        {
                            linesLog.Add(line);
                            if (linesLog.Count != 1)
                                linesLog.Sort();

                            File.WriteAllLines("Log.txt", linesLog);
                            SendToAllClients(line);
                        }
                        else
                        {
                            ShowLogForOneClient(client, linesLog);
                        }
                    }
                });
            }
        }

        static async void ShowLogForOneClient(TcpClient client, List<string> lineLog)
        {
            await Task.Factory.StartNew(() =>
            {
                if (client.Connected)
                {
                    if (lineLog.Count != 0)
                    {
                        try
                        {
                            StreamWriter sw = new StreamWriter(client.GetStream())
                            {
                                AutoFlush = true
                            };
                            sw.WriteLine("Server Log");
                            foreach (var x in lineLog)
                                sw.WriteLine(x);
                        }
                        catch { }
                    }
                    else
                    {
                        try
                        {
                            StreamWriter sw = new StreamWriter(client.GetStream())
                            {
                                AutoFlush = true
                            };
                            sw.WriteLine("Файл лога пуст или отсутствует!");
                        }
                        catch { }
                    }
                }
            });
        }

        static async void SendToAllClients(string message)
        {
            await Task.Factory.StartNew(() =>
            {
                for (int i = 0; i < clients.Count; i++)
                {
                    try
                    {
                        if (clients[i].Client.Connected)
                        {
                            StreamWriter sw = new StreamWriter(clients[i].Client.GetStream())
                            {
                                AutoFlush = true
                            };
                            sw.WriteLine(message);
                        }
                        else
                            clients.RemoveAt(i);
                    }
                    catch { }
                }
            });
        }
    }
}
