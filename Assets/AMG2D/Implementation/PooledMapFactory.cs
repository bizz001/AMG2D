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

namespace AMG2D.Implementation
{
    /// <summary>
    /// 
    /// </summary>
    public class PooledMapFactory : IMapElementFactory
    {
        private Dictionary<EGameObjectType, ConcurrentQueue<GameObject>> _tilesPool;

        private Dictionary<EGameObjectType, ConcurrentQueue<GameObject>> _externalObjectsPool;

        private GeneralMapConfig _config;

        private int _lastPlayerSegment;
        private List<int> _lastActiveSegments;
        private readonly object _activationLock = new object();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapConfig"></param>
        public PooledMapFactory(GeneralMapConfig mapConfig)
        {
            _config = mapConfig ?? throw new ArgumentNullException($"Argument {nameof(mapConfig)} cannot be null");

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
        public void ActivateTiles(MonoBehaviour parent, IEnumerable<TileInformation[]> tiles)
        {
            if (_config.EnableSegmentation) ActivateSegmentedTiles(tiles);
            else ActivateAllTiles(tiles);
        }

        private void ActivateAllTiles(IEnumerable<TileInformation[]> tiles)
        {
            if (tiles == null) return;
            foreach (var tilesLine in tiles)
            {
                foreach (var tile in tilesLine)
                {
                    var currentTileType = GetObjectType(tile.TileType);
                    if (tile.CurrentPrefab == null && _tilesPool[currentTileType].TryDequeue(out var pooledTile))
                    {
                        pooledTile.transform.position = new Vector2(tile.X, tile.Y);
                        pooledTile.SetActive(true);
                        tile.CurrentPrefab = pooledTile;
                    }
                    else
                    {
                        tile.CurrentPrefab = MonoBehaviour.Instantiate(_config.ObjectSeeds[currentTileType], new Vector2(tile.X, tile.Y), Quaternion.identity);
                        tile.CurrentPrefab.SetActive(true);
                    }
                }
            }
        }

        private void ActivateSegmentedTiles(IEnumerable<TileInformation[]> tiles)
        {
            lock(_activationLock)
            {
                var currentPlayerSegment = (int)(_config.Camera.transform.position.x / _config.SegmentSize) + 1;
                if (currentPlayerSegment == _lastPlayerSegment) return;

                //add current player segment and neighbouring segments
                var activeSegments = new List<int>();
                activeSegments.Add(currentPlayerSegment);
                activeSegments.Add(currentPlayerSegment - 1);
                activeSegments.Add(currentPlayerSegment + 1);

                if(_lastActiveSegments != null)
                {
                    ReleaseTiles(tiles.Select(tileLine => tileLine).Where(tileLine => _lastActiveSegments.Contains(tileLine.First().SegmentNumber)
                                        && !activeSegments.Contains(tileLine.First().SegmentNumber)));
                    ActivateAllTiles(tiles.Select(tileLine => tileLine).Where(tileLine => activeSegments.Contains(tileLine.First().SegmentNumber)
                                        && !_lastActiveSegments.Contains(tileLine.First().SegmentNumber)));
                }
                else
                {
                    ActivateAllTiles(tiles.Select(tileLine => tileLine).Where(tileLine => activeSegments.Contains(tileLine.First().SegmentNumber)));

                }

                _lastPlayerSegment = currentPlayerSegment;
                _lastActiveSegments = activeSegments;
            }
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
