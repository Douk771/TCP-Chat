using CommonLib;
using MVVMMiniLib;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace WPFChatClient.ViewModels
{
    class MainViewModel : ObservableObject
    {
        private string _IP;
        /// <summary>
        /// 
        /// </summary>
        public string IP
        {
            get => _IP;
            set  {
                OnPropertyChanged(ref _IP, value);
                isEnableBtnConnection = true;
            }
        }

        private int _Port;
        /// <summary>
        /// 
        /// </summary>
        public int Port
        {
            get => _Port;
            set => OnPropertyChanged(ref _Port, value);
        }

        private string _Chat;
        /// <summary>
        /// 
        /// </summary>
        public string Chat
        {
            get => _Chat;
            set => OnPropertyChanged(ref _Chat, value);
        }


        private string _Message;
        /// <summary>
        /// 
        /// </summary>
        public string Message
        {
            get => _Message;
            set => OnPropertyChanged(ref _Message, value);
        }


        private bool _isEnableBtnConnection;
        /// <summary>
        /// 
        /// </summary>
        public bool isEnableBtnConnection
        {
            get => _isEnableBtnConnection;
            set => OnPropertyChanged(ref _isEnableBtnConnection, value);
        }

        public ICommand ConnectCmd
        {
            get => new CommandBase(x => Connect(), true);
        }

        public ICommand ShowLogCmd
        {
            get => new CommandBase(x =>
            {
                Message = "showlog";
                Send();
            }, true);
        }

        public ICommand SendCmd
        {
            get => new CommandBase(x => Send(), true);
        }

        TcpClient client;
        StreamReader sr;
        StreamWriter sw;

        public MainViewModel()
        {
            IP = Dns.GetHostEntry(Dns.GetHostName()).AddressList[1].ToString();
            Port = 1975;
            isEnableBtnConnection = false;

            ReadStream();
            Connect();
        }

        public void Send ()
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    if (Message?.Length != 0)
                    {
                        if (Message.ToLower() == "showlog")
                            Chat = "";
                        sw.WriteLine($"{Message}");
                        Message = "";

                    }
                }
                catch (NullReferenceException)
                {
                    isEnableBtnConnection = true;
                    MessageBox.Show("Не верный IP адресс сервера");
                }
            });
        }

        public void Connect()
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    client = new TcpClient();
                    client.Connect(IP, Port);
                    sr = new StreamReader(client.GetStream());
                    sw = new StreamWriter(client.GetStream());
                    sw.AutoFlush = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            });
        }

        public void ReadStream()
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    try
                    {
                        if (client?.Connected == true)
                        {
                            var line = sr.ReadLine();

                            if (line != null)
                            {
                                Chat += line + "\n";
                            }
                            else
                            {
                                client.Close();
                                Chat += "ConnectedError" + "\n";
                                isEnableBtnConnection = true;
                            }
                        }
                        Task.Delay(10).Wait();
                    }
                    catch (Exception) { }
                }
            });
        }

    }


}
