using System.Collections;
using AMG2D.Model.Persistence;

namespace AMG2D.Model
{
    /// <summary>
    /// Service contract interface that provides ground creation for <see cref="MapPersistence"/> objects.
    /// </summary>
    public interface IGroundGenerator
    {
        /// <summary>
        /// Coroutine that modifies the provided <see cref="MapPersistence"/> in order to add ground according to the configuration injected at creation.
        /// When implementing this interface, the provided coroutine must be executed at the end.
        /// </summary>
        /// <param name="map">map to modify.</param>
        /// <param name="continueWith">coroutine to execute at the end.</param>
        /// <returns></returns>
        public IEnumerator CreateGround(MapPersistence map, IEnumerator continueWith = null);
    }
}