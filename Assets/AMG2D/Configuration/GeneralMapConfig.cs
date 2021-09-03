using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace AMG2D.Configuration
{
    /// <summary>
    /// Class that holds general map configuration parameters.
    /// </summary>
    [Serializable]
    public class GeneralMapConfig
    {
        /// <summary>
        /// Aspects collection for each type of tile.
        /// </summary>
        [SerializeReference]
        public Dictionary<EGameObjectType, GameObject> Aspects;

        /// <summary>
        /// Height of the generated map.
        /// </summary>
        public int Height;

        /// <summary>
        /// Width of the generated map.
        /// </summary>
        public int Width;

        /// <summary>
        /// Thickness of the map border. If set to 0 the map will have no border.
        /// </summary>
        public int MapBorderThickness;

        /// <summary>
        /// <see cref="TileBase"/> objects reference that will be used to complete ground tiles.
        /// </summary>
        [SerializeReference]
        public TileBase GroundTile;

        /// <summary>
        /// <see cref="TileBase"/> objects reference that will be used to complete ground tiles.
        /// </summary>
        [SerializeReference]
        public TileBase PlatformTile;

        /// <summary>
        /// Generation seed of the entire map. Each seed will generate a deterministic random map.
        /// </summary>
        public int GenerationSeed;

        /// <summary>
        /// Object seeds for the type specified by the key of the <see cref="Dictionary{TKey, TValue}"/>.
        /// </summary>
        public Dictionary<string, GameObject> ObjectSeeds;

        /// <summary>
        /// Camera object for movement tracking;
        /// </summary>
        public GameObject Camera;

        /// <summary>
        /// Indicates whether map segmentation should be enabled.
        /// </summary>
        public bool EnableSegmentation;

        /// <summary>
        /// Size in tiles of each individual segment.
        /// </summary>
        public int SegmentSize;

        /// <summary>
        /// Number of segments that will compose the map.
        /// </summary>
        public int NumberOfSegments;

        /// <summary>
        /// Speed at which each individual segment is loaded.
        /// Must be a positive number. Setting this value to a number greater than <see cref="SegmentSize"/> will cause segments to load instantly. Might affect performance.
        /// </summary>
        public int SegmentLoadingSpeed;

        /// <summary>
        /// List of external objects that will be set to active once the map is finised loading.
        /// </summary>
        public List<GameObject> ObjectsToEnable;
    }
}
