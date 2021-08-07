using System;
using System.Collections;
using AMG2D.Model.Persistence;

namespace AMG2D.Model
{
    public interface IGroundGenerator
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="test"></param>
        public IEnumerator CreateGround(MapPersistence map, IEnumerator continueWith = null);
    }
}