using System.Collections;
using AMG2D.Model.Persistence;
using UnityEngine;

namespace AMG2D.Model
{
    public interface ITilesFactory
    {
        public IEnumerator ActivateTiles(MapPersistence map, MonoBehaviour parent, IEnumerator continueWith = null);
        public IEnumerator ReleaseTiles(TileInformation[][] tiles, IEnumerator continueWith = null);
    }
}
