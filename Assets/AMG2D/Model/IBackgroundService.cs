using UnityEngine;

namespace AMG2D.Model
{
    /// <summary>
    /// Service contract that provides a map background.
    /// </summary>
    public interface IBackgroundService
    {
        /// <summary>
        /// Set the starting point and size of the map.
        /// </summary>
        /// <param name="position">Initial position</param>
        /// <param name="height">Map height</param>
        void SetMapLimits(Vector2 position, int height);

        /// <summary>
        /// Update background based on current settings. It is recommended to call this from within a LateUpdate method.
        /// </summary>
        void UpdateBackground();
    }
}
