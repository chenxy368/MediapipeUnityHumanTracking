using NetMQ;
using NetMQ.Sockets;
using System;
using System.Threading;
using System.Collections.Concurrent;
using UnityEngine;


namespace LandmarkInterface
{
    public class Client : MonoBehaviour
    {
        private readonly ConcurrentQueue<Action> runOnMainThread = new ConcurrentQueue<Action>();
        private ReceiverOneway receiver;

        public string leftdata;
        public string rightdata;

        public void Start()
        {
            receiver = new ReceiverOneway();
            receiver.Start((MeidaPipeData d) => runOnMainThread.Enqueue(() =>
                {
                    string data = d.str;
                    
                    if (!String.IsNullOrEmpty(data)) 
                    {
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
                }
            ));
        }

        public void Update()
        {
            if (!runOnMainThread.IsEmpty)
            {
                Action action;
                while (runOnMainThread.TryDequeue(out action))
                {
                    action.Invoke();
                }
            }
        }

        private void OnDestroy()
        {
            receiver.Stop();
            NetMQConfig.Cleanup();  // Must be here to work more than once
        }
    }
}

