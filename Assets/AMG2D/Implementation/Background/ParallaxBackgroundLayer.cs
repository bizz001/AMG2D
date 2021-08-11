using System;
using AMG2D.Configuration;
using UnityEngine;
using static AMG2D.Configuration.BackgroundConfig;

namespace AMG2D.Implementation.Background
{
    /// <summary>
    /// Class designed to hold all parallax background layer information and perform required logic to update position.
    /// </summary>
    internal class ParallaxBackgroundLayer
    {
        private BackgroundLayerConfig _config;
        private Vector2 _initalPosition;
        private int _height;
        private GameObject BackgroundPrefab { get; }
        private float _referenceXPosition;
        private float _width;
        private readonly CompleteConfiguration _generalConfig;

        /// <summary>
        /// Create a new instance of <see cref="ParallaxBackgroundLayer"/> using the provided configuration information.
        /// </summary>
        /// <param name="layerConfig"></param>
        /// <param name="position"></param>
        /// <param name="height"></param>
        /// <param name="sortOrder"></param>
        internal ParallaxBackgroundLayer(BackgroundLayerConfig layerConfig, CompleteConfiguration completeConfig, Vector2 position, int height)
        {
            _config = layerConfig ?? throw new ArgumentException($"Argument {nameof(layerConfig)} cannot be null.");
            _generalConfig = completeConfig ?? throw new ArgumentException($"Argument {nameof(completeConfig)} cannot be null.");
            _initalPosition = position;
            _height = height;
            BackgroundPrefab = CreateBackgroundLayerPrefeb(layerConfig);
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
            config.BaseImage.transform.position = _initalPosition + new Vector2(0, (_height / 2) - _generalConfig.Background.MapPadding);
            SetNewHeight(ref config.BaseImage, _height + _generalConfig.Background.MapPadding);

            //Create parent to hold all background instances.
            var finalPrefab = new GameObject();
            if (config.ParallaxIntensity >= 0.5f)
            {
                finalPrefab.transform.SetParent(_generalConfig.Camera.transform);
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
            //X Axis parallax
            var newCameraXPosition = _generalConfig.Camera.transform.position.x;
            float newXPosition = _referenceXPosition + (newCameraXPosition - _referenceXPosition) * _config.ParallaxIntensity;
            float newXOffset = newCameraXPosition - newXPosition;

            //Y Axis parallax
            var mapCenter = _initalPosition.y + (_height / 2);
            var newCameraYPosition = _generalConfig.Camera.transform.position.y;
            float newYPosition = mapCenter + (newCameraYPosition - _initalPosition.y - (_generalConfig.Background.HorizonHeight * _height)) * _config.ParallaxIntensity * _generalConfig.Background.VerticalParallaxModifier;

            var targetPos = new Vector2(newXPosition, newYPosition);
            BackgroundPrefab.transform.position = targetPos;
            if (Math.Abs(newXOffset) >= _width) _referenceXPosition = newCameraXPosition;
        }
    }
}
