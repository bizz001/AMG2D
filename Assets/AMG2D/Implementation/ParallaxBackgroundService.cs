using System;
using AMG2D.Configuration;
using AMG2D.Model;

namespace AMG2D.Implementation
{
    public class ParallaxBackgroundService : IBackgroundService
    {
        private GeneralMapConfig _config;

        public ParallaxBackgroundService(GeneralMapConfig mapConfig)
        {
            _config = mapConfig ?? throw new ArgumentNullException($"Argument {nameof(mapConfig)} cannot be null");
        }

        public void UpdateBackground()
        {
            //throw new NotImplementedException();
        }
    }
}
