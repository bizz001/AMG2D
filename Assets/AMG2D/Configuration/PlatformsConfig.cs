using System;

namespace AMG2D.Configuration
{
    /// <summary>
    /// Class that holds all the information required to generate the platforms.
    /// </summary>
    [Serializable]
    public class PlatformsConfig
    {
        /// <summary>
        /// Indicates the maximum height, in tiles, of the platforms.
        /// </summary>
        public int MaximumHeight;

        /// <summary>
        /// Indicates the minimum height, in tiles, of the platform.
        /// </summary>
        public int MinimumHeight;

        /// <summary>
        /// Indicates the thickness of the platforms. Tiles.
        /// </summary>
        public int Thickness;

        /// <summary>
        /// Indicates the density of the generated platforms.
        /// </summary>
        public int Density;

        /// <summary>
        /// Indicates the minimum width of the generated platforms.
        /// </summary>
        public int MinWidth;

        /// <summary>
        /// Indicates the maximum width of the generated platforms. 
        /// </summary>
        public int MaxWidth;

    }
}