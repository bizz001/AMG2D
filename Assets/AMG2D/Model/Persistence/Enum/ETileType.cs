using System;

namespace AMG2D.Model.Persistence
{
    /// <summary>
    /// Specifies all possible tile types that can be generated.
    /// </summary>
    public enum ETileType
    {
        /// <summary>
        /// Represents air tiles.
        /// </summary>
        Air = 0,

        /// <summary>
        /// Represents ground tiles.
        /// </summary>
        Ground,

        /// <summary>
        /// Represents grass tiles.
        /// </summary>
        Grass,

        /// <summary>
        /// Represents grass tiles.
        /// </summary>
        Stone,

        /// <summary>
        /// Represents platform tiles.
        /// </summary>
        Platform,

        /// <summary>
        /// Represents cave tiles.
        /// </summary>
        Cave
    }
}
