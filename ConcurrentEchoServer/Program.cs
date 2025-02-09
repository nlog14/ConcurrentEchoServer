﻿using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ConcurrentEchoServer
{
    class Program
    {
        static void Main(string[] args)
        {
            //Writing to the console to be able to differentiate what is running
            Console.WriteLine("Server:");

            //Creates a listener able to listen for connections incoming on port 7 only on the loopback adapter
            //loopback adapter is only used for connections on the local machine
            TcpListener listener = new TcpListener(IPAddress.Loopback, 7);

            //Actually starts the listener
            listener.Start();

            //In order for the server to be able to handle more than one client a while loop is needed.
            //here it is while true, because we don't have something that tells it to stop
            while (true)
            {
                //Here the code will wait, until a client connects and then returns an instance of the TcpClient class
                TcpClient socket = listener.AcceptTcpClient();

                //Because TCP holds the connection open, in order to handle several clients at the same time
                //it starts a new thread for each client
                //instead of having all the code here, it is moved (refactored) to a seperate method HandleClient
                Task.Run(() => HandleClient(socket));

                //the while loop ends here, after the server closes the socket connected to the client
                //but before the server stops listening.
            }

            //this line will never be reached because of the while loop
            //instead it will only stop when the program is stopped
            listener.Stop();
        }

        //New method to keep the code more maintenance friendly
        public static void HandleClient(TcpClient socket)
        {
            //Gets the stream object from the socket. The stream object is able to recieve and send data
            NetworkStream ns = socket.GetStream();
            //The StreamReader is an easier way to read data from a Stream, it uses the NetworkStream
            StreamReader reader = new StreamReader(ns);
            //The StreamWriter is an easier way to write data to a Stream, it uses the NetworkStream
            StreamWriter writer = new StreamWriter(ns);

            //Here it reads all data send until a line break (cr lf) is received; notice the Line part of the ReadLine
            string message = reader.ReadLine();
            //Here it writes the received data to the Console
            //this is only for testing purposes, to verify that the server recieves the data
            Console.WriteLine(message);
            //Here it writes the data back to the client and appends a line break (cr lf); notice the line part of WriteLine
            writer.WriteLine(message);
            //Makes sure that the message isn't buffered somewhere, and actually send to the client
            //Always remember to flush after you
            writer.Flush();

            //Because it doesn't expect more messages from the client, it closes the socket/connection
            socket.Close();
        }
    }
}
