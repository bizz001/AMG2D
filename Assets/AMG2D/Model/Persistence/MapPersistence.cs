using System;
using System.Collections.Generic;

namespace AMG2D.Model.Persistence
{
    /// <summary>
    /// 
    /// </summary>
    public class MapPersistence
    {
        private bool _isMapVertical;

        /// <summary>
        /// 
        /// </summary>
        public IList<TileInformation[]> PersistedMap { get; }

        /// <summary>
        /// 
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// 
        /// </summary>
        public int Height { get; }

        /// <summary>
        /// 
        /// </summary>
        public int SegmentSize { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public MapPersistence(int width, int height, int segmentSize)
        {
            if (width == 0 || height == 0)
            {
                throw new ArgumentException($"Parameter {nameof(width)} or {nameof(height)} cannot be 0.");
            }
            Width = width;
            Height = height;
            SegmentSize = segmentSize;
            _isMapVertical = Width > Height;
            PersistedMap = CreateEmptyMap();
        }

        public void ClearMap()
        {
            foreach (var tileLine in PersistedMap)
            {
                foreach (var tile in tileLine)
                {
                    tile.TileType = Enum.ETileType.Air;
                }
            }
        }

        private IList<TileInformation[]> CreateEmptyMap()
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
            var emptyMap = new List<TileInformation[]>();
            for (int x = 0; x < xIterator; x++)
            {
                var yComponent = new TileInformation[yIterator];
                for (int y = 0; y < yIterator; y++)
                {
                    yComponent[y] = new TileInformation(x, y, (x / SegmentSize) + 1);
                }
                emptyMap.Add(yComponent);
            }
            return emptyMap;
        }

        public override string ToString()
        {
            var result = string.Empty;

            foreach (var line in PersistedMap)
            {
                foreach (var tile in line)
                {
                    result += tile.TileType.ToString().Substring(0,1) + " ";
                }
                result += "\r\n";
            }
            return result;
        }
    }
}
