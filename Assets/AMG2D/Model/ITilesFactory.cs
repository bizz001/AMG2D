using System.Collections;
using AMG2D.Model.Persistence;
using UnityEngine;

namespace AMG2D.Model
{
    /// <summary>
    /// Service interface that provides a methods for controlling the lifespan of game tiles.
    /// </summary>
    public interface ITilesFactory
    {
        /// <summary>
        /// Coroutine that activates the Tiles objects of the specified map and then executes the callback coroutine.
        /// </summary>
        /// <param name="map">map to activate.</param>
        /// <param name="parent">parent object of the map.</param>
        /// <param name="continueWith">callback to execute when done.</param>
        /// <returns></returns>
        public IEnumerator ActivateTiles(MapPersistence map, MonoBehaviour parent, IEnumerator continueWith = null);

        /// <summary>
        /// Coroutine that releases the specified tiles and then continues execution with the provided callback coroutine.
        /// </summary>
        /// <param name="tiles">tiles to release.</param>
        /// <param name="continueWith">callback to execute when done.</param>
        /// <returns></returns>
        public IEnumerator ReleaseTiles(TileInformation[][] tiles, IEnumerator continueWith = null);
    }
}
