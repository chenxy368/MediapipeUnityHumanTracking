using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;


namespace LandmarkInterface
{
    public class UDPReceive : MonoBehaviour
    {
        Thread receiveThread;
        UdpClient client;
        public int port = 5052;
        public bool startRecieving = true;
        public bool printToConsole = true;
        public string data;
        public string leftdata;
        public string rightdata;

        public void Start()
        {
            receiveThread = new Thread(new ThreadStart(ReceiveData));
            receiveThread.IsBackground = true;
            receiveThread.Start();
        }

        // Update is called once per frame
        private void ReceiveData()
        {
            client = new UdpClient(port);
            while (startRecieving) {
                try {
                    IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                    byte[] dataByte = client.Receive(ref anyIP);
                    data = Encoding.UTF8.GetString(dataByte);

                    if (printToConsole) Console.WriteLine(data);
                    if (!String.IsNullOrEmpty(data)) 
                    {
                        Console.WriteLine("a");
                        string[] twohanddata = data.Split('+');
                    

                        leftdata = twohanddata[0].Remove(0, 1);
                        leftdata = leftdata.Remove(leftdata.Length - 1, 1);           

                        rightdata = twohanddata[1].Remove(0, 1);
                        rightdata = rightdata.Remove(rightdata.Length - 1, 1);           
                    } 
                    else
                    {
                        leftdata = "";
                        rightdata = "";
                    }
                    

                } catch (Exception e) {
                    Console.WriteLine(e.ToString());
                }
            }
        }
    }
}

