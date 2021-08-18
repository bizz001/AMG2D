using System;
using System.Collections.Generic;

namespace AMG2D.Model.Persistence
{
    /// <summary>
    /// Class that holds all the information necessary to display all the map elements such as Tiles and external objects.
    /// </summary>
    public class MapPersistence
    {
        private bool _isMapVertical;

        /// <summary>
        /// Collection of all tiles present in this map, stored as <see cref="TileInformation"/> objects.
        /// </summary>
        public TileInformation[][] PersistedMap { get; }

        /// <summary>
        /// Collection of all external object instances to be generated on the map.
        /// </summary>
        public List<ExternalObjectInfo> ExternalObjects { get; }

        /// <summary>
        /// Total Width of the map.
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// Total Height of the map.
        /// </summary>
        public int Height { get; }

        /// <summary>
        /// The size of each individual segment when segmentation is enabled.
        /// </summary>
        public int SegmentSize { get; }

        /// <summary>
        /// Creates an instance of <see cref="MapPersistence"/> using the provided dimensions.
        /// </summary>
        /// <param name="width"><width of the map./param>
        /// <param name="height">height of the map.</param>
        /// <param name="segmentSize">size of the map segments.</param>
        public MapPersistence(int width, int height, int segmentSize)
        {
            if (width == 0 || height == 0)
            {
                throw new ArgumentException($"Parameter {nameof(width)} or {nameof(height)} cannot be 0.");
            }
            ExternalObjects = new List<ExternalObjectInfo>();
            Width = width;
            Height = height;
            SegmentSize = segmentSize;
            _isMapVertical = Width > Height;
            PersistedMap = CreateEmptyMap();
        }

        /// <summary>
        /// Clears the map by setting all tiles to <see cref="Enum.ETileType.Air"/>
        /// </summary>
        public void ClearMap()
        {
            foreach (var tileLine in PersistedMap)
            {
                foreach (var tile in tileLine)
                {
                    tile.TileType = ETileType.Air;
                }
            }
        }

        /// <summary>
        /// Creats all the initial TileInformation objects that will compose this map.
        /// </summary>
        /// <returns></returns>
        private TileInformation[][] CreateEmptyMap()
        {
            int xIterator, yIterator;
            if (_isMapVertical)
            {
                xIterator = Width;
                yIterator = Height;
            }
            else
            {
                xIterator = Height;
                yIterator = Width;
            }

            //create empty map
            var emptyMap = new TileInformation[xIterator][];
            for (int x = 0; x < xIterator; x++)
            {
                var yComponent = new TileInformation[yIterator];
                for (int y = 0; y < yIterator; y++)
                {
                    yComponent[y] = new TileInformation(x, y, (x / SegmentSize) + 1);
                }
                emptyMap[x] = yComponent;
            }
            return emptyMap;
        }
    }
}
