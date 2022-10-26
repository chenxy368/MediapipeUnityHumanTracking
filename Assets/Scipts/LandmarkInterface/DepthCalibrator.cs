namespace LandmarkInterface
{
    /// <summary>
    /// This class is for the prediction of wrist's z coordinate which cannot
    /// detected by the camera and Mediapipr.
    /// </summary>
    /// </remark>
    /// Predict actual distance based on measured length with formula z = m/s + c
    /// where z is the z coordinate of wrist and s is the scale factor. Ideally, s
    /// increases as hand getting closer to the camera. However, it is not a linear
    /// relationship. Wait for latter improvement.
    /// measured parameters: m:-0.0719f, c:0.439f
    public class DepthCalibrator
    {
        private float m;
        private float c;

        public DepthCalibrator(float m, float c)
        {
            this.m = m;
            this.c = c;
        }

        public float GetDepthFromThumbLength(float length)
        {
            if (length == 0)
                return 0;
            return m / length + c;
        }
    }

}
