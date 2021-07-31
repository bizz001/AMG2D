using System;
using AMG2D.Model;
using AMG2D.Model.Persistence;
using AMG2D.Configuration;
using UnityEngine;

namespace AMG2D.Implementation
{
    public class BasicTileEnhancer : ITileEnhancer
    {
        private GeneralMapConfig _config;

        public BasicTileEnhancer(GeneralMapConfig mapConfig)
        {
            _config = mapConfig ?? throw new ArgumentNullException($"Argument {nameof(mapConfig)} cannot be null");
        }

        void ITileEnhancer.PaintTiles(ref MapPersistence map)
        {
            throw new NotImplementedException();
        }

        void ITileEnhancer.SetTilesCollider(ref MapPersistence map)
        {
            
        }
    }
}
