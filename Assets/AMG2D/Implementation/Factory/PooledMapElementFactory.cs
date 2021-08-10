using System;
using System.Collections.Generic;
using System.Collections;
using AMG2D.Model;
using AMG2D.Model.Persistence;
using AMG2D.Model.Persistence.Enum;
using UnityEngine;
using AMG2D.Configuration;
using AMG2D.Configuration.Enum;
using System.Linq;

namespace AMG2D.Implementation
{
    /// <summary>
    /// 
    /// </summary>
    public class PooledMapElementFactory : IMapElementFactory
    {
        private const int HYSTERESIS = 1;

        private Dictionary<string, Queue<GameObject>> _pools;
        private GeneralMapConfig _config;
        private int _lastPlayerSegment;
        private List<int> _lastActiveSegments;
        private int _direction;
        private int _lastTransitionedSegment;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapConfig"></param>
        public PooledMapElementFactory(GeneralMapConfig mapConfig)
        {
            _config = mapConfig ?? throw new ArgumentNullException($"Argument {nameof(mapConfig)} cannot be null");
            _pools = new Dictionary<string, Queue<GameObject>>();
            foreach (var seed in _config.ObjectSeeds)
            {
                _pools.Add(seed.Key, new Queue<GameObject>());
            }

            foreach (var externalObject in _config.ExternalObjects.ExternalObjects)
            {
                _pools.Add(externalObject.UniqueID, new Queue<GameObject>());
            }
        }

        public IEnumerator ActivateExternalObjects(MapPersistence map, IEnumerator continueWith)
        {
            if (_config.EnableSegmentation) yield return ActivateExternalObjectWithSegmentation(map.ExternalObjects);
            else yield return ActivateAllObjects(map.ExternalObjects);
            yield return continueWith;
        }

        public IEnumerator ActivateExternalObjectWithSegmentation(List<ExternalObjectInfo> externalObjects)
        {

             var currentPlayerSegment = (int)(_config.Camera.transform.position.x / _config.SegmentSize) + 1;
            if (currentPlayerSegment == _lastPlayerSegment) yield break;

            //Hysteresis to prevent erratic loading/unloading
            if (currentPlayerSegment == _lastTransitionedSegment) yield break;

            //add current player segment and neighbouring segments
            var activeSegments = new List<int>();
            activeSegments.Add(currentPlayerSegment);
            for (int i = 1; i <= (_config.NumberOfSegments - 1) / 2; i++)
            {
                activeSegments.Add(currentPlayerSegment - i);
                activeSegments.Add(currentPlayerSegment + i);
            }
            if (_lastActiveSegments != null)
            {
                yield return ReleaseExternalObject(externalObjects.Select(obj => obj)
                    .Where(obj => _lastActiveSegments.Contains(obj.AsignedTile.SegmentNumber) && !activeSegments.Contains(obj.AsignedTile.SegmentNumber)).ToList());
                yield return ActivateAllObjects(externalObjects.Select(obj => obj)
                    .Where(obj => activeSegments.Contains(obj.AsignedTile.SegmentNumber) && !_lastActiveSegments.Contains(obj.AsignedTile.SegmentNumber)).ToList());
            }
            else
            {
                yield return ActivateAllObjects(externalObjects.Select(obj => obj).Where(obj => activeSegments.Contains(obj.AsignedTile.SegmentNumber)).ToList());
            }
            _lastTransitionedSegment = _lastPlayerSegment;
            _lastPlayerSegment = currentPlayerSegment;
            _lastActiveSegments = activeSegments;
        }

        private IEnumerator ActivateAllObjects(List<ExternalObjectInfo> externalObjects)
        {
            foreach (var obj in externalObjects)
            {
                if (obj.SpawnedObject != null) continue;
                if(_pools[obj.TypeID].Count > 0)
                {
                    obj.SpawnedObject = _pools[obj.TypeID].Dequeue();
                    obj.SpawnedObject.transform.position = new Vector2(obj.AsignedTile.X + 0.5f, obj.AsignedTile.Y + 0.5f);
                    obj.SpawnedObject.SetActive(true);
                }
                else
                {
                    obj.SpawnedObject = MonoBehaviour.Instantiate(obj.Template, new Vector2(obj.AsignedTile.X + 0.5f, obj.AsignedTile.Y + 0.5f), Quaternion.identity);
                }
            }
            yield break;
        }

        public IEnumerator ReleaseExternalObject(List<ExternalObjectInfo> externalObjects, IEnumerator continueWith = null)
        {
            foreach (var obj in externalObjects)
            {
                if (obj.SpawnedObject == null) continue;
                obj.SpawnedObject.SetActive(false);
                _pools[obj.TypeID].Enqueue(obj.SpawnedObject);
                obj.SpawnedObject = null;
            }
            yield break;
        }
    }
}
