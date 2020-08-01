using System.Net.Sockets;

namespace WPFChatClient.Models
{
    class ConnectedClient
    {
        public TcpClient Client { get; set; }

        public ConnectedClient(TcpClient client)
        {
            Client = client;
        }
    }
}
