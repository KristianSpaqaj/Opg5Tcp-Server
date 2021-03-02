using BeerClassLibrary;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Obl5TCPServer
{
    public class Server
    {
        static TcpListener serverSocket = new TcpListener(IPAddress.Loopback, 4646);
        static List<Beer> Data = new List<Beer>()
        {
            new Beer(1,"Tuborg",12.95,4.5),
            new Beer(2,"Carlsberg",13.5,5),
            new Beer(3,"Dansk Pilsner",5,4.3)
        };

        public static void Start()
        {
            serverSocket.Start();
            Console.WriteLine("Server has started");
            while (true)
            {
                TcpClient connectionSocket = serverSocket.AcceptTcpClient();
                Console.WriteLine("Connection established");
                new Thread(()=> Server.ClientLoop(connectionSocket)).Start();
            }
            serverSocket.Stop();
        }

        public static void ClientLoop(TcpClient socket)
        {
            Stream ns = socket.GetStream();
            StreamReader sr = new StreamReader(ns);
            StreamWriter sw = new StreamWriter(ns);
            sw.AutoFlush = true;


            string function = sr.ReadLine();
            string data = sr.ReadLine();
            if (function == "HentAlle")
            {
                sw.WriteLine(JsonConvert.SerializeObject(GetAll()));

            }
            else if (function == "Hent")
            {
                int id = int.Parse(data);
                sw.WriteLine(JsonConvert.SerializeObject(GetId(id)));
            }

            else if (function == "Gem")
            {
                Beer b = JsonConvert.DeserializeObject<Beer>(data);
                Add(b);
            }

            ns.Close();
        }

        public static List<Beer> GetAll()
        {
            return Data;
        }

        public static Beer GetId(int id)
        {
            return Data.Find(x => x.Id == id);
        }

        public static Beer Add(Beer value)
        {
            Data.Add(value);
            return value;
        }
    }
}
