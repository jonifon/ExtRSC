using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Security.Cryptography;

namespace Server
{
    class Server
    {
        public EndPoint Ip; 
        public int Listen; 
        public bool Active; 
        private Socket _listener;
        private volatile CancellationTokenSource _cts;
        public Dictionary<Socket, Client> Clients = new Dictionary<Socket, Client>();

        // 16 bytes
        private readonly byte[] _key = Encoding.UTF8.GetBytes("YourSecretKey123");
        private readonly byte[] _iv = Encoding.UTF8.GetBytes("YourInitVector12");

        public Server(string ip, int port)
        {
            this.Listen = port;
            this.Ip = new IPEndPoint(IPAddress.Parse(ip), Listen);
            this._cts = new CancellationTokenSource();
            this._listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Start()
        {
            if (!Active)
            {
                _listener.Bind(Ip);
                _listener.Listen(16);
                Active = true;
            }
            else
            {
                Console.WriteLine("Сервер уже запущен.");
            }

            while (Active || !_cts.Token.IsCancellationRequested)
            {
                try
                {
                    Socket listenerAccept = _listener.Accept();
                    if (listenerAccept != null)
                    {
                        Task.Run(
                          () => ClientThread(listenerAccept),
                          _cts.Token
                        );
                    }
                }
                catch { }
            }
        }

        public void Stop()
        {
            if (Active)
            {
                _cts.Cancel();
                _listener.Close();  
                Active = false;
            } else
            {
                Console.WriteLine("Сервер уже остановлен.");
            }
        }
        public void ClientThread(Socket client)
        {
            Client clientObject = new Client(client, this);
            Clients.Add(client, clientObject);
            Task.Run(() => clientObject.ReadData());
        }

        public void SendCommand(Socket client, string command)
        {
            string encryptedCommand = EncryptCommand(command);
            byte[] encryptedData = Encoding.UTF8.GetBytes(encryptedCommand);
            client.Send(encryptedData);
        }

        public string DecryptCommand(byte[] encryptedData)
        {
            string encryptedCommand = Encoding.UTF8.GetString(encryptedData);
            byte[] cipherData = Convert.FromBase64String(encryptedCommand);
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = _key;
                aesAlg.IV = _iv;

                using (MemoryStream memoryStream = new MemoryStream(cipherData))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aesAlg.CreateDecryptor(), CryptoStreamMode.Read))
                    {
                        byte[] decryptedData = new byte[cipherData.Length];
                        int decryptedDataLength = cryptoStream.Read(decryptedData, 0, decryptedData.Length);
                        return Encoding.UTF8.GetString(decryptedData, 0, decryptedDataLength);
                    }
                }
            }
        }

        private string EncryptCommand(string command)
        {
            byte[] plainData = Encoding.UTF8.GetBytes(command);
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = _key;
                aesAlg.IV = _iv;

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aesAlg.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(plainData, 0, plainData.Length);
                        cryptoStream.FlushFinalBlock();
                        byte[] encryptedData = memoryStream.ToArray();
                        return Convert.ToBase64String(encryptedData);
                    }
                }
            }
        }

        public List<string> GetConnectedClients()
        {
            List<string> clients = new List<string>();
            foreach (var client in Clients.Keys)
            {
                string clientIP = ((IPEndPoint)client.RemoteEndPoint).Address.ToString();
                clients.Add(clientIP);
            }
            return clients;
        }
    }
}
