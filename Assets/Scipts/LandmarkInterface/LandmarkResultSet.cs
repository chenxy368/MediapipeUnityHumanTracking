using LandmarkInterface.Filter;
using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using UnityEngine;

namespace LandmarkInterface
{
    public class LandmarkResultSet : MonoBehaviour
    {
        public bool UseFilter = true;

        [Range(0.0f, 1.0f)] public double FilterTimeInterval = 0.4;
        [Range(0.0f, 1.0f)] public double FilterNoise = 0.1;
        [Header("Filter extreme data exceeding the limit")]
        [Range(0.0f, 1.0f)] public double DisplacementLimit = 0.4;

        private LandmarkListFilter LeftHandFilter;
        private LandmarkListFilter RightHandFilter;

        private List<Vector3> leftHandLandmarks;
        public List<Vector3> LeftHandLandmarks
        {
            get => leftHandLandmarks;
            set
            {
                if (UseFilter)
                {
                    LeftHandFilter.CorrectAndPredict(value);
                }
                leftHandLandmarks = value;
            }
        }

        private List<Vector3> rightHandLandmarks;
        public List<Vector3> RightHandLandmarks
        {
            get => rightHandLandmarks;
            set
            {
                if (UseFilter)
                {
                    RightHandFilter.CorrectAndPredict(value);
                }
                rightHandLandmarks = value;
            }
        }

        //private UDPReceive udpReceive;
        private Client client;
        private void Start() {
            //udpReceive = GameObject.Find("Manager").GetComponent<UDPReceive>();
            client = GameObject.Find("Manager").GetComponent<Client>();
        }

        private void Awake()
        {
            LeftHandFilter = new LandmarkListFilter(FilterTimeInterval, FilterNoise, DisplacementLimit);
            RightHandFilter = new LandmarkListFilter(FilterTimeInterval, FilterNoise, DisplacementLimit);
        }

        private void Update()
        {
            LeftHandFilter.UpdateFilterParameter(FilterTimeInterval, FilterNoise, DisplacementLimit);
            RightHandFilter.UpdateFilterParameter(FilterTimeInterval, FilterNoise, DisplacementLimit);
        }

        public void UpdateLandmark(LandmarkType landmarkType)
        {   
            
            if (landmarkType == LandmarkType.LeftHand)
            {
                //if (!String.IsNullOrEmpty(udpReceive.rightdata)) 
                if (!String.IsNullOrEmpty(client.rightdata)) 
                {
                    //string[] points = udpReceive.rightdata.Split(',');
                    string[] points = client.rightdata.Split(',');
                    List<Vector3> output = new List<Vector3>();
                    for (int i = 0; i < 21; ++i) {
                        float x = float.Parse(points[3 * i]);
                        float y = float.Parse(points[3 * i + 1]);
                        float z = float.Parse(points[3 * i + 2]);
                                
                        var v = new Vector3(x, y, z);
                        output.Add(v);
                    }

                    LeftHandLandmarks = output; 
                }
            }
            else if (landmarkType == LandmarkType.RightHand)
            {
                //if (!String.IsNullOrEmpty(udpReceive.leftdata))
                if (!String.IsNullOrEmpty(client.leftdata))
                {
                    //string[] points = udpReceive.leftdata.Split(',');
                    string[] points = client.leftdata.Split(',');
                    List<Vector3> output = new List<Vector3>();
                    for (int i = 0; i < 21; ++i) {
                        float x = float.Parse(points[3 * i]);
                        float y = float.Parse(points[3 * i + 1]);
                        float z = float.Parse(points[3 * i + 2]);
                                
                        var v = new Vector3(x, y, z);
                        output.Add(v);
                    }
                
                    RightHandLandmarks = output; 
                }
            }
            else
            {
                Debug.LogError("Not Implemented");
            }
        }

        public List<Vector3> GetLandmarks(LandmarkType landmarkType)
        {
            if (UseFilter)
            {
                if (landmarkType == LandmarkType.LeftHand)
                {
                    return LeftHandFilter.GetPositions();
                }
                else if (landmarkType == LandmarkType.RightHand)
                {
                    return RightHandFilter.GetPositions();
                }
            }
            else
            {
                if (landmarkType == LandmarkType.LeftHand)
                {
                    return LeftHandLandmarks != null ? LeftHandLandmarks : null;
                }
                else if (landmarkType == LandmarkType.RightHand)
                {
                    return RightHandLandmarks != null ? RightHandLandmarks : null;
                }
            }
            return null;
        }

    }

}