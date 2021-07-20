using System;
using UnityEngine;
using static AMG2D.Configuration.BackgroundConfig;

namespace AMG2D.Implementation
{
    /// <summary>
    /// Class designed to hold all parallax background layer information and perform required logic to update position.
    /// </summary>
    internal class ParallaxBackgroundLayer
    {
        private const int MAP_PADDING = 1;
        private BackgroundLayerConfig _config;
        private Vector2 _initalPosition;
        private int _height;
        private GameObject BackgroundPrefab { get; }
        private float _referenceXPosition;
        private float _width;
        private readonly GameObject _camera;

        /// <summary>
        /// Create a new instance of <see cref="ParallaxBackgroundLayer"/> using the provided configuration information.
        /// </summary>
        /// <param name="config"></param>
        /// <param name="position"></param>
        /// <param name="height"></param>
        /// <param name="sortOrder"></param>
        internal ParallaxBackgroundLayer(BackgroundLayerConfig config, GameObject camera, Vector2 position, int height, short sortOrder)
        {
            _config = config ?? throw new ArgumentException($"Argument {nameof(config)} cannot be null.");
            _camera = camera ?? throw new ArgumentException($"Argument {nameof(camera)} cannot be null.");
            _initalPosition = position;
            _height = height;
            //_config.BaseImage.GetComponent<SpriteRenderer>().sortingOrder = sortOrder;
            BackgroundPrefab = CreateBackgroundLayerPrefeb(config);
            _referenceXPosition = BackgroundPrefab.transform.position.x;
        }

        /// <summary>
        /// Create a the final background layer prefab that will be shown based on configured information.
        /// </summary>
        /// <param name="config">Background layer configuration</param>
        /// <returns></returns>
        private GameObject CreateBackgroundLayerPrefeb(BackgroundLayerConfig config)
        {
            //First update position and scale to match current map dimensions.
            config.BaseImage.transform.position = _initalPosition + new Vector2(0, (_height / 2) - MAP_PADDING);
            SetNewHeight(ref config.BaseImage, _height + MAP_PADDING);

            //Create parent to hold all background instances.
            var finalPrefab = new GameObject();
            if (config.ParallaxIntensity > 0.5f)
            {
                finalPrefab.transform.SetParent(_camera.transform);
            }
            finalPrefab.transform.position = config.BaseImage.transform.position;
            config.BaseImage.transform.SetParent(finalPrefab.transform);

            //adjust position of initial object so that prefab position is centered on resulting image composition
            var initialOffset = config.BaseImage.GetComponent<Renderer>().bounds.size.x * (config.Repetition - 1) / 2;
            config.BaseImage.transform.position = new Vector3(
                config.BaseImage.transform.position.x - initialOffset,
                config.BaseImage.transform.position.y);

            //create all background instances and link to parent
            for (int i = 1; i < config.Repetition; i++)
            {
                var newPosition = new Vector2(config.BaseImage.transform.position.x + config.BaseImage.GetComponent<Renderer>().bounds.size.x * i,
                    config.BaseImage.transform.position.y);
                var clone = MonoBehaviour.Instantiate(config.BaseImage, newPosition, Quaternion.identity);
                clone.transform.SetParent(finalPrefab.transform);
            }
            return finalPrefab;
        }

        /// <summary>
        /// Adjust image size to a new height while maintaining aspect ratio.
        /// </summary>
        /// <param name="objectToScale"></param>
        /// <param name="newHeight"></param>
        private void SetNewHeight(ref GameObject objectToScale, float newHeight)
        {
            float size = objectToScale.GetComponent<Renderer>().bounds.size.y;
            Vector3 rescale = objectToScale.transform.localScale;
            rescale.y = newHeight * rescale.y / size;
            rescale.x = rescale.y;
            objectToScale.transform.localScale = rescale;

            //update local width info with new width
            _width = objectToScale.GetComponent<Renderer>().bounds.size.x;
        }

        /// <summary>
        /// Update the position of this background layer based on the new camera position.
        /// </summary>
        /// <param name="newCameraPosition">New camera poisition</param>
        public void UpdatePosition()
        {
            var newCameraPosition = _camera.transform.position.x;
            float avanceParallax = _referenceXPosition + (newCameraPosition - _referenceXPosition) * _config.ParallaxIntensity;
            float distCameraParallax = newCameraPosition - avanceParallax;

            var targetPos = new Vector3(avanceParallax,
                BackgroundPrefab.transform.position.y,
                BackgroundPrefab.transform.position.z);
            BackgroundPrefab.transform.position = targetPos;
            if (Math.Abs(distCameraParallax) >= _width) _referenceXPosition = newCameraPosition;
        }
    }
}
