using UnityEngine;

namespace LandmarkInterface
{
    /// <summary>
    /// This class introduce a property for the control points of the model.
    /// Help to find the GameObject.
    /// </summary>
    public class TransformLink : MonoBehaviour
    {
        /// <summary>
        /// Transform property of the control points
        /// </summary>
        public Transform Target;

        /// <summary>
        /// Update control points' position and rotation
        /// </summary>
        public void UpdateTransform()
        {
            if (Target == null)
                return;
            Target.SetPositionAndRotation(this.transform.position, this.transform.rotation);
        }
    }

}