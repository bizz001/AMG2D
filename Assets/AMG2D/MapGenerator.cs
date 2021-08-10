using System;
using System.Collections;
using System.Collections.Generic;
using AMG2D.Bootstrap;
using AMG2D.Configuration;
using AMG2D.Configuration.Enum;
using AMG2D.Model;
using AMG2D.Model.Persistence;
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
        private ITilesFactory _tilesFactory;
        private IExternalObjectsPositioner _externalObjectsPositioner;
        private ICaveGenerator _caveGenerator;
        private IBackgroundService _background;

        private bool courutineCompleted;
        private MapPersistence _baseMap;

        [SerializeReference]
        public GeneralMapConfig Config;
        private bool _initializationCoroutineFinished;

        void Start()
        {
            _baseMap = new MapPersistence(Config.Width, Config.Height, Config.SegmentSize);
            if (Config == null) Config = new GeneralMapConfig();
            var seeds = new Dictionary<string, GameObject>()
            {
                { EGameObjectType.GroundTile.ToString(), dirt },
                { EGameObjectType.GrassTile.ToString(), grass },
                { EGameObjectType.StoneTile.ToString(), stone }
            };
            Config.ObjectSeeds = seeds;
            ServiceLocator.Build(Config);
            ResolveServices();

            _background.SetMapLimits(transform.position, Config.Height);

            IEnumerator startSequence =
                StartCoroutineSequence(
                _groundGenerator.CreateGround(_baseMap,
                _platformGenerator.CreatePlatforms(_baseMap,
                _externalObjectsPositioner.PositionExternalObjects(_baseMap,
                _tilesFactory.ActivateTiles(_baseMap, this,
                _elementFactory.ActivateExternalObjects(_baseMap,
                EnableExternalObjects(
                EndCoroutineSequence()
                )))))));
            StartCoroutine(startSequence);
        }

        private IEnumerator EnableExternalObjects(IEnumerator continueWith)
        {
            foreach (var obj in Config.ObjectsToEnable)
            {
                obj.SetActive(true);
            }
            yield return continueWith;
        }

        private IEnumerator EndCoroutineSequence()
        {
            courutineCompleted = true;
            yield break;
        }

        private IEnumerator StartCoroutineSequence(IEnumerator continueWith)
        {
            courutineCompleted = false;
            yield return continueWith;
        }

        private void FixedUpdate()
        {
            if (!courutineCompleted) return;
            courutineCompleted = false;
            IEnumerator updateSequence =
                StartCoroutineSequence(
                _tilesFactory.ActivateTiles(_baseMap, this,
                _elementFactory.ActivateExternalObjects(_baseMap,
                EndCoroutineSequence()
                )));
            StartCoroutine(updateSequence);
        }
        
        private void LateUpdate()
        {
             _background.UpdateBackground();
            
        }

        private void ResolveServices()
        {
            try
            {
                _elementFactory = ServiceLocator.GetService<IMapElementFactory>()
                    ?? throw new ArgumentNullException($"Service {nameof(IMapElementFactory)} cannot be null");
                _tilesFactory = ServiceLocator.GetService<ITilesFactory>()
                    ?? throw new ArgumentNullException($"Service {nameof(ITilesFactory)} cannot be null");
                _tileEnhancer = ServiceLocator.GetService<ITileEnhancer>()
                    ?? throw new ArgumentNullException($"Service {nameof(ITileEnhancer)} cannot be null");
                _groundGenerator = ServiceLocator.GetService<IGroundGenerator>()
                    ?? throw new ArgumentNullException($"Service {nameof(IGroundGenerator)} cannot be null");
                _caveGenerator = ServiceLocator.GetService<ICaveGenerator>()
                    ?? throw new ArgumentNullException($"Service {nameof(ICaveGenerator)} cannot be null");
                _externalObjectsPositioner = ServiceLocator.GetService<IExternalObjectsPositioner>()
                    ?? throw new ArgumentNullException($"Service {nameof(IExternalObjectsPositioner)} cannot be null");
                _platformGenerator = ServiceLocator.GetService<IPlatformGenerator>()
                    ?? throw new ArgumentNullException($"Service {nameof(IPlatformGenerator)} cannot be null");
                _background = ServiceLocator.GetService<IBackgroundService>()
                    ?? throw new ArgumentNullException($"Service {nameof(IBackgroundService)} cannot be null");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to resolve services due to: {ex}");
                Application.Quit();
            }
        }
    }
}
