using UnityEngine;

namespace AMG2D.Model
{
    public interface IBackgroundService
    {
        void UpdateBackground();
        void SetMapLimits(Vector2 position, int height);
    }
}
