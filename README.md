# AMG2D - Automatic Map Generator 2D

This project provides a [Unity](https://unity.com) script that can generate platform-style maps for 2D games.

## Description

This project contains a full Unity project that provides a testing environment for the main tool. The main folder is the Assets folder, and it contains the following:

* [2D Car](https://assetstore.unity.com/packages/tools/physics/2d-car-73763). A 2D car asset for testing purposes. Available on the Unity Asset Store.
* AMG2D. This is the main folder of this project. It contains the main script in the form of MapGenerator.cs file. This file contains the MapGenerator class, extending [MonoBehaviour](https://docs.unity3d.com/ScriptReference/MonoBehaviour.html) class that provides various configuration parameters in order to generate a complete game map. It also contains various other classes that provide different functionalities to the main class.
* [Hero Knight - Pixel Art](https://assetstore.unity.com/packages/2d/characters/hero-knight-pixel-art-165188). A 2D character for testing purposes. Available on the Unity Asset Store.
* [Pixel Adventure 2](https://assetstore.unity.com/packages/2d/characters/pixel-adventure-2-155418) An asset package for testing purposes. Available on the Unity Asset Store.
* [Platformer Tileset](https://assetstore.unity.com/packages/2d/characters/free-platformer-tileset-182519). A Tileset package for testing purposes. Available on the Unity Asset Store.


The configuration parameters include the following:

### General configuration

| Name | Type | Description |
|---|---|---|
|Height|int|Height of the generated map.|
|Width|int|Width of the generated map.|
|Map Border Thickness|int|Thickness of the map border. If set to 0 the map will have no border.|
|Ground Tile|TileBase|TileBase object reference that will be used to complete ground tiles.|
|Platform Tile|TileBase|TileBase object reference that will be used to complete ground tiles.|
|Generation Seed|int|Generation seed of the entire map. Each seed will generate a deterministic random map.|
|Camera|GameObject|Camera object for movement tracking. Used to determine active segments when segmentation is enabled.|
|Enable segmentation|bool|Indicates whether map segmentation should be enabled.|
|Segment Size|int|Size in tiles of each individual segment.|
|Number of segments|int|Number of segments that will compose the map.|
|Segment loading speed|int|Speed at which each individual segment is loaded. Setting this value to a number greater than the value of SegmentSize parameter will cause segments to load instantly. Might affect performance.|
|Objects to enable|List{GameObject}| List of external objects that will be set to active once the map is finised loading. Useful for enabling player charater for example so that it appears when map is fully loaded.|

### Background Configuration

| Name | Type | Description |
|---|---|---|
| Vertical Parallax Modifier |float|Value by which the parallax intensity of each layer is multiplied in order to apply vertical parallax. Recommended values between 1.0 and 0.1|
|Horizon height|float|Height of the horizon as a proportion of the total height. 0.5 will set the horizon in the middle of the map height.|
|Map padding|int|Vertical background padding relative to the map vertical size.|
|Background layers|BackgroundLayerConfig[]|Array containing the background configuration of each parallax layer.|

### Background layer configuration

| Name | Type | Description |
|---|---|---|
|Base image|GameObject|GameObject reference representing the image to be set as layer. The user is responsible of setting the Sorting Order of base image of each layer so that the layers with lower parallax intensity appear behind layers with higher parallax intensity.
|Parallax intensity|float|The intensity of the parallax effect. Must be a value between -1 and 1. A value of 0 means the layer will be static relative to the map. A value of 1 means the layer will be static relative to the camera. A value less than 0 means the layer will be in front of the map.|
|Repetition|int|Number of times to repeat the layer in order to cover all camera view. Increase this value if you see the edge of the background when moving.|

### Ground configuration

| Name | Type | Description |
|---|---|---|
|Initial height|int|The starting and the avrage height of the generated ground.|
|Smoothness|float|The smoothness of the generated ground.|

### Platforms configuration

| Name | Type | Description |
|---|---|---|
|Enable platforms generation|bool|Indicates wether platform generation should be enabled.|
|Maximum height|int|Indicates the maximum height, in tiles, of the platforms.|
|Minimum height|int|Indicates the minimum height, in tiles, of the platform.|
|Thickness|int|Indicates the thickness of the platforms. Tiles.|
|Density|int|Indicates the density of the generated platforms.|
|Min width|int|Indicates the minimum width of the generated platforms.|
|Max width|int|Indicates the maximum width of the generated platforms.|

### External objects configuration

External objects are configured using an array of External objects configuration:

| Name | Type | Description |
|---|---|---|
|Unique ID|string|Unique ID of this External object configuration|
|Object Template|GameObject|Object template that will be cloned across the map.|
|Density|int|Density of the objects spawned.|

## Getting Started

For now, in order to use this project, simply download or clone the repository and then copy the AMD2D folder from this project into your project. Then add the MapGenerator script as a Component to a blank GameObject and configure the generator acording to your needs based on the parameters documentation listed above.

## Authors

Daniel Ionel Bizau
dibizau@gmail.com

## Version History

* 1.0.0
    * First version. Only available on GitHub.

## License

This project is licensed under the MIT License - see the LICENSE.md file for details.

