using System;
using UnityEngine;

namespace AMG2D.Configuration
{
    /// <summary>
    /// Class that holds information regarding cave creating configuration.
    /// Currently empty as it is not yet implemented.
    /// </summary>
    [Serializable]
    public class CompleteConfiguration
    {
        /// <summary>
        /// Current <see cref="BackgroundConfig"/> instanace.
        /// </summary>
        [SerializeReference]
        public BackgroundConfig Background = new BackgroundConfig();

        /// <summary>
        /// Current <see cref="GroundConfig"/> instanace.
        /// </summary>
        [SerializeReference]
        public GroundConfig Ground = new GroundConfig();

        /// <summary>
        /// Current <see cref="PlatformsConfig"/> instanace.
        /// </summary>
        [SerializeReference]
        public PlatformsConfig Platforms = new PlatformsConfig();

        /// <summary>
        /// Current <see cref="ExternalObjectsConfig"/> instanace.
        /// </summary>
        [SerializeReference]
        public ExternalObjectsConfig ExternalObjects = new ExternalObjectsConfig();
    }
}