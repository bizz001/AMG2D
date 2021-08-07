using System;
using System.Collections.Generic;
using AMG2D.Configuration.Enum;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace AMG2D.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class GeneralMapConfig
    {
        public int Height;

        public int Width;

        [SerializeReference]
        public TileBase GroundTile;

        [SerializeReference]
        public TileBase PlatformTile;

        public Dictionary<EGameObjectType, GameObject> Aspects;

        public int GenerationSeed;

        public int MapBorderThickness;

        public Dictionary<string, GameObject> ObjectSeeds;

        [SerializeReference]
        public BackgroundConfig Background = new BackgroundConfig();

        [SerializeReference]
        public GroundConfig Ground = new GroundConfig();

        [SerializeReference]
        public PlatformsConfig Platforms = new PlatformsConfig();

        [SerializeReference]
        public ExternalObjectsConfig ExternalObjects = new ExternalObjectsConfig();

        /// <summary>
        /// Camera object for movement tracking;
        /// </summary>
        public GameObject Camera;

        public bool EnableSegmentation;

        public int SegmentSize;

        public int NumberOfSegments;

        public int SegmentLoadingSpeed;

        public List<GameObject> ObjectsToEnable;

    }
}
