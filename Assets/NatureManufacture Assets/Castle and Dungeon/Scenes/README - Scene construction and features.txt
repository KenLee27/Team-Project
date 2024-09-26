BEFORE YOU START:

About pack:
1. Scenes are pretty big only bottleneck is the CPU (amount of objects). We test them at multiple common GPUs and FPS depends on the CPU.
2. Decals are only supported at HD and URP (URP needs to add render feature in Universal Renderer file)
3. We will expand the pack with more prefabs, buildings, etc to show the full potential of the pack. Be patient. 
4. We will reduce the used lightmap size at the demo scene and adjust the lighting interiors, step by step.
5. We also will provide some tutorials on how to play with the pack and build a nice environment.
6. We will also build a dungeon map as an example. 
Unity has a 6 GB limit of files so it will be inside (if we have space) or as a small addon with prefabs and map. 
7. We will provide fixes in the upcoming days. Pack has 2500+ prefabs and we need a bit of time to polish everything. 
That is why packs on initial -50% sale.
8. Use deferred render at the demo scene. It's because the scene has many reflection probes.
Forward may have a problem with that. Terrain will not render properly.

Unity Built-in
1. Use linear render.
2. To have emission at unity Built-In you need to have a post-processing stack inside the project.
You can find it in the unity files in the package manager. The emission to appear needs a bloom effect at the camera. That's how the engine works.
3. Our demo scene has multiple light and reflection probes. Please use deferred render on it.
Forward does not handle such an amount of reflection probes and lights at one screen. 
For your scenes you can use what you need, it's just for our demo scene to hold nice fps.
4. Rather reduce LOD bias to 1-1.5. It depends on the goal device.

HD and URP
1.Please import SRP packs only if your project is using HD or URP.
2.You will find it in the HD and URP Support packs folder, follow their readme.
3.If you download the pack via the actual engine version unity asset store server should provide you proper file out of the box. The same HD and URP files will not work on other engine and RP versions. Even if they do not throw errors out of the box they could be rendered wrong.
4.Import the proper RP compatibility pack to the proper engine version more details at their readme.

LIGHTMAPS IMPORTANT:
- We bake lightmaps at 9 pixels per unity unit. It's really low, it's because of the max pack size
- To improve light quality at the demo scene increase this to 12 and build light via GPU.
     1. If you haven't baked light a lot before I suggest:
Edit --> Preferences --> GI Cashe --> Improve it to 50-100GB? This will avoid problems with memory during bake.
     2. Bake light at the scene at resolution 4-5 and then at 12. Just to build calculation and cache library.

About scenes and solutions:
1. Most materials at the scene use snow cover. You can replace them with non-snow variants or simply open them in the project and change the snow amount to 0 
2. There are objects at the scene called snow occluders. They are useful to remove snow from interiors. 
They simply paint vertex color so our shaders remove snow. Play with them at our demo scene called "Snow Occluding Tutorial Scene"
They cull snow so you don't lose batching because of another material - without snow. You can use the same material in interiors (snow variant) and just paint them via vertex color. You will get a nice snow blend out of the box on window borders, doors, etc.
Remember that vertex color creates a copy of the 3d mesh, so it starts to eat disc storage as a scene file but... they are compressable to low values during build so this is not so bad solution.
3. A few objects at a scene like stairs and chains were placed by our spline tools from L.V.E 2023 with fence generator profiles. 
If you have this system you can apply the profile to the fence generator and place stairs and chains via spline. Super fast and flexible solution.
Profiles you will find in "Fence Profiles" it's a unity package.
4. We place rivers, lakes, and many pavements via L.V.E 2023 lakes and rivers. It's a super useful tool to build such shapes and vertex color them.
You can find the whole system here:
 https://assetstore.unity.com/packages/tools/terrain/l-v-e-2023-lava-volcano-environment-2023-253816?aid=1011l7vcu
We give you nice trials from lava rivers, particles, and rocks just to make the scene a complete image.
5. You can easily change the scene into grassland by replacing cover textures.
Turn off snow cover (reduce snow amount to 0) at architecture materials and replace snow cover in rocks via grass texture.
You can also create grass texture and other textures via Unity Muse, look at our video tutorial related to the pack, we change the whole scene into sand and clay in a very short time.

About materials:
1. There are only a few materials used at the scene, which is why there are not many batches and used textures
2. Each material has:
- Base texture: It uses UV0 and its tilled texture like clay, stone, marble, rock, etc no visible sharp shapes.
- Cover textures: The texture used for the top cover is mostly used as planar)
- Shape textures: They hold information about the shape, ambient of the object, leaks mask, and curvature
- Leaks texture: It holds information about leaks facture.
- In shape materials AO from textures is used for emission to avoid adding another texture to the project.
- vertex colors B and G are responsible for the Cover for example snow and sand.
- Vertex color R is responsible for emission or wetness.

I will make a nice tutorial about this shader usage. 
The idea behind is to have multiple materials and don't waste a huge amount of memory into texture and hold them in super high resolution.
Base texture: is shared between many materials. 
You keep 1 nice texture for example stone and share them between materials so they fit together as they are built from the same material.
Shape texture: (normal and mask map) is per material as it's responsible for individual object shape.
Materials could be heated or become wet at runtime if you have L.V.E 2023 in the project. You can replace lava via water in a very short time with R.A.M 2019.
