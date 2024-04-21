using System;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    internal class Client
    {
        Socket _client;
        Server _server;

        public Client(Socket socket, Server server)
        {
            _client = socket;
            _server = server;
        }

        public void ReadData()
        {
            try
            {
                while (true)
                {
                    byte[] data = new byte[_client.ReceiveBufferSize];
                    int bytesRead = _client.Receive(data);
                    if (bytesRead == 0)
                    {
                        break;
                    }
                    byte[] encryptedData = new byte[bytesRead];
                    Array.Copy(data, encryptedData, bytesRead);
                    string response = _server.DecryptCommand(encryptedData);
                    Console.WriteLine($"\nReceived response from client: {response}");
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"Socket exception: {ex.Message}");
            }
            finally
            {
                _server.Clients.Remove(_client);
                _client.Close();
            }
        }
    }
}