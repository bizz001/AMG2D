using System;
using System.Collections.Generic;
using AMG2D.Configuration.Enum;
using UnityEngine;

namespace AMG2D.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class GeneralMapConfig
    {
        [SerializeField]
        private bool _isCorrectnessChecked;

        [SerializeField]
        private int _height;

        [SerializeField]
        private int _width;

        [SerializeField]
        private Dictionary<EGameObjectType, GameObject> _aspects;

        /// <summary>
        /// 
        /// </summary>
        public int Height
        {
            get { return _height; }
            set
            {
                if(value != _height)
                {
                    _isCorrectnessChecked = false;
                    _height = value;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int Width
        {
            get { return _width; }
            set
            {
                if (value != _width)
                {
                    _isCorrectnessChecked = false;
                    _width = value;
                }
            }
        }

        public Dictionary<EGameObjectType, GameObject> ObjectSeeds;

        /// <summary>
        /// Camera object for movement tracking;
        /// </summary>
        public GameObject Camera;

        public bool EnableSegmentation;

        public int SegmentSize;

        public int CameraWidth;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool CheckCorrectness()
        {
            if (_isCorrectnessChecked) return _isCorrectnessChecked;

            //check consistency
            _isCorrectnessChecked = true;

            return _isCorrectnessChecked;
        }

    }
}
