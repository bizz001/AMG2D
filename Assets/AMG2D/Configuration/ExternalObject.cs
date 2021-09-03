using System;
using UnityEngine;

namespace AMG2D.Configuration
{
    /// <summary>
    /// Class that holds information regarding external objects spawing of a specific type.
    /// </summary>
    [Serializable]
    public class ExternalObjectConfig
    {
        /// <summary>
        /// Unique ID of this rule.
        /// </summary>
        public string UniqueID;

        /// <summary>
        /// Object template that will be cloned.
        /// </summary>
        public GameObject ObjectTemplate;

        /// <summary>
        /// General placement position of the cloned objects.
        /// </summary>
        public EObjectPosition Position;

        /// <summary>
        /// Density of the objects spawned.
        /// </summary>
        public int Density;
    }
}
