using System.Collections;
using AMG2D.Model.Persistence;

namespace AMG2D.Model
{
    /// <summary>
    /// Service contract that provides positioning for external objects.
    /// </summary>
    public interface IExternalObjectsPositioner
    {
        /// <summary>
        /// Modifies the provided <see cref="MapPersistence"/> in order to add external objects.
        /// </summary>
        /// <param name="map">map to modify.</param>
        /// <param name="continueWith">coroutine to execute at the end.</param>
        /// <returns></returns>
        public IEnumerator PositionExternalObjects(MapPersistence map, IEnumerator continueWith = null);
    }
}