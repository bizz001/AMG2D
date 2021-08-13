using System;
using UnityEngine;

namespace AMG2D.Model.Persistence
{
    /// <summary>
    /// Object holding the information of a specific external object instance that will be placed on the map.
    /// </summary>
    public class ExternalObjectInfo
    {
        /// <summary>
        /// Type of the object.
        /// </summary>
        public string TypeID;

        /// <summary>
        /// <see cref="GameObject"/> instance that represents this external object.
        /// </summary>
        public GameObject SpawnedObject;

        /// <summary>
        /// Asigned tile where the object will be positioned initially.
        /// </summary>
        public TileInformation AsignedTile;

        /// <summary>
        /// <see cref="GameObject"/> template from which the <see cref="SpawnedObject"/> must be created.
        /// </summary>
        public GameObject Template;
    }
}
