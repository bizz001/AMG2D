using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using AMG2D.Configuration;
using AMG2D.Model;
using AMG2D.Model.Persistence;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace AMG2D.Implementation
{
    /// <summary>
    /// 
    /// </summary>
    public class TiledMapFactory : ITilesFactory
    {
        private readonly GeneralMapConfig _config;
        private int _lastPlayerSegment;
        private List<int> _lastActiveSegments;
        private Tilemap groundTilemap;
        private Tilemap platformTilemap;
        private bool _isRunning;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapConfig"></param>
        public TiledMapFactory(GeneralMapConfig mapConfig)
        {
            _config = mapConfig ?? throw new ArgumentNullException($"Argument {nameof(mapConfig)} cannot be null");
            var grid = new GameObject("TilemapGrid").AddComponent<Grid>();

            groundTilemap = new GameObject($"{nameof(groundTilemap)}").AddComponent<Tilemap>();
            groundTilemap.gameObject.AddComponent<TilemapRenderer>().sortingOrder = 20;
            groundTilemap.gameObject.AddComponent<TilemapCollider2D>().usedByComposite = true;
            groundTilemap.gameObject.AddComponent<CompositeCollider2D>().generationType = CompositeCollider2D.GenerationType.Manual;
            groundTilemap.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
            groundTilemap.transform.SetParent(grid.transform);

            platformTilemap = new GameObject($"{nameof(platformTilemap)}").AddComponent<Tilemap>();
            platformTilemap.gameObject.AddComponent<TilemapRenderer>().sortingOrder = 20;
            platformTilemap.gameObject.AddComponent<TilemapCollider2D>();
            platformTilemap.gameObject.AddComponent<CompositeCollider2D>().generationType = CompositeCollider2D.GenerationType.Manual;
            platformTilemap.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
            platformTilemap.transform.SetParent(grid.transform);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        public IEnumerator ActivateTiles(MapPersistence map, MonoBehaviour parent, IEnumerator continueWith)
        {
            if (!_isRunning)
            {
                _isRunning = true;
                if (_config.EnableSegmentation)  yield return ActivateSegmentedTiles(map);
                else if (!map.PersistedMap.First().First().IsActive) yield return ActivateAllTiles(map.PersistedMap);
            }
            yield return continueWith;
        }
        private IEnumerator ActivateAllTiles(TileInformation[][] tiles)
        {
            if (tiles == null || !tiles.Any()) yield break;
            var width = tiles.Length;
            var height = tiles.First().Length;
            for (int x = 0; x < width; x++)
            {
                TileBase[] groundTilesToSet = new TileBase[tiles.Length * tiles.First().Length];
                Vector3Int[] groundPositions = new Vector3Int[tiles.Length * tiles.First().Length];
                int groundTilesCounter = 0;

                TileBase[] platformTilesToSet = new TileBase[tiles.Length * tiles.First().Length];
                Vector3Int[] platformPositions = new Vector3Int[tiles.Length * tiles.First().Length];
                int platformTilesCounter = 0;
                for (int y = 0; y < height; y++)
                {
                    switch (tiles[x][y].TileType)
                    {
                        case Model.Persistence.Enum.ETileType.Ground:
                        case Model.Persistence.Enum.ETileType.Grass:
                        case Model.Persistence.Enum.ETileType.Stone:
                            groundTilesToSet[groundTilesCounter] = _config.PlatformTile;
                            groundPositions[groundTilesCounter] = new Vector3Int(tiles[x][y].X, tiles[x][y].Y, 0);
                            groundTilesCounter++;
                            break;
                        case Model.Persistence.Enum.ETileType.Platform:
                            platformTilesToSet[platformTilesCounter] = _config.PlatformTile;
                            platformPositions[platformTilesCounter] = new Vector3Int(tiles[x][y].X, tiles[x][y].Y, 0);
                            platformTilesCounter++;
                            break;
                        default:
                            break;
                    }
                }
                if(x % _config.SegmentLoadingSpeed == 0) yield return null;
                if(groundTilesCounter > 0)
                {
                    groundTilemap.SetTiles(groundPositions, groundTilesToSet);
                }
                if (platformTilesCounter > 0)
                {
                    platformTilemap.SetTiles(platformPositions, platformTilesToSet);
                }
            }
            groundTilemap.gameObject.GetComponent<CompositeCollider2D>().GenerateGeometry();
            yield return null;

            platformTilemap.gameObject.GetComponent<CompositeCollider2D>().GenerateGeometry();

            yield break;
        }

        private IEnumerator ActivateSegmentedTiles(MapPersistence map)
        {
            try
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
                if (_lastActiveSegments != null)
                {
                    yield return ReleaseTiles(tiles.Select(tileLine => tileLine)
                        .Where(tileLine => _lastActiveSegments.Contains(tileLine.First().SegmentNumber) && !activeSegments.Contains(tileLine.First().SegmentNumber)).ToArray());
                    yield return ActivateAllTiles(tiles.Select(tileLine => tileLine)
                        .Where(tileLine => activeSegments.Contains(tileLine.First().SegmentNumber) && !_lastActiveSegments.Contains(tileLine.First().SegmentNumber)).ToArray());
                }
                else
                {
                    yield return ActivateAllTiles(tiles.Select(tileLine => tileLine).Where(tileLine => activeSegments.Contains(tileLine.First().SegmentNumber)).ToArray());
                }
                _lastPlayerSegment = currentPlayerSegment;
                _lastActiveSegments = activeSegments;
            }
            finally
            {
                _isRunning = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tiles"></param>
        public IEnumerator ReleaseTiles(TileInformation[][] tiles, IEnumerator continueWith = null)
        {
            if (tiles != null && tiles.Any())
            {
                Vector3Int[] positionsToRelease = new Vector3Int[tiles.Length * tiles.First().Length];
                TileBase[] defaultTiles = new TileBase[tiles.Length * tiles.First().Length];
                int releaseCounter = 0;
                var width = tiles.Length;
                var height = tiles.First().Length;
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        positionsToRelease[releaseCounter++] = new Vector3Int(tiles[x][y].X, tiles[x][y].Y, 0);
                    }
                    if (x % _config.SegmentLoadingSpeed == 0) yield return null;
                }
                groundTilemap.SetTiles(positionsToRelease, defaultTiles);
                platformTilemap.SetTiles(positionsToRelease, defaultTiles);
            }
            yield return continueWith;
        }
    }
}