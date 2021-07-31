using System;
using System.Collections.Generic;
using System.Linq;
using AMG2D.Configuration;
using AMG2D.Model;
using AMG2D.Model.Persistence;
using AMG2D.Model.Persistence.Enum;
using UnityEngine;
using Random = System.Random;

namespace AMG2D.Implementation
{
    public class CompleteMapGenerator : ICaveGenerator, IGroundGenerator, IPlatformGenerator, IExternalObjectsPositioner
    {
        private GeneralMapConfig _config;
        private Random _seededRandomGen;

        public CompleteMapGenerator(GeneralMapConfig mapConfig)
        {
            _config = mapConfig ?? throw new ArgumentNullException($"Argument {nameof(mapConfig)} cannot be null");
            _seededRandomGen = new Random(_config.GenerationSeed);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="map"></param>
        public void CreateCaves(ref MapPersistence map)
        {
            return;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="map"></param>
        public void CreateGround(ref MapPersistence map)
        {
            for (int x = 0; x < map.PersistedMap.Length; x++)
            {
                var noise = Mathf.PerlinNoise(x / _config.Ground.Smoothness, _config.GenerationSeed);
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
        }

        public void CreatePlatforms(ref MapPersistence map)
        {
            bool isTopReached = false;
            while (!isTopReached)
            {
                int currentX = 0;
                while (currentX < map.PersistedMap.Length)
                {
                    if (GetRandomBool(_config.Platforms.Density / 100))
                    {
                        var platformWidth = GetRandomFromRange(_config.Platforms.MinWidth, _config.Platforms.MaxWidth);
                        var platformHeight = GetRandomFromRange(_config.Platforms.MinimumHeight, _config.Platforms.MaximumHeight);

                        var platformWidthArea = map.PersistedMap.Skip(currentX - 1).Take(platformWidth + 2);
                        var maximumGroundHeight = platformWidthArea.Max(column => column.Where(tile => tile.TileType == ETileType.Grass).Max(tile => tile.Y));
                        var topPlatformTile = maximumGroundHeight + _config.Platforms.Thickness + platformHeight;
                        var bottomPlatformTile = topPlatformTile - _config.Platforms.Thickness;
                        platformWidthArea.Skip(1).Take(platformWidth);
                        if (topPlatformTile < _config.Height - _config.MapBorderThickness)
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
            CreateBorder(ref map);
        }

        public void PositionExternalObjects(ref MapPersistence map)
        {
            foreach (var configuredObject in _config.ExternalObjects.ExternalObjects)
            {
                switch (configuredObject.Position)
                {
                    case Configuration.Enum.EObjectPosition.Any:
                        PositionObjectInAny(ref map, configuredObject);
                        break;
                    case Configuration.Enum.EObjectPosition.Air:
                        PositionObjectInAir(ref map, configuredObject);
                        break;
                    case Configuration.Enum.EObjectPosition.OnGround:
                        PositionObjectOnGround(ref map, configuredObject);
                        break;
                    case Configuration.Enum.EObjectPosition.Soil:
                        PositionObjectInSoil(ref map, configuredObject);
                        break;
                    case Configuration.Enum.EObjectPosition.Cave:
                        PositionObjectInCave(ref map, configuredObject);
                        break;
                    default:
                        break;
                }
            }
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
            if (_config.MapBorderThickness == 0) return;
            for (int y = 0; y < _config.MapBorderThickness; y++)
            {
                for (int x = 0; x < map.PersistedMap.Length; x++)
                {
                    map.PersistedMap[x][y].TileType = ETileType.Stone;
                    map.PersistedMap[x][_config.Height - 1 - y].TileType = ETileType.Stone;
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
