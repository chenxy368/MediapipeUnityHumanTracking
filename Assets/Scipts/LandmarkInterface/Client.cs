using NetMQ;
using System;
using System.Collections.Concurrent;
using UnityEngine;

namespace LandmarkInterface
{
    /// <summary>
    /// This class gets data from a reveiver, processes the data
    /// and store the data in leftdata and rightdata.
    /// </summary>
    public class Client : MonoBehaviour
    {
        /// <summary>
        /// A thread-safe first in-first out (FIFO) collection for
        /// store the action corresponding to each data from the message queue.
        /// </summary>
        private readonly ConcurrentQueue<Action> runOnMainThread = new ConcurrentQueue<Action>();
        private ReceiverOneway receiver;

        /// <summary>
        /// The raw left hand relative position data. 
        /// </summary>
        public string leftdata;

        /// <summary>
        /// The raw right hand relative position data. 
        /// </summary>
        public string rightdata;

        /// <summary>
        /// This is the start function for this MonoBehaviour class, which initialize the 
        /// receiver and registrate the callback function.
        /// </summary>
        /// <remarks>
        /// With every call of callback function, the runOnMainThread enqueue a new action
        /// for data processing. In ReceiverOneway class, the callback function is called
        /// after getting one data from python program.
        /// <see cref="ReceiverOneway"/>
        /// <see cref="MeidaPipeData"/>
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

        /// <summary>
        /// This is the update function for this MonoBehaviour class. It checks whether
        /// there are remaining action in the runOnMainThread. It invoke all actions in the
        /// queue, which corresponding to all the data that have not been processed.
        /// </summary>
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

        /// <summary>
        /// This method stop the receiver and clean up the message queue.
        /// </summary>
        private void OnDestroy()
        {
            receiver.Stop();
            NetMQConfig.Cleanup();  // Must be here to work more than once
        }
    }
}

