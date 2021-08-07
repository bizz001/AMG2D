using System;
using AMG2D.Model;
using AMG2D.Model.Persistence;
using AMG2D.Configuration;
using UnityEngine;
using System.Collections;

namespace AMG2D.Implementation
{
    public class BasicTileEnhancer : ITileEnhancer
    {
        private GeneralMapConfig _config;

        public BasicTileEnhancer(GeneralMapConfig mapConfig)
        {
            _config = mapConfig ?? throw new ArgumentNullException($"Argument {nameof(mapConfig)} cannot be null");
        }

        public IEnumerator PaintTiles(MapPersistence map, IEnumerator continueWith = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerator SetTilesCollider(MapPersistence map, IEnumerator continueWith = null)
        {
            throw new NotImplementedException();
        }
    }
}
