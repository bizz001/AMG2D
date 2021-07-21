using System;
using System.Collections;
using System.Collections.Generic;
using AMG2D.Bootstrap;
using AMG2D.Configuration;
using AMG2D.Configuration.Enum;
using AMG2D.Model;
using AMG2D.Model.Persistence;
using AMG2D.Model.Persistence.Enum;
using UnityEngine;

namespace AMG2D
{
    public class MapGenerator : MonoBehaviour
    {
        [SerializeField] GameObject dirt, grass, stone;

        private IMapElementFactory _elementFactory;
        private ITileEnhancer _tileEnhancer;
        private IGroundGenerator _groundGenerator;
        private IPlatformGenerator _platformGenerator;
        private ICaveGenerator _caveGenerator;
        private IBackgroundService _background;

        private MapPersistence _baseMap;

        [SerializeReference]
        public GeneralMapConfig Config;

        void Start()
        {
            _baseMap = new MapPersistence(Config.Width, Config.Height, Config.SegmentSize);
            if (Config == null) Config = new GeneralMapConfig();
            var seeds = new Dictionary<EGameObjectType, GameObject>()
            {
                { EGameObjectType.GroundTile, grass },
                { EGameObjectType.AirTile, stone }

            };
            Config.ObjectSeeds = seeds;
            ServiceLocator.Build(Config);
            ResolveServices();

            _background.SetMapLimits(transform.position, Config.Height);

            Generate();
            
            Debug.Log(_baseMap.ToString());
        }

        public void Generate()
        {
            _elementFactory.ReleaseTiles(_baseMap.PersistedMap);
            _baseMap.ClearMap();
            _groundGenerator.CreateGround(ref _baseMap);
            
            _elementFactory.ActivateTiles(this, _baseMap.PersistedMap);
        }

        private void FixedUpdate()
        {
            _elementFactory.ActivateTiles(this, _baseMap.PersistedMap);
        }
        
        private void LateUpdate()
        {
            _background.UpdateBackground();
            
        }

        private void ResolveServices()
        {
            _elementFactory = ServiceLocator.GetService<IMapElementFactory>()
                ?? throw new ArgumentNullException($"Service {nameof(IMapElementFactory)} cannot be null");
            _tileEnhancer = ServiceLocator.GetService<ITileEnhancer>()
                ?? throw new ArgumentNullException($"Service {nameof(ITileEnhancer)} cannot be null");
            _groundGenerator = ServiceLocator.GetService<IGroundGenerator>()
                ?? throw new ArgumentNullException($"Service {nameof(IGroundGenerator)} cannot be null");
            _caveGenerator = ServiceLocator.GetService<ICaveGenerator>()
                ?? throw new ArgumentNullException($"Service {nameof(ICaveGenerator)} cannot be null");
            _platformGenerator = ServiceLocator.GetService<IPlatformGenerator>()
                ?? throw new ArgumentNullException($"Service {nameof(IPlatformGenerator)} cannot be null");
            _background = ServiceLocator.GetService<IBackgroundService>()
                ?? throw new ArgumentNullException($"Service {nameof(IBackgroundService)} cannot be null");
        }
    }
}
