using System;
using AMG2D.Configuration.Enum;
using UnityEngine;

namespace AMG2D.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class ExternalObjectConfig
    {
        /// <summary>
        /// 
        /// </summary>
        public string UniqueID;

        /// <summary>
        /// 
        /// </summary>
        public GameObject ObjectTemplate;

        /// <summary>
        /// 
        /// </summary>
        public EObjectPosition Position;

        /// <summary>
        /// 
        /// </summary>
        public int Density;

    }
}
