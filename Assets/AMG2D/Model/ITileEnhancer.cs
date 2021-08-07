using System;
using System.Collections;
using AMG2D.Model.Persistence;

namespace AMG2D.Model
{
    public interface ITileEnhancer
    {
        public IEnumerator PaintTiles(MapPersistence map, IEnumerator continueWith = null);
        public IEnumerator SetTilesCollider(MapPersistence map, IEnumerator continueWith = null);
    }
}
