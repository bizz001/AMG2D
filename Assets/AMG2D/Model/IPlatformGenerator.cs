using System;
using System.Collections.Generic;
using AMG2D.Model.Persistence;

namespace AMG2D.Model
{
    public interface IPlatformGenerator
    {
        public void CreatePlatforms(ref MapPersistence map);
    }
}
