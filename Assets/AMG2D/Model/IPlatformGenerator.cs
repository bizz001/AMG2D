using System;
using System.Collections;
using AMG2D.Model.Persistence;

namespace AMG2D.Model
{
    public interface IPlatformGenerator
    {
        public IEnumerator CreatePlatforms(MapPersistence map, IEnumerator continueWith = null);
    }
}
