using System;

namespace AMG2D.Configuration
{
    /// <summary>
    /// Class holding configuration information used to generate the ground.
    /// </summary>
    [Serializable]
    public class GroundConfig
    {
        /// <summary>
        /// The starting height of the generated ground.
        /// </summary>
        public int InitialHeight;

        /// <summary>
        /// The smoothness of the ground generated.
        /// </summary>
        public float Smoothness;
    }
}
