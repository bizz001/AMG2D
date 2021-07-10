using System;
using System.Collections.Generic;
using AMG2D.Model.Persistence;

namespace AMG2D.Model
{
    public interface ITileEnhancer
    {
        public void PaintTiles(ref MapPersistence map);
        public void SetTilesCollider(ref MapPersistence map);
    }
}
