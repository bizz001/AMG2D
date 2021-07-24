using System;
using System.Collections.Generic;
using AMG2D.Model.Persistence;
using AMG2D.Model.Persistence.Enum;
using UnityEngine;

namespace AMG2D.Model
{
    public interface IMapElementFactory
    {
        public bool ActivateTiles(TileInformation[][] tile);
        public void ReleaseTiles(TileInformation[][] tile);

    }
}
