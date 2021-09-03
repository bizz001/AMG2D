using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using AMG2D.Configuration;
using AMG2D.Model;
using AMG2D.Model.Persistence;
using UnityEngine;
using Random = System.Random;

namespace AMG2D.Implementation
{
    /// <summary>
    /// Service class that implements all interfaces that alter the persisted map, adding or modifying different elements.
    /// </summary>
    public class CompleteMapGenerator : ICaveGenerator, IGroundGenerator, IPlatformGenerator, IExternalObjectsPositioner
    {
        private CompleteConfiguration _config;
        private Random _seededRandomGen;

        /// <summary>
        /// Create an instance of <see cref="CompleteMapGenerator"/> using the provided configuration
        /// </summary>
        /// <param name="config"></param>
        public CompleteMapGenerator(CompleteConfiguration config)
        {
            _config = config ?? throw new ArgumentNullException($"Argument {nameof(config)} cannot be null");
            _seededRandomGen = new Random(_config.GeneralMapSettings.GenerationSeed);
        }

        /// <summary>
        /// Coroutine that mdifies the provided <see cref="MapPersistence"/> in order to add caves according to the configuration injected at creation.
        /// Executes provided coroutine at the end.
        /// WARNING: NOT IMPLEMENTED.
        /// </summary>
        /// <param name="map">map to modify.</param>
        /// <param name="continueWith">coroutine to execute at the end.</param>
        /// <returns></returns>
        public IEnumerator CreateCaves(MapPersistence map, IEnumerator continueWith)
        {
            yield return continueWith;
        }

        /// <summary>
        /// Coroutine that modifies the provided <see cref="MapPersistence"/> in order to add ground according to the configuration injected at creation.
        /// Executes provided coroutine at the end.
        /// </summary>
        /// <param name="map">map to modify.</param>
        /// <param name="continueWith">coroutine to execute at the end.</param>
        /// <returns></returns>
        public IEnumerator CreateGround(MapPersistence map, IEnumerator continueWith)
        {
            for (int x = 0; x < map.PersistedMap.Length; x++)
            {
                var noise = Mathf.PerlinNoise(x / _config.Ground.Smoothness, _config.GeneralMapSettings.GenerationSeed);
                var terrainHeight = Mathf.RoundToInt(_config.Ground.InitialHeight * noise);
                foreach (var tile in map.PersistedMap[x])
                {
                    if (tile.Y < terrainHeight)
                    {
                        tile.TileType = ETileType.Ground;
                    }
                    else
                    {
                        if (tile.Y == terrainHeight) tile.TileType = ETileType.Grass;

                    }
                }
            }
            yield return continueWith;
        }

        /// <summary>
        /// Modifies the provided <see cref="MapPersistence"/> in order to add ground according to the configuration injected at creation.
        /// Executes provided callback coroutine at the end.
        /// </summary>
        /// <param name="map">map to modify.</param>
        /// <param name="continueWith">coroutine to execute at the end.</param>
        /// <returns></returns>
        public IEnumerator CreatePlatforms(MapPersistence map, IEnumerator continueWith)
        {
            if (_config.Platforms.EnablePlatformsGeneration && _config.Platforms.Density != 0)
            {
                bool isTopReached = false;
                while (!isTopReached)
                {
                    int currentX = 0;
                    while (currentX < map.PersistedMap.Length)
                    {
                        if (GetRandomBool(_config.Platforms.Density / 100f))
                        {
                            var platformWidth = GetRandomFromRange(_config.Platforms.MinWidth, _config.Platforms.MaxWidth);
                            var platformHeight = GetRandomFromRange(_config.Platforms.MinimumHeight, _config.Platforms.MaximumHeight);

                            var platformWidthArea = map.PersistedMap.Skip(currentX - 1).Take(platformWidth + 2);
                            var maximumGroundHeight = platformWidthArea.Max(column => column.Where(tile => tile.TileType == ETileType.Grass).Max(tile => tile.Y));
                            var topPlatformTile = maximumGroundHeight + _config.Platforms.Thickness + platformHeight;
                            var bottomPlatformTile = topPlatformTile - _config.Platforms.Thickness;
                            platformWidthArea.Skip(1).Take(platformWidth);
                            if (topPlatformTile < _config.GeneralMapSettings.Height - _config.GeneralMapSettings.MapBorderThickness)
                            {
                                foreach (var column in platformWidthArea)
                                {
                                    foreach (var tile in column)
                                    {
                                        if (tile.Y == topPlatformTile) tile.TileType = ETileType.Grass;
                                        if (tile.Y < topPlatformTile && tile.Y > bottomPlatformTile) tile.TileType = ETileType.Ground;
                                    }
                                }
                                currentX += platformWidth;
                            }
                            else
                            {
                                isTopReached = true;
                            }
                        }
                        currentX++;
                    }
                }
            }
            CreateBorder(ref map);
            yield return continueWith;
        }

        /// <summary>
        /// Coroutine that modifies the provided <see cref="MapPersistence"/> in order to add external objects according to the configuration injected at creation.
        /// </summary>
        /// <param name="map">map to modify.</param>
        /// <param name="continueWith">coroutine to execute at the end.</param>
        /// <returns></returns>
        public IEnumerator PositionExternalObjects(MapPersistence map, IEnumerator continueWith)
        {
            foreach (var configuredObject in _config.ExternalObjects.ExternalObjects)
            {
                switch (configuredObject.Position)
                {
                    case EObjectPosition.Any:
                        PositionObjectInAny(ref map, configuredObject);
                        break;
                    case EObjectPosition.Air:
                        PositionObjectInAir(ref map, configuredObject);
                        break;
                    case EObjectPosition.OnGround:
                        PositionObjectOnGround(ref map, configuredObject);
                        break;
                    case EObjectPosition.Soil:
                        PositionObjectInSoil(ref map, configuredObject);
                        break;
                    case EObjectPosition.Cave:
                        PositionObjectInCave(ref map, configuredObject);
                        break;
                    default:
                        break;
                }
            }
            yield return continueWith;
        }

        private void PositionObjectInCave(ref MapPersistence map, ExternalObjectConfig objectToPlace)
        {
            return;
        }

        private void PositionObjectInSoil(ref MapPersistence map, ExternalObjectConfig objectToPlace)
        {
            var placementCandidates = map.PersistedMap.SelectMany(column => column.Where(tile => tile.TileType == ETileType.Ground));
            foreach (var candidate in placementCandidates)
            {
                if (GetRandomBool(objectToPlace.Density))
                {
                    map.ExternalObjects.Add(new ExternalObjectInfo { AsignedTile = candidate, TypeID = objectToPlace.UniqueID, Template = objectToPlace.ObjectTemplate });
                }
            }
        }

        private void PositionObjectOnGround(ref MapPersistence map, ExternalObjectConfig objectToPlace)
        {
            var placementCandidates = new List<TileInformation>();

            for (int x = 0; x < map.PersistedMap.Length; x++)
            {
                for (int y = 0; y < map.PersistedMap[x].Length; y++)
                {
                    if (map.PersistedMap[x][y].TileType == ETileType.Grass)
                    {
                        placementCandidates.Add(map.PersistedMap[x][y + 1]);
                    }
                }
            } 
            foreach (var candidate in placementCandidates)
            {
                if (GetRandomBool(objectToPlace.Density))
                {
                    map.ExternalObjects.Add(new ExternalObjectInfo { AsignedTile = candidate, TypeID = objectToPlace.UniqueID, Template = objectToPlace.ObjectTemplate });
                }
            }
        }

        private void PositionObjectInAir(ref MapPersistence map, ExternalObjectConfig objectToPlace)
        {
            var placementCandidates = map.PersistedMap.SelectMany(column => column.Where(tile => tile.TileType == ETileType.Air));
            foreach (var candidate in placementCandidates)
            {
                if (GetRandomBool(objectToPlace.Density))
                {
                    map.ExternalObjects.Add(new ExternalObjectInfo { AsignedTile = candidate, TypeID = objectToPlace.UniqueID, Template = objectToPlace.ObjectTemplate });
                }
            }
        }

        private void PositionObjectInAny(ref MapPersistence map, ExternalObjectConfig objectToPlace)
        {
            PositionObjectInAir(ref map, objectToPlace);
            PositionObjectInCave(ref map, objectToPlace);
            PositionObjectOnGround(ref map, objectToPlace);
            PositionObjectInSoil(ref map, objectToPlace);
        }

        private void CreateBorder(ref MapPersistence map)
        {
            if (_config.GeneralMapSettings.MapBorderThickness == 0) return;
            for (int y = 0; y < _config.GeneralMapSettings.MapBorderThickness; y++)
            {
                for (int x = 0; x < map.PersistedMap.Length; x++)
                {
                    map.PersistedMap[x][y].TileType = ETileType.Stone;
                    map.PersistedMap[x][_config.GeneralMapSettings.Height - 1 - y].TileType = ETileType.Stone;
                }
                foreach (var item in map.PersistedMap[y])
                {
                    item.TileType = ETileType.Stone;
                }

                foreach (var item in map.PersistedMap[map.PersistedMap.Length - y - 1])
                {
                    item.TileType = ETileType.Stone;
                }
            }
        }

        /// <summary>
        /// Generates a random bool value based on the probability provided to return a true.
        /// </summary>
        /// <param name="trueProbability"></param>
        /// <returns></returns>
        private bool GetRandomBool(float trueProbability)
        {
            return (_seededRandomGen.NextDouble() * 100) <= trueProbability;
        }

        private int GetRandomFromRange(int from, int to)
        {
            var range = to - from;
            return (int)(from + range * _seededRandomGen.NextDouble());
        }
    }
}
