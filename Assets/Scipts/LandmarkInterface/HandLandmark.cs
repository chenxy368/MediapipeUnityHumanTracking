using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LandmarkInterface
{
    /// <summary>
    /// This class uses the original data to control the model's hands.
    /// Since the IK model is already setted, it controls the wrist and finger tips. 
    /// </summary>
    public class HandLandmark : MonoBehaviour
    {
        /// <summary>
        /// Original data from Mediapipe, it stores in LandmarkResultSet class.
        /// <see cref="LandmarkResultSet"/>
        /// </summary>
        public LandmarkResultSet resultSet;

        /// <summary>
        /// All 21 landmarks of each hand. Notice that although only finger tips 
        /// and wrist are needed, it also stores the other landmarks for debuging purpose. 
        /// </summary>
        public List<GameObject> landmarkObjects;

        /// <summary>
        /// All 20 rigging bones of each hand corresponding to landmarks on the 
        /// figure mesh model. Use it to let the landmark position fit into mesh model.
        /// </summary>
        public List<GameObject> landmarkOnMesh;

        /// <summary>
        /// Mark this hand left or right.
        /// </summary>
        public LandmarkType LandmarkType;

        [SerializeField]
        /// <summary>
        /// The length of the model's thumb. Compare thumb length with landmarks to check
        /// current scale of landmark.
        /// </summary>
        private float thumbModelLength = 0.03f;

        /// <summary>
        /// Current landmarks' scale. Predict the wrist's z coordinate with it.
        /// </summary>
        private float scale;

        /// <summary>
        /// Since the Mediapipe cannot get z coordinate(distance between hand and camera)
        /// of wrist, use the depthCalibrator to predict it.
        /// <see cref="DepthCalibrator"/>
        /// </summary>
        private DepthCalibrator depthCalibrator = new DepthCalibrator(-0.0719f, 0.439f);
        
        /// <summary>
        /// Five fingertips' control point transform properties. Set them to control the model.
        /// </summary>
        private TransformLink[] transformLinkers;

        /// <summary>
        /// Length of all 20 rigging bones of each hand corresponding to landmarks on the 
        /// figure mesh model. Use it to let the landmark position fit into mesh model.
        /// </summary>
        private float[] boneLengths;

        /// <summary>
        /// Debug mode flag
        /// </summary>
        private bool debugMode = true;

        /// <summary>
        /// Initialize transformLinkers and boneLengths at very begining.
        /// Notice that other fields are initialized in the scene.
        /// </summary>
        private void Start()
        {
            // Properties TransformLink are added to fingertips' point 4, 8, 12, 16, 20
            transformLinkers = this.transform.GetComponentsInChildren<TransformLink>();
            boneLengths = new float[20];
            for (int i = 0; i < 20; i++) 
            {
                boneLengths[i] = landmarkOnMesh[i].transform.localPosition.magnitude;
            }
        }

        /// <summary>
        /// Get updated landmarks input and update the position and rotation of 
        /// models' control points.
        /// </summary>
        private void Update()
        {
            resultSet.UpdateLandmark(this.LandmarkType);

            var list = resultSet.GetLandmarks(this.LandmarkType);
            if (list != null && list.Count != 0)
            {
                updateLandmarkPosition(list);
                updateLandmarkScale();
            }

            updateWristRotation();
            foreach (var linker in transformLinkers)
            {
                linker.UpdateTransform();
            }

        }

        /// <summary>
        /// Update 21 landmarks' position. Wrist point(point 0) is absolute position
        /// while the other 20 points are related position to the wrist.
        /// </summary>
        /// <param name="landmarks">Landmark input.</param>
        private void updateLandmarkPosition(List<Vector3> landmarks)
        {
            var offset = landmarks[0];
            for (int i = 1; i < landmarks.Count; i++)
            {
                landmarkObjects[i].transform.localPosition = landmarks[i] - offset;
            }

            var thumbDetectedLength = Vector3.Distance(landmarks[0], landmarks[1]);
            if (thumbDetectedLength == 0)
                return;
            scale = thumbModelLength / thumbDetectedLength;

            float depth = depthCalibrator.GetDepthFromThumbLength(scale);
            this.transform.localPosition = new Vector3(offset.x, offset.y, depth * scale);
        }

        /// <summary>
        /// Calibrate the landmark position to let it fit into the model's corresponding
        /// parts. Basically, let each bone has same length with model or computing by landmarks.
        /// </summary>
        private void updateLandmarkScale()
        {
            Vector3[] tmp = new Vector3[21];
            for (int i = 0; i < landmarkObjects.Count; i++) {
                tmp[i] = landmarkObjects[i].transform.localPosition;
            }

            for (int i = 1; i < landmarkObjects.Count; i++)
            {
                Vector3 direction;
                Vector3 correctPos;
                if (i == 1 || i == 5 || i == 9 || i == 13|| i == 17) 
                {
                    direction = Vector3.Normalize(tmp[i]);
                    correctPos = direction * boneLengths[i-1] + tmp[0];
                }
                else 
                {
                    direction = Vector3.Normalize(tmp[i] - tmp[i-1]);
                    correctPos = direction * boneLengths[i-1] + landmarkObjects[i-1].transform.localPosition;
                }
                
                landmarkObjects[i].transform.localPosition = correctPos;
            }
        }

        /// <summary>
        /// Calculate wrist's rotation by cross product of two vectors on palm.
        /// </summary>
        private void updateWristRotation()
        {
            var wristTransform = landmarkObjects[0].transform;
            var indexFinger = landmarkObjects[5].transform.position;
            var middleFinger = landmarkObjects[9].transform.position;

            var vectorToMiddle = middleFinger - wristTransform.position;
            var vectorToIndex = indexFinger - wristTransform.position;
            Vector3.OrthoNormalize(ref vectorToMiddle, ref vectorToIndex);

            Vector3 normalVector = Vector3.Cross(vectorToIndex, vectorToMiddle);
            wristTransform.rotation = Quaternion.LookRotation(normalVector, vectorToIndex);
        }

        /// <summary>
        /// For debugging purpose, draw a red sphere centered at the landmarks' position if
        /// enable the debugging mode.
        /// </summary>
        private void OnDrawGizmos()
        {
            if (!debugMode) return;
            Gizmos.color = Color.red;
            for (int i = 0; i < landmarkObjects.Count; i++)
            {
                Gizmos.DrawSphere(landmarkObjects[i].transform.position, 0.005f);
            }
        }
    }
}