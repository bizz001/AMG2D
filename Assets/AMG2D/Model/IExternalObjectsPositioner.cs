using System.Collections;
using System.Collections.Generic;
using AMG2D.Model.Persistence;

namespace AMG2D.Model
{
    public interface IExternalObjectsPositioner
    {
        public IEnumerator PositionExternalObjects(MapPersistence map, IEnumerator continueWith = null);
    }
}