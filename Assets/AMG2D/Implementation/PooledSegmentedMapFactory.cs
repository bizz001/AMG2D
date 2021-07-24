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
            var totalWatch = Stopwatch.StartNew();
            var activeSegments = new List<int>();
            var watch2 = new Stopwatch();
            var watch = new Stopwatch();
            long first = 0, foreach2 = 0, logging = 0, second = 0, third = 0, forth = 0, setTileActive = 0, create = 0, setParent = 0, estoNo = 0;
            foreach (var tilesLine in tiles)
            {
                watch2.Restart();
                foreach (var tile in tilesLine)
                {
                    watch.Restart();

                    var currentTileType = GetObjectType(tile.TileType);
                    if (!tile.IsActive && _tilesPool[currentTileType].TryDequeue(out var pooledTile))
                    {
                        pooledTile.transform.position = new Vector2(tile.X, tile.Y);
                        //pooledTile.SetActive(true);
                        tile.CurrentPrefab = pooledTile;
                    }
                    else
                    {
                        tile.CurrentPrefab = MonoBehaviour.Instantiate(_config.ObjectSeeds[currentTileType], new Vector2(tile.X, tile.Y), Quaternion.identity);
                    }
                    watch.Stop();
                    create += watch.ElapsedMilliseconds;
                    watch.Restart();
                    if (!_segmentParents.TryGetValue(tile.SegmentNumber, out GameObject parent))
                    {
                        parent = new GameObject { name = $"MapSegment{tile.SegmentNumber}" };
                        watch.Stop();
                        first += watch.ElapsedMilliseconds;
                        watch.Restart();
                        //parent.SetActive(false);
                        parent.AddComponent<CompositeCollider2D>().generationType = CompositeCollider2D.GenerationType.Manual;
                        watch.Stop();
                        second += watch.ElapsedMilliseconds;
                        watch.Restart();
                        parent.AddComponent<TilemapCollider2D>().usedByComposite = true;
                        watch.Stop();
                        third += watch.ElapsedMilliseconds;
                        watch.Restart();
                        parent.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
                        watch.Stop();
                        forth += watch.ElapsedMilliseconds;
                        watch.Restart();
                        _segmentParents.Add(tile.SegmentNumber, parent);
                    }
                    tile.CurrentPrefab.transform.SetParent(parent.transform);
                    watch.Stop();
                    setParent += watch.ElapsedMilliseconds;
                    watch.Restart();
                    tile.CurrentPrefab.SetActive(true);
                    watch.Stop();
                    setTileActive += watch.ElapsedMilliseconds; 
                }
                watch2.Stop();
                foreach2 += watch2.ElapsedMilliseconds;
                //_segmentParents[tilesLine.First().SegmentNumber].SetActive(true);
                watch.Restart();
                activeSegments.Add(tilesLine.First().SegmentNumber);
                watch.Stop();
                estoNo += watch.ElapsedMilliseconds;
            }
            watch.Restart();
            Debug.Log($"{nameof(ActivateAllTiles)} time: first: {first}; " +
                $"second: {second}; third: {third}; forth: {forth}; create+reuse: {create}; activateTile: {setTileActive} setTileParent: {setParent}; estoNo: {estoNo}");
            watch.Stop();
            logging += watch.ElapsedMilliseconds;

            totalWatch.Stop();
            var foreachTime = totalWatch.ElapsedMilliseconds;
            totalWatch.Restart();

            foreach (var tilesLine in tiles)
            {
                _segmentParents[tilesLine.First().SegmentNumber].SetActive(true);
            }

            totalWatch.Stop();
            var parentActivation = totalWatch.ElapsedMilliseconds;
            totalWatch.Restart();

            //var activeSegmentColliders = _segmentParents.Where(pair => activeSegments.Contains(pair.Key))
                //.Select(pair => pair.Value.GetComponent<CompositeCollider2D>());

            totalWatch.Stop();
            var colliderSelection = totalWatch.ElapsedMilliseconds;
            totalWatch.Restart();

            //foreach (var collider in activeSegmentColliders)
            //{
            //    collider.GenerateGeometry();
            //}
            totalWatch.Stop();
            var colliderGeneration = totalWatch.ElapsedMilliseconds;

            Debug.Log($"{nameof(ActivateAllTiles)} time: Logging: {logging}; Foreach: {foreachTime}; internalForeach: {foreach2};" +
                $"parentActivation: {parentActivation}; colliderSelection: {colliderSelection}; colliderGeneration: {colliderGeneration}");
            return true;
        }

        private bool ActivateSegmentedTiles(IEnumerable<TileInformation[]> tiles)
        {
            lock(_activationLock)
            {
                var currentPlayerSegment = (int)(_config.Camera.transform.position.x / _config.SegmentSize) + 1;
                if (currentPlayerSegment == _lastPlayerSegment) return false;

                //add current player segment and neighbouring segments
                var activeSegments = new List<int>
                {
                    currentPlayerSegment,
                    currentPlayerSegment - 1,
                    currentPlayerSegment + 1
                };
                var releaseWatch = new Stopwatch();
                var activateWatch = new Stopwatch();
                if(_lastActiveSegments != null)
                {
                    releaseWatch.Start();

                    foreach (var segmentNumber in _lastActiveSegments)
                    {
                        if (!activeSegments.Contains(segmentNumber) && _segmentParents.ContainsKey(segmentNumber))
                        {
                            _segmentParents[segmentNumber].SetActive(false);
                        }

                    }

                    //ReleaseTiles(tiles.Select(tileLine => tileLine).Where(tileLine => _lastActiveSegments.Contains(tileLine.First().SegmentNumber) && !activeSegments.Contains(tileLine.First().SegmentNumber)));

                    releaseWatch.Stop();
                    activateWatch.Start();
                    ActivateAllTiles(tiles.Select(tileLine => tileLine).Where(tileLine => activeSegments.Contains(tileLine.First().SegmentNumber)
                                        && !_lastActiveSegments.Contains(tileLine.First().SegmentNumber)));
                    activateWatch.Stop();
                    Debug.Log($"Time: Release: {releaseWatch.ElapsedMilliseconds}; Activation: {activateWatch.ElapsedMilliseconds}");
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
                _segmentParents[tilesLine.First().SegmentNumber].SetActive(false);
                //foreach (var tile in tilesLine)
                //{
                //    if (tile.CurrentPrefab != null)
                //    {
                //        tile.CurrentPrefab.SetActive(false);
                //        _tilesPool[GetObjectType(tile.TileType)].Enqueue(tile.CurrentPrefab);
                //        tile.CurrentPrefab = null;
                //    }
                //}
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
