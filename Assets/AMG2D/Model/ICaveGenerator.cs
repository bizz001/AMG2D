using System;
using System.Collections;
using AMG2D.Model.Persistence;

namespace AMG2D.Model
{
    public interface ICaveGenerator
    {
        public IEnumerator CreateCaves(MapPersistence map, IEnumerator continueWith = null);
    }
}
