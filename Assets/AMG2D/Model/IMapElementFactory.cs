using System;
using System.Collections;
using AMG2D.Model.Persistence;
using AMG2D.Model.Persistence.Enum;
using UnityEngine;

namespace AMG2D.Model
{
    public interface IMapElementFactory
    {
        public IEnumerator ActivateExternalObjects(MapPersistence map, IEnumerator continueWith = null);
        public IEnumerator ReleaseExternalObject(MapPersistence map, IEnumerator continueWith = null);

    }
}
