using System.Collections;
using System.Collections.Generic;
using AMG2D.Model.Persistence;

namespace AMG2D.Model
{
    /// <summary>
    /// Interface that provides methods for controlling the lifespan of in-game objects that represent a <see cref="MapPersistence"/> instance.
    /// </summary>
    public interface IMapElementFactory
    {
        /// <summary>
        /// Coroutine that activates the external objects of the specified map and then executes the callback coroutine.
        /// Implementing classes must execute provided coroutine at the end of the execution.
        /// </summary>
        /// <param name="map">Map to activate external objects for.</param>
        /// <param name="continueWith">Coroutine callback to execute after this call ends.</param>
        /// <returns></returns>
        public IEnumerator ActivateExternalObjects(MapPersistence map, IEnumerator continueWith = null);

        /// <summary>
        /// Coroutine that deactivates the specified List of external objects.
        /// </summary>
        /// <param name="map">Objects to deactivate.</param>
        /// <param name="continueWith">Coroutine callback to execute after this call ends.</param>
        /// <returns></returns>
        public IEnumerator ReleaseExternalObject(List<ExternalObjectInfo> externalObjects, IEnumerator continueWith = null);

    }
}
