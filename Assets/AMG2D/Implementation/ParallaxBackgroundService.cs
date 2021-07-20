using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AMG2D.Configuration;
using AMG2D.Model;
using UnityEngine;

namespace AMG2D.Implementation
{
    public class ParallaxBackgroundService : IBackgroundService
    {
        private GeneralMapConfig _config;
        private float posInic, anchura;
        private bool _mapLimitsSet = false;
        private List<ParallaxBackgroundLayer> _layers = new List<ParallaxBackgroundLayer>();

        public ParallaxBackgroundService(GeneralMapConfig mapConfig)
        {
            _config = mapConfig ?? throw new ArgumentNullException($"Argument {nameof(mapConfig)} cannot be null");
        }

        public void SetMapLimits(Vector2 position, int height)
        {
            foreach (var layerConfig in _config.Background.BackgroundLayers)
            {
                _layers.Add(new ParallaxBackgroundLayer(layerConfig, position, height, 0));
            }
            _mapLimitsSet = true;
        }

        /// <summary>
        /// Update background based on current settings. It is recommended to call this from within a FixedUpdate method.
        /// </summary>
        public void UpdateBackground()
        {
            if (_mapLimitsSet)
            {
                foreach (var layer in _layers)
                {
                    layer.UpdatePosition(_config.Camera.transform.position.x);
                }
                //Parallel.ForEach(_layers, layer => layer.UpdatePosition(_config.Camera.transform.position.x));
            }
        }
    }
}
