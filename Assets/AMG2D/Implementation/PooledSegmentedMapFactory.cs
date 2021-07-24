using System;
using System.Collections.Generic;
using AMG2D.Model;
using AMG2D.Model.Persistence;
using AMG2D.Model.Persistence.Enum;
using UnityEngine;
using AMG2D.Configuration;
using AMG2D.Configuration.Enum;
using System.Collections.Concurrent;
using System.Linq;
using UnityEngine.Tilemaps;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace AMG2D.Implementation
{
    /// <summary>
    /// 
    /// </summary>
    public class PooledSegmentedMapFactory : IMapElementFactory
    {
        private Dictionary<EGameObjectType, ConcurrentQueue<GameObject>> _tilesPool;

        private Dictionary<EGameObjectType, ConcurrentQueue<GameObject>> _externalObjectsPool;

        private Dictionary<int, GameObject> _segmentParents;

        private GeneralMapConfig _config;

        private int _lastPlayerSegment;
        private List<int> _lastActiveSegments;
        private readonly object _activationLock = new object();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapConfig"></param>
        public PooledSegmentedMapFactory(GeneralMapConfig mapConfig)
        {
            _config = mapConfig ?? throw new ArgumentNullException($"Argument {nameof(mapConfig)} cannot be null");

            _segmentParents = new Dictionary<int, GameObject>();
            _tilesPool = new Dictionary<EGameObjectType, ConcurrentQueue<GameObject>>();
            foreach (var seed in _config.ObjectSeeds)
            {
                _tilesPool.Add(seed.Key, new ConcurrentQueue<GameObject>());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="tiles"></param>
        public bool ActivateTiles(IEnumerable<TileInformation[]> tiles)
        {
            if (_config.EnableSegmentation) return ActivateSegmentedTiles(tiles);
            else return ActivateAllTiles(tiles);
        }

        private bool ActivateAllTiles(IEnumerable<TileInformation[]> tiles)
        {
            if (tiles == null) return false;
            HashSet<int> activatedSegments = new HashSet<int>();
            foreach (var tilesLine in tiles)
            {
                activatedSegments.Add(tilesLine.First().SegmentNumber);
                foreach (var tile in tilesLine)
                {
                    var currentTileType = GetObjectType(tile.TileType);
                    if (!tile.IsActive && _tilesPool[currentTileType].TryDequeue(out var pooledTile))
                    {
                        pooledTile.transform.position = new Vector2(tile.X, tile.Y);
                        tile.CurrentPrefab = pooledTile;
                    }
                    else
                    {
                        tile.CurrentPrefab = MonoBehaviour.Instantiate(_config.ObjectSeeds[currentTileType], new Vector2(tile.X, tile.Y), Quaternion.identity);
                    }
                    if (!_segmentParents.TryGetValue(tile.SegmentNumber, out GameObject parent))
                    {
                        parent = new GameObject { name = $"MapSegment{tile.SegmentNumber}" };
                        parent.AddComponent<CompositeCollider2D>().generationType = CompositeCollider2D.GenerationType.Manual;
                        parent.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
                        _segmentParents.Add(tile.SegmentNumber, parent);
                    }
                    tile.CurrentPrefab.transform.SetParent(parent.transform);
                    tile.CurrentPrefab.SetActive(true);
                }
            }
            foreach (var segment in activatedSegments)
            {
                _segmentParents[segment].GetComponent<CompositeCollider2D>().GenerateGeometry();
            }
            return true;
        }

        private bool ActivateSegmentedTiles(IEnumerable<TileInformation[]> tiles)
        {
            lock(_activationLock)
            {
                var currentPlayerSegment = (int)(_config.Camera.transform.position.x / _config.SegmentSize) + 1;
                if (currentPlayerSegment == _lastPlayerSegment) return false;

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
                    ReleaseTiles(tiles.Select(tileLine => tileLine)
                        .Where(tileLine => _lastActiveSegments.Contains(tileLine.First().SegmentNumber) && !activeSegments.Contains(tileLine.First().SegmentNumber)));
                    ActivateAllTiles(tiles.Select(tileLine => tileLine)
                        .Where(tileLine => activeSegments.Contains(tileLine.First().SegmentNumber) && !_lastActiveSegments.Contains(tileLine.First().SegmentNumber)));
                }
                else
                {
                    ActivateAllTiles(tiles.Select(tileLine => tileLine).Where(tileLine => activeSegments.Contains(tileLine.First().SegmentNumber)));

                }
                _lastPlayerSegment = currentPlayerSegment;
                _lastActiveSegments = activeSegments;
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tiles"></param>
        public void ReleaseTiles(IEnumerable<TileInformation[]> tiles)
        {
            foreach (var tilesLine in tiles)
            {                
                foreach (var tile in tilesLine)
                {
                    if (tile.CurrentPrefab != null)
                    {
                        tile.CurrentPrefab.SetActive(false);
                        _tilesPool[GetObjectType(tile.TileType)].Enqueue(tile.CurrentPrefab);
                        tile.CurrentPrefab = null;
                    }
                }
            }
        }
        
        private EGameObjectType GetObjectType(ETileType tile)
        {
            return tile switch
            {
                ETileType.Air => EGameObjectType.AirTile,
                ETileType.Cave => EGameObjectType.CaveTile,
                ETileType.Ground => EGameObjectType.GroundTile,
                ETileType.Platform => EGameObjectType.PlatformTile,
                _ => EGameObjectType.Unknown,
            };
        }
        
    }
}
