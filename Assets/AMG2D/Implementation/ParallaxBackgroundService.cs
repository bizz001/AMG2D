﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AMG2D.Configuration;
using AMG2D.Model;
using UnityEngine;

namespace AMG2D.Implementation
{
    /// <summary>
    /// Background service implementation that provides a parallax effect to the background.
    /// </summary>
    public class ParallaxBackgroundService : IBackgroundService
    {
        private readonly GeneralMapConfig _config;
        private readonly List<ParallaxBackgroundLayer> _layers = new List<ParallaxBackgroundLayer>();
        private bool _mapLimitsSet = false;

        /// <summary>
        /// Creats a new instance of <see cref="ParallaxBackgroundService"/> using the provided configuration.
        /// </summary>
        /// <param name="mapConfig">General </param>
        public ParallaxBackgroundService(GeneralMapConfig mapConfig)
        {
            _config = mapConfig ?? throw new ArgumentNullException($"Argument {nameof(mapConfig)} cannot be null");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="height"></param>
        public void SetMapLimits(Vector2 position, int height)
        {
            foreach (var layerConfig in _config.Background.BackgroundLayers)
            {
                _layers.Add(new ParallaxBackgroundLayer(layerConfig, _config, position, height));
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
                    layer.UpdatePosition();
                }
            }
        }
    }
}
