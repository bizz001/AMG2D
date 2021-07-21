using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditorInternal;

namespace AMG2D.Configuration
{
    [Serializable]
    public class BackgroundConfig
    {
        public float VerticalParallaxModifier = 1;

        public float HorizonHeight = 0.5f;

        public int MapPadding;

        public BackgroundLayerConfig[] BackgroundLayers;

        [Serializable]   
        public class BackgroundLayerConfig
        {
            public GameObject BaseImage;

            public float ParallaxIntensity;

            public int Repetition;
        }
    }
}
