using System;
using System.Collections.Generic;
using AMG2D.Configuration;
using AMG2D.Model;
using AMG2D.Model.Persistence;
using AMG2D.Model.Persistence.Enum;
using Random = UnityEngine.Random;

public class CompleteMapGenerator : ICaveGenerator, IGroundGenerator, IPlatformGenerator
{
    private int width, height;
    private int minStoneheight = 1, maxStoneHeight = 2;
    private GeneralMapConfig _config;

    public CompleteMapGenerator(GeneralMapConfig mapConfig)
    {
        _config = mapConfig ?? throw new ArgumentNullException($"Argument {nameof(mapConfig)} cannot be null");
    }

    /// <summary>
    /// Hola <see cref="CompleteMapGenerator"/>
    /// </summary>
    /// <param name="map"></param>
    void ICaveGenerator.CreateCaves(ref MapPersistence map)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="map"></param>
    void IGroundGenerator.CreateGround(ref MapPersistence map)
    {
        width = map.Width;
        height = map.Height / 4;
        for (int x = 0; x < width; x++)//This will help spawn a tile on the x axis
        {
            // now for procedural generation we need to gradually increase and decrease the height value
            int minHeight = height - 1;
            int maxHeight = height + 2;
            height = Random.Range(minHeight, maxHeight);
            int minStoneSpawnDistance = height - minStoneheight;
            int maxStoneSpawnDistance = height - maxStoneHeight;
            int totalStoneSpawnDistance = Random.Range(minStoneSpawnDistance, maxStoneSpawnDistance);
            //Perlin noise.
            for (int y = 0; y < height; y++)//This will help spawn a tile on the y axis
            {
                map.PersistedMap[x][y].TileType = ETileType.Ground;

                if (y < totalStoneSpawnDistance)
                {
                    //spawnObj(stone, x, y);
                }
                else
                {
                    //spawnObj(dirt, x, y);
                }

            }
            if (totalStoneSpawnDistance == height)
            {
                //spawnObj(stone, x, height);
            }
            else
            {
                //spawnObj(grass, x, height);
            }
        }
    }

    void IPlatformGenerator.CreatePlatforms(ref MapPersistence map)
    {
        throw new NotImplementedException();
    }
}
