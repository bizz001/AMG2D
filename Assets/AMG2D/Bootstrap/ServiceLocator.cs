using System;
using System.Collections.Generic;
using AMG2D.Implementation;
using AMG2D.Model;
using AMG2D.Configuration;
using AMG2D.Implementation.Background;

namespace AMG2D.Bootstrap
{
    /// <summary>
    /// Static class meant to provide dependency injection to to classes that cannot have custom constructors.
    /// </summary>
    public static class ServiceLocator
    {
        private static Dictionary<Type, object> _services;
        private static bool _isBuilt;

        /// <summary>
        /// Prepares internal state of class for resolving services.
        /// </summary>
        public static void Build(CompleteConfiguration completeConfig)
        {
            if(completeConfig == null) throw new ArgumentNullException($"Argument {nameof(completeConfig)} cannot be null");
            _services = new Dictionary<Type, object>
            {
                { typeof(IMapElementFactory), new PooledMapElementFactory(completeConfig) },
                { typeof(ITilesFactory), new TiledMapFactory(completeConfig.GeneralMapSettings) },
                { typeof(ICaveGenerator), new CompleteMapGenerator(completeConfig) },
                { typeof(IPlatformGenerator), new CompleteMapGenerator(completeConfig) },
                { typeof(IGroundGenerator), new CompleteMapGenerator(completeConfig) },
                { typeof(IExternalObjectsPositioner), new CompleteMapGenerator(completeConfig) },
                { typeof(IBackgroundService), new ParallaxBackgroundService(completeConfig) }
            };
            _isBuilt = true;
        }

        /// <summary>
        /// Returns the impelentation for a service of a specific type.
        /// </summary>
        /// <typeparam name="ServiceType">The type of the service to resolve.</typeparam>
        /// <returns></returns>
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
