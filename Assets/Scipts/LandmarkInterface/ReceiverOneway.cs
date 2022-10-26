using NetMQ;
using NetMQ.Sockets;
using System;
using System.Threading;
using UnityEngine;

/// <summary>
/// This class gets data from message queue(ZeroMQ) in one thread.
/// </summary>
public class ReceiverOneway
{
    /// <summary>
    /// This is the data receiver thread.
    /// </summary>
    private readonly Thread receiveThread;

    /// <summary>
    /// This if a running flag of receiveThread.
    /// </summary>
    private bool running;

    /// <summary>
    /// This method registrate the receiveThread's callback function and set the running to true.
    /// </summary>
    /// <param name="callback">The call back function of receiveThread.</param>
    public void Start(Action<MeidaPipeData> callback)
    {
        running = true;
        receiveThread.Start(callback);
    }

    /// <summary>
    /// This method destroy the receiveThread and set the running to false.
    /// </summary>
    public void Stop()
    {
        running = false;
        receiveThread.Join();
    }

    /// <summary>
    /// This method initailze the receiveThread and run the thread while flag is true. 
    /// </summary>
    /// <remarks>
    /// The receiveThread use ZeroMq to communicate with the python program and convert
    /// the data from string to Json that defined in MediaPipeData. This method then
    /// uses the callback function to process the Json data.
    /// <see cref="MeidaPipeData"/>
    public ReceiverOneway()
    {
        receiveThread = new Thread((object callback) => 
        {
            using (var socket = new PullSocket())  // <- The PULL socket
            {
                socket.Connect("tcp://localhost:5557");

                while (running)
                {
                    string message = socket.ReceiveFrameString();
                    MeidaPipeData data = JsonUtility.FromJson<MeidaPipeData>(message);
                    ((Action<MeidaPipeData>)callback)(data);
                }
            }
        });
    }
}
    


