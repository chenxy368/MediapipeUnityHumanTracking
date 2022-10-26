using System;
using System.Collections.Generic;
using UnityEngine;

namespace LandmarkInterface
{
    public class LandmarkResultSet : MonoBehaviour
    {
        /// <summary>
        /// Left hand Mediapipe normalized landmark list.
        /// </summary>
        private List<Vector3> leftHandLandmarks;

        /// <summary>
        /// Right hand Mediapipe normalized landmark list.
        /// </summary>
        private List<Vector3> rightHandLandmarks;

        /// <summary>
        /// ZeroMq receiving client
        /// <see cref="Client"/>
        /// </summary>
        private Client client;

        /// <summary>
        /// Initialize the client with the Client property of Manager
        /// </summary>
        private void Start() {
            client = GameObject.Find("Manager").GetComponent<Client>();
        }

        /// <summary>
        /// Update the landmark lists with the data from ZeroMq.
        /// </summary>
        /// <param name="landmarkType">Handedness.</param>
        public void UpdateLandmark(LandmarkType landmarkType)
        {   
            
            if (landmarkType == LandmarkType.LeftHand)
            {
                if (!String.IsNullOrEmpty(client.rightdata)) 
                {
                    string[] points = client.rightdata.Split(',');
                    List<Vector3> output = new List<Vector3>();
                    for (int i = 0; i < 21; ++i) {
                        float x = float.Parse(points[3 * i]);
                        float y = float.Parse(points[3 * i + 1]);
                        float z = float.Parse(points[3 * i + 2]);
                                
                        var v = new Vector3(x, y, z);
                        output.Add(v);
                    }

                    leftHandLandmarks = output; 
                }
            }
            else if (landmarkType == LandmarkType.RightHand)
            {
                if (!String.IsNullOrEmpty(client.leftdata))
                {
                    string[] points = client.leftdata.Split(',');
                    List<Vector3> output = new List<Vector3>();
                    for (int i = 0; i < 21; ++i) {
                        float x = float.Parse(points[3 * i]);
                        float y = float.Parse(points[3 * i + 1]);
                        float z = float.Parse(points[3 * i + 2]);
                                
                        var v = new Vector3(x, y, z);
                        output.Add(v);
                    }
                
                    rightHandLandmarks = output; 
                }
            }
            else
            {
                Debug.LogError("Not Implemented");
            }
        }

        /// <summary>
        /// Getter of the private landmark list.
        /// </summary>
        /// <param name="landmarkType">Handedness.</param>
        /// <returns>The landmark list corresponding the input handedness.</returns>
        public List<Vector3> GetLandmarks(LandmarkType landmarkType)
        {
            if (landmarkType == LandmarkType.LeftHand)
            {
                return leftHandLandmarks != null ? leftHandLandmarks : null;
            }
            else if (landmarkType == LandmarkType.RightHand)
            {
                return rightHandLandmarks != null ? rightHandLandmarks : null;
            }
            return null;
        }
    }

}