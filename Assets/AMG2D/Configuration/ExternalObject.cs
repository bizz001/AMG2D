using System;
using AMG2D.Configuration.Enum;
using UnityEngine;

namespace AMG2D.Configuration
{
    [Serializable]
    public class ExternalObjectConfig
    {
        public string UniqueID;

        public GameObject ObjectTemplate;

        public EObjectPosition Position;

        public int Density;

    }
}
