using System;
using System.Collections.Generic;
using AMG2D.Implementation;
using AMG2D.Model;
using AMG2D.Configuration;

namespace AMG2D.Bootstrap
{ 
    public static class ServiceLocator
    {
        private static Dictionary<Type, object> _services;
        private static bool _isBuilt;

        /// <summary>
        /// Prepares internal state of class for resolving services.
        /// </summary>
        public static void Build(GeneralMapConfig mapConfig)
        {
            if(mapConfig == null) throw new ArgumentNullException($"Argument {nameof(mapConfig)} cannot be null");
            if (!mapConfig.CheckCorrectness()) throw new ArgumentException($"Configuration is currently in an incorrect state. Please check configuration.");

            _services = new Dictionary<Type, object>
            {
                { typeof(IMapElementFactory), new PooledSegmentedMapFactory(mapConfig) },
                { typeof(ITileEnhancer), new BasicTileEnhancer(mapConfig) },
                { typeof(ICaveGenerator), new CompleteMapGenerator(mapConfig) },
                { typeof(IPlatformGenerator), new CompleteMapGenerator(mapConfig) },
                { typeof(IGroundGenerator), new CompleteMapGenerator(mapConfig) },
                { typeof(IExternalObjectsPositioner), new CompleteMapGenerator(mapConfig) },
                { typeof(IBackgroundService), new ParallaxBackgroundService(mapConfig) }
            };

            _isBuilt = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="ServiceType"></typeparam>
        public static ServiceType GetService<ServiceType>()
        {
            if (!_isBuilt) throw new InvalidOperationException($"{nameof(ServiceLocator)} must be built before resolving services. please call {nameof(Build)} method.");
            try
            {
                return (ServiceType) _services[typeof(ServiceType)];
            }
            catch (KeyNotFoundException ex)
            {
                throw new ArgumentException($"Cannot locate service of type {typeof(ServiceType)}", ex);
            }
        }
    }
}
