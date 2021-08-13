using System;
using System.Collections;
using AMG2D.Model.Persistence;

namespace AMG2D.Model
{
    /// <summary>
    /// Service contract that provides cave generation to maps.
    /// </summary>
    public interface ICaveGenerator
    {
        /// <summary>
        /// Modifies the provided <see cref="MapPersistence"/> in order to add caves.
        /// </summary>
        /// <param name="map">map to modify.</param>
        /// <param name="continueWith">coroutine to execute at the end.</param>
        /// <returns></returns>
        public IEnumerator CreateCaves(MapPersistence map, IEnumerator continueWith = null);
    }
}
