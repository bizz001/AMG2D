using System;

namespace AMG2D.Configuration.Enum
{
    public enum EGameObjectType
    {
        Unknown = 0,
        /// <summary>
        /// Represents air tiles.
        /// </summary>
        AirTile,

        /// <summary>
        /// Represents ground tiles.
        /// </summary>
        GroundTile,

        /// <summary>
        /// Represents platform tiles.
        /// </summary>
        PlatformTile,

        /// <summary>
        /// Represents cave tiles.
        /// </summary>
        CaveTile,

        ExternalObject
    }
}
