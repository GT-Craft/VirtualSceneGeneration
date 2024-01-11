# GT-CRAFT: Virtual Scene Generation

GT-Craft is a framework for fast-prototyping geospatial-based digital twins in Unity 3D.
It uses a streamed satellite map image and elevation data to create a virtual scene in Unity 3D.
For more detail, please check [our paper]().
The map streaming and semantic segmentation phases are implemented in [this repo](https://github.com/GT-Craft/Map_Streaming_SemanticExtraction).

<img src="https://github.com/GT-Craft/VirtualSceneGeneration/blob/main/Figure/dt.gif?raw=true">

------------------------------------------------------------------------------------------------

## Setup
#### Prerequisite
The current implementation is developed and test on Ubuntu 22.04, [Unity 2020.3](https://unity.com/releases/editor/archive), and [Unity USD SDK 1.0.3](https://github.com/Unity-Technologies/usd-unity-sdk).

#### Map Data Preparation
**The sample map data are already included under `VirtualSceneGeneration/Assets/MapData`.**

If you want to generate the virtual scene for your target area, you need to get mask images of the object semantics and elevation matrix by using [this repo](https://github.com/GT-Craft/Map_Streaming_SemanticExtraction).
The new masks and elevation matrix need to be placed under `Assets/MapData`.

------------------------------------------------------------------------------------------------

## Scene Generation Components

#### MapDataLoader
It loads the map image as texture, and semantic/elevation matrices under `Assets/MapData`.

#### MapCreator
1. Terrain creation: Based on the elevation, the terrain is created. As the elevation value is the height from the sea level (sometimes too high or too low in the editor), it is possible to adjust the height offset parameter.
2. Object creation by semantic mask images
    - Building creation: The building objects are created by `Assets/MapData/building_mask.bin`. Currently, the building objects are textured by the pixel color in the map image, and a primitive cuboid with mesh and collider.
    - Road creation: The roads are based on the terrain. The masked area of the terrain as roads is annotated with navigation mesh of different costs than the terrain.
* When generating the virtual scene based on the geospatial data of the map, it is possible to adjust the map scale to the real-world map size via the `Scale Factor` parameter. If it is 2, the virtual-real ratio is 1:2.

#### MapManager
The generated terrain, buildings, and roads are managed as children of MapManager in the scene. As MapManager includes all the generated virtual objects, the USD exporter works with MapManager to export the generated scene in a USD format.

#### UsdExporter
When **R key** is typed, USD exporter exports the scene objects under MapManager as USD file. It can take some time to record the whole object details.
