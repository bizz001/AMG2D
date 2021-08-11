using System;
using UnityEngine;

namespace AMG2D.Configuration
{
    /// <summary>
    /// Class that holds information regarding background configuration.
    /// </summary>
    [Serializable]
    public class BackgroundConfig
    {
        /// <summary>
        /// Value by which the parallax intensity of each layer is multiplied in order to apply vertical parallax.
        /// Recommended values between <see cref="1.0f"/> and <see cref="0.1f"/>
        /// Default value: <see cref="1f"/>
        /// </summary>
        public float VerticalParallaxModifier = 1;

        /// <summary>
        /// Height of the horizon as a proportion of the total height.
        /// <see cref="0.5f"/> will set the horizon in the middle of the map height.
        /// </summary>
        public float HorizonHeight = 0.5f;

        /// <summary>
        /// Vertical map padding relative to the map size.
        /// </summary>
        public int MapPadding;

        /// <summary>
        /// Array containing the background configuration of each parallax layer.
        /// </summary>
        public BackgroundLayerConfig[] BackgroundLayers;

        /// <summary>
        /// Class containing background configuration of individual layer.
        /// </summary>
        [Serializable]   
        public class BackgroundLayerConfig
        {
            /// <summary>
            /// <see cref="GameObject"/> instance representing the image to be set as layer.
            /// </summary>
            public GameObject BaseImage;

            /// <summary>
            /// The intensity of the parallax effect. Must be a value between <see cref="-1f"/> and <see cref="1f"/>
            /// <see cref="0f"/> means the layer will be static relative to the map.
            /// <see cref="1f"/> means the layer will be static relative to the camera.
            /// values less than <see cref="0"/> means the layer will be in front of the map.
            /// </summary>
            public float ParallaxIntensity;

            /// <summary>
            /// Number of times to repeat the layer in order to cover all camera view. Increase this value if you see the edge of the background when moving.
            /// </summary>
            public int Repetition;
        }
    }
}
