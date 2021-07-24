using UnityEngine;

namespace AMG2D.Model
{
    public interface IBackgroundService
    {
        void SetMapLimits(Vector2 position, int height);
        void UpdateBackground();
    }
}
