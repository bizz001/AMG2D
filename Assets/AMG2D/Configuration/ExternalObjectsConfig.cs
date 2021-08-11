using System;
using System.Collections.Generic;

namespace AMG2D.Configuration
{
    /// <summary>
    /// Class that holds the configuration information of all the external objects to be spawned.
    /// </summary>
    [Serializable]
    public class ExternalObjectsConfig
    {
        /// <summary>
        /// Array that holds the configuration information of all the external objects to be spanwed.
        /// </summary>
        public ExternalObjectConfig[] ExternalObjects;
    }
}
