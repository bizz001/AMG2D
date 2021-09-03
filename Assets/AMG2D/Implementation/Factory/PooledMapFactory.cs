using System;
using System.Collections.Generic;
using System.Collections;
using AMG2D.Model;
using AMG2D.Model.Persistence;
using UnityEngine;
using AMG2D.Configuration;
using System.Linq;

namespace AMG2D.Implementation
{
    /// <summary>
    /// <see cref="ITilesFactory"/> implementation that creates the map out of pooled objects of <see cref="GameObject"/> type.
    /// </summary>
    public class PooledMapFactory : ITilesFactory
    {
        private Dictionary<string, Queue<GameObject>> _pools;
        private Queue<GameObject> _segmentPool;
        private GeneralMapConfig _config;
        private readonly Dictionary<int, GameObject> _segmentParents;
        private int _lastPlayerSegment;
        private List<int> _lastActiveSegments;

        /// <summary>
        /// Creates an instance of <see cref="PooledMapFactory"/> using the provided configuration.
        /// </summary>
        /// <param name="mapConfig">configuration object that will determine the behaviour of this instance.</param>
        public PooledMapFactory(GeneralMapConfig mapConfig)
        {
            _config = mapConfig ?? throw new ArgumentNullException($"Argument {nameof(mapConfig)} cannot be null");
            _segmentParents = new Dictionary<int, GameObject>();
            _pools = new Dictionary<string, Queue<GameObject>>();
            _segmentPool = new Queue<GameObject>();
            foreach (var seed in _config.ObjectSeeds)
            {
                _pools.Add(seed.Key, new Queue<GameObject>());
            }
        }

        /// <summary>
        /// Coroutine that activates the Tiles objects of the specified map and then executes the callback coroutine.
        /// </summary>
        /// <param name="map">map to activate.</param>
        /// <param name="parent">parent object of the map.</param>
        /// <param name="continueWith">callback to execute when done.</param>
        /// <returns></returns>
        public IEnumerator ActivateTiles(MapPersistence map, MonoBehaviour parent, IEnumerator continueWith)
        {
            if (_config.EnableSegmentation) yield return ActivateSegmentedTiles(map, parent);
            else if (!map.PersistedMap.First().First().IsActive) yield return ActivateAllTiles(map.PersistedMap, parent);
            yield return continueWith;
        }

        private IEnumerator ActivateAllTiles(TileInformation[][] tiles, MonoBehaviour parent)
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
                    if (!_segmentParents.TryGetValue(tile.SegmentNumber, out GameObject segment)) //search active segments first
                    {
                        if(_segmentPool.Count == 0) //try pool second
                        {
                            segment = new GameObject();
                            segment.transform.SetParent(parent.transform);
                            var collider = segment.AddComponent<CompositeCollider2D>();
                            collider.generationType = CompositeCollider2D.GenerationType.Manual;
                            segment.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
                        }
                        else
                        {
                            segment = _segmentPool.Dequeue();
                        }
                        segment.name = $"MapSegment{tile.SegmentNumber}";
                        _segmentParents.Add(tile.SegmentNumber, segment);
                    }
                    tile.CurrentPrefab.transform.SetParent(segment.transform);
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

        private IEnumerator ActivateSegmentedTiles(MapPersistence map, MonoBehaviour parent)
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
                    .Where(tileLine => activeSegments.Contains(tileLine.First().SegmentNumber) && !_lastActiveSegments.Contains(tileLine.First().SegmentNumber)).ToArray(), parent);
            }
            else
            {
                ActivateAllTiles(tiles.Select(tileLine => tileLine).Where(tileLine => activeSegments.Contains(tileLine.First().SegmentNumber)).ToArray(), parent);
            }
            _lastPlayerSegment = currentPlayerSegment;
            _lastActiveSegments = activeSegments;
        }

        /// <summary>
        /// Coroutine that releases the specified tiles and then continues execution with the provided callback coroutine.
        /// </summary>
        /// <param name="tiles">tiles to release.</param>
        /// <param name="continueWith">callback to execute when done.</param>
        /// <returns></returns>
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
