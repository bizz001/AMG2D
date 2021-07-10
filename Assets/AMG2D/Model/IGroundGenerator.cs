using System;
using System.Collections.Generic;
using AMG2D.Model.Persistence;

namespace AMG2D.Model
{
    public interface IGroundGenerator
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="test"></param>
        public void CreateGround(ref MapPersistence map);
    }
}