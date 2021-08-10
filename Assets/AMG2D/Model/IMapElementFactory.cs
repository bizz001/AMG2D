using System;
using System.Collections;
using System.Collections.Generic;
using AMG2D.Model.Persistence;

namespace AMG2D.Model
{
    public interface IMapElementFactory
    {
        public IEnumerator ActivateExternalObjects(MapPersistence map, IEnumerator continueWith = null);
        public IEnumerator ReleaseExternalObject(List<ExternalObjectInfo> externalObjects, IEnumerator continueWith = null);

    }
}
