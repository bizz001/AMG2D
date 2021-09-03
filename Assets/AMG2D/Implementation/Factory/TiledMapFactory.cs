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
    /// <see cref="ITilesFactory"/> implementation that creates a map based on <see cref="Tilemap"/> objects.
    /// </summary>
    public class TiledMapFactory : ITilesFactory
    {
        private readonly GeneralMapConfig _config;
        private int _lastPlayerSegment;
        private int _lastTransitionedSegment;
        private List<int> _lastActiveSegments;
        private Tilemap groundTilemap;
        private Tilemap platformTilemap;
        private bool _isRunning;

        /// <summary>
        /// Creates an instance of <see cref="TiledMapFactory"/> using the provided configuration.
        /// </summary>
        /// <param name="mapConfig">configuration for this instance.</param>
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
        /// Coroutine that activates the Tiles objects of the specified map and then executes the callback coroutine.
        /// </summary>
        /// <param name="map">map to activate.</param>
        /// <param name="parent">parent object of the map.</param>
        /// <param name="continueWith">callback to execute when done.</param>
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
                        case ETileType.Ground:
                        case ETileType.Grass:
                        case ETileType.Stone:
                            groundTilesToSet[groundTilesCounter] = _config.PlatformTile;
                            groundPositions[groundTilesCounter] = new Vector3Int(tiles[x][y].X, tiles[x][y].Y, 0);
                            groundTilesCounter++;
                            break;
                        case ETileType.Platform:
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
            yield return null;
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

                //Hysteresis to prevent erratic loading/unloading
                if (currentPlayerSegment == _lastTransitionedSegment) yield break;

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
                _lastTransitionedSegment = _lastPlayerSegment;
                _lastPlayerSegment = currentPlayerSegment;
                _lastActiveSegments = activeSegments;
            }
            finally
            {
                _isRunning = false;
            }
        }

        /// <summary>
        /// Coroutine that releases the specified tiles and then continues execution with the provided callback coroutine.
        /// </summary>
        /// <param name="tiles">tiles to release.</param>
        /// <param name="continueWith">callback to execute when done.</param>
        /// <returns></returns>
        public IEnumerator ReleaseTiles(TileInformation[][] tiles, IEnumerator continueWith = null)
        {
            if (tiles != null && tiles.Any())
            {
                TileBase[] defaultTiles = new TileBase[tiles.Length * tiles.First().Length];
                int releaseCounter = 0;
                var width = tiles.Length;
                var height = tiles.First().Length;
                for (int x = 0; x < width; x++)
                {
                    Vector3Int[] positionsToRelease = new Vector3Int[tiles.Length * tiles.First().Length];

                    for (int y = 0; y < height; y++)
                    {
                        positionsToRelease[releaseCounter++] = new Vector3Int(tiles[x][y].X, tiles[x][y].Y, 0);
                    }
                    groundTilemap.SetTiles(positionsToRelease, defaultTiles);
                    platformTilemap.SetTiles(positionsToRelease, defaultTiles);
                    if (x % _config.SegmentLoadingSpeed == 0) yield return null;
                }
            }
            yield return continueWith;
        }
    }
}