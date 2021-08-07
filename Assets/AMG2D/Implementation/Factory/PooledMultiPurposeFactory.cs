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
    public class PooledMultiPurposeFactory : IMapElementFactory, ITilesFactory
    {
        private Dictionary<string, Queue<GameObject>> _pools;
        private Queue<GameObject> _segmentPool;
        private GeneralMapConfig _config;
        private readonly Dictionary<int, GameObject> _segmentParents;
        private int _lastPlayerSegment;
        private List<int> _lastActiveSegments;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapConfig"></param>
        public PooledMultiPurposeFactory(GeneralMapConfig mapConfig)
        {
            _config = mapConfig ?? throw new ArgumentNullException($"Argument {nameof(mapConfig)} cannot be null");
            _segmentParents = new Dictionary<int, GameObject>();
            _pools = new Dictionary<string, Queue<GameObject>>();
            _segmentPool = new Queue<GameObject>();
            foreach (var seed in _config.ObjectSeeds)
            {
                _pools.Add(seed.Key, new Queue<GameObject>());
            }

            foreach (var externalObject in _config.ExternalObjects.ExternalObjects)
            {
                _pools.Add(externalObject.UniqueID, new Queue<GameObject>());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="tiles"></param>
        public IEnumerator ActivateTiles(MapPersistence map, MonoBehaviour parent, IEnumerator continueWith)
        {
            if (_config.EnableSegmentation) yield return ActivateSegmentedTiles(map);
            else if (!map.PersistedMap.First().First().IsActive) yield return ActivateAllTiles(map.PersistedMap);
            yield return continueWith;
        }

        private IEnumerator ActivateAllTiles(TileInformation[][] tiles)
        {
            if (tiles == null) yield break;
            HashSet<int> activatedSegments = new HashSet<int>();
            foreach (var tilesLine in tiles)
            {
                activatedSegments.Add(tilesLine.First().SegmentNumber);
                foreach (var tile in tilesLine)
                {
                    var currentTileType = GetObjectType(tile.TileType);
                    if (tile.TileType == ETileType.Air) continue;
                    if (!tile.IsActive && _pools[currentTileType.ToString()].Count > 0)
                    {
                        var pooledTile = _pools[currentTileType.ToString()].Dequeue();
                        pooledTile.transform.position = new Vector2(tile.X, tile.Y);
                        tile.CurrentPrefab = pooledTile;
                    }
                    else
                    {
                        tile.CurrentPrefab = MonoBehaviour.Instantiate(_config.ObjectSeeds[currentTileType.ToString()], new Vector2(tile.X, tile.Y), Quaternion.identity);
                        tile.CurrentPrefab.SetActive(true);
                    }
                    if (!_segmentParents.TryGetValue(tile.SegmentNumber, out GameObject parent)) //search active segments first
                    {
                        if(_segmentPool.Count == 0) //try pool second
                        {
                            parent = new GameObject();
                            var collider = parent.AddComponent<CompositeCollider2D>();
                            collider.generationType = CompositeCollider2D.GenerationType.Manual;
                            parent.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
                        }
                        else
                        {
                            parent = _segmentPool.Dequeue();
                        }
                        parent.name = $"MapSegment{tile.SegmentNumber}";
                        _segmentParents.Add(tile.SegmentNumber, parent);
                    }
                    tile.CurrentPrefab.transform.SetParent(parent.transform);
                }
            }
            //_doLater = () =>
            //{
            //    foreach (var segmentNum in activatedSegments)
            //    {
            //        if (_segmentParents.TryGetValue(segmentNum, out var seg)) seg.GetComponent<CompositeCollider2D>().GenerateGeometry();
            //    }
            //};
        }

        private IEnumerator ActivateSegmentedTiles(MapPersistence map)
        {
            var currentPlayerSegment = (int)(_config.Camera.transform.position.x / _config.SegmentSize) + 1;
            if (currentPlayerSegment == _lastPlayerSegment) yield break;
            var tiles = map.PersistedMap;
            //add current player segment and neighbouring segments
            var activeSegments = new List<int>();
            activeSegments.Add(currentPlayerSegment);
            for (int i = 1; i <= (_config.NumberOfSegments - 1) / 2; i++)
            {
                activeSegments.Add(currentPlayerSegment - i);
                activeSegments.Add(currentPlayerSegment + i);
            }
            if(_lastActiveSegments != null)
            {
                yield return ReleaseTiles(tiles.Select(tileLine => tileLine)
                    .Where(tileLine => _lastActiveSegments.Contains(tileLine.First().SegmentNumber) && !activeSegments.Contains(tileLine.First().SegmentNumber)).ToArray());
                yield return ActivateAllTiles(tiles.Select(tileLine => tileLine)
                    .Where(tileLine => activeSegments.Contains(tileLine.First().SegmentNumber) && !_lastActiveSegments.Contains(tileLine.First().SegmentNumber)).ToArray());
            }
            else
            {
                ActivateAllTiles(tiles.Select(tileLine => tileLine).Where(tileLine => activeSegments.Contains(tileLine.First().SegmentNumber)).ToArray());
            }
            _lastPlayerSegment = currentPlayerSegment;
            _lastActiveSegments = activeSegments;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tiles"></param>
        public IEnumerator ReleaseTiles(TileInformation[][] tiles, IEnumerator continueWith = null)
        {
            foreach (var tilesLine in tiles)
            {                
                foreach (var tile in tilesLine)
                {
                    if (tile.CurrentPrefab != null)
                    {
                        if (_segmentParents.TryGetValue(tile.SegmentNumber, out var segment))
                        {
                            _segmentParents.Remove(tile.SegmentNumber);
                            _segmentPool.Enqueue(segment);
                        }
                        _pools[GetObjectType(tile.TileType).ToString()].Enqueue(tile.CurrentPrefab);
                        tile.CurrentPrefab = null;
                    }
                }
            }
            yield return continueWith;
        }

        public IEnumerator ActivateExternalObjects(MapPersistence map, IEnumerator continueWith)
        {
            if (_config.EnableSegmentation) yield break;
            foreach (var obj in map.ExternalObjects)
            {
                obj.SpawnedObject = MonoBehaviour.Instantiate(obj.Template, new Vector2(obj.AsignedTile.X, obj.AsignedTile.Y), Quaternion.identity);
            }
            yield return continueWith;
        }

        public IEnumerator ReleaseExternalObject(MapPersistence map, IEnumerator continueWith)
        {
            yield return continueWith;
        }

        private EGameObjectType GetObjectType(ETileType tile)
        {
            return tile switch
            {
                ETileType.Air => EGameObjectType.AirTile,
                ETileType.Cave => EGameObjectType.CaveTile,
                ETileType.Stone => EGameObjectType.StoneTile,
                ETileType.Grass => EGameObjectType.GrassTile,
                ETileType.Ground => EGameObjectType.GroundTile,
                ETileType.Platform => EGameObjectType.PlatformTile,
                _ => EGameObjectType.Unknown,
            };
        }
    }
}
