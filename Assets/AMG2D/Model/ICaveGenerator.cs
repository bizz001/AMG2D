using System;
using System.Collections.Generic;
using AMG2D.Model.Persistence;

namespace AMG2D.Model
{
    public interface ICaveGenerator
    {
        public void CreateCaves(ref MapPersistence map);
    }
}
