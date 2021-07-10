using System;
using AMG2D.Model.Persistence.Enum;
using UnityEngine;

namespace AMG2D.Model.Persistence
{
    /// <summary>
    /// Class designed for holding information of about specific tiles.
    /// To be used in the creating of Tile instances.
    /// </summary>
    public class TileInformation
    {
        private ETileType _tileType;
        /// <summary>
        /// Gets or sets the tile type. Atempting to change type of a tile while it is active will cause an InvalidOperationException to be thrown.
        /// </summary>
        public ETileType TileType
        {
            get { return _tileType; }
            set
            {
                if (IsActive) throw new InvalidOperationException("Cannot change type of a tile while it is active.");
                _tileType = value;
            }
        }

        /// <summary>
        /// Indicates whether this tile is active. Active tiles cannot be changed.
        /// </summary>
        public bool IsActive => CurrentPrefab != null;

        /// <summary>
        /// Gets the X component of this tile's position.
        /// </summary>
        public int X { get; }

        /// <summary>
        /// Gets the Y component of this tile's position.
        /// </summary>
        public int Y { get; }

        /// <summary>
        /// Gets the segment number asigned to this tile.
        /// </summary>
        public int SegmentNumber { get; }

        /// <summary>
        /// Gets or sets the current GameObject instance asinged to this tile.
        /// </summary>
        public GameObject CurrentPrefab { get; set; }

        /// <summary>
        /// Constructs a TileInformation instance at a specified (x, y) position.
        /// </summary>
        /// <param name="x">X component of tile position.</param>
        /// <param name="y">Y component of tile position.</param>
        public TileInformation(int x, int y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Constructs a TileInformation instance at a specified (x, y) position and with the specified segmentNumber.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="segmentNumber"></param>
        public TileInformation(int x, int y, int segmentNumber) : this(x, y)
        {
            SegmentNumber = segmentNumber;
        }
    }
}
