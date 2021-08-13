using System;
using System.Collections;
using AMG2D.Model.Persistence;

namespace AMG2D.Model
{
    /// <summary>
    /// Interface that privdes a way to create platforms.
    /// </summary>
    public interface IPlatformGenerator
    {
        /// <summary>
        /// Modifies the provided <see cref="MapPersistence"/> in order to add ground.
        /// </summary>
        /// <param name="map">map to modify.</param>
        /// <param name="continueWith">coroutine to execute at the end.</param>
        /// <returns></returns>
        public IEnumerator CreatePlatforms(MapPersistence map, IEnumerator continueWith = null);
    }
}
