using NetMQ;
using NetMQ.Sockets;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Text;
using UnityEngine;
using UnityEngine.UI;


public class ReceiverOneway
{
    private readonly Thread receiveThread;
    private bool running;

    public void Start(Action<MeidaPipeData> callback)
    {
        running = true;
        receiveThread.Start(callback);
    }

    public void Stop()
    {
        running = false;
        receiveThread.Join();
    }

    public ReceiverOneway()
    {
        receiveThread = new Thread((object callback) => 
        {
            using (var socket = new PullSocket())  // <- The PULL socket
            {
                socket.Connect("tcp://localhost:5555");

                while (running)
                {
                    // No socket.SendFrameEmpty(); here
                    string message = socket.ReceiveFrameString();
                    MeidaPipeData data = JsonUtility.FromJson<MeidaPipeData>(message);
                    ((Action<MeidaPipeData>)callback)(data);
                }
            }
        });
    }
}
    


