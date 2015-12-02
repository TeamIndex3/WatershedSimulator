# WatershedSimulator

##**Installation Guide:**##

1. Install [Unity 5 Personal](http://unity3d.com/get-unity)

2. Open Unity

3. Open /Assets/BaseScene.unity either by double clicking in Explorer / Fider or loading it from the "Open Scene" menu in Unity

4. Press the "Run" play button (located top-center of the Unity scene window)

5. Press the "Pause" button next to the "Run" triangle to inspect the scene. Mouse wheel will move the camera in and out, and holding "Alt" or "Command" will cause a click and drag to pan the camera


The current functionality procedurally creates a mesh from a *.TIN file (A common GIS file which is composed of triangles creating a surface - see http://www.xmswiki.com/wiki/TIN_Files for a breakdown of the file type). It applies a rigidbody, a texture/material, a non-kinematic physics model to the mesh and then drops a (currently, non procedural) ball on the mesh. 


##**Program**##

A GUI that allows the user to select certain parameters, as well as input a raster / TIN file to be created and move the camera. 

Here are two examples of FPS style camera controllers: 
http://wiki.unity3d.com/index.php/FPSWalkerEnhanced 
http://docs.unity3d.com/ScriptReference/CharacterController.Move.html
<p>Unity GUI class reference: 
http://docs.unity3d.com/Manual/UISystem.html

Create a TIN from an input raster using Delaunay Triangulation(This could be omitted, it is the hardest part of the project)
https://en.wikipedia.org/wiki/Delaunay_triangulation
https://msdn.microsoft.com/en-us/library/system.windows.media.matrix.determinant(v=vs.110).aspx

Create a terrain mesh from a TIN and load it to the scene

Procedurally spawn spheres and drop them on the mesh

A camera and lighting model that has nice shading


###*Program Extensions (Reach Goals)*###

Allow the user to dynamically change where the spheres spawn (i.e. the spheres spawn from a 2d grid that is set above the terrain - let them define the center, length, and width of the grid, as well as the rotation of it)

Allow the user camera control so they can navigate the scene

Allow the user to set points where they can measure runoff, to get an idea of the percent of dropped water that goes through a certain section of the pourshed

**Cool stuff this project includes:**

We can implement a simple lighting model and any type of static or dynamic camera we want

We can apply textures to the environment based on height 

It looks pretty

It is a surprisingly useful tool for Geographers

If we create a TIN from a raster, that would be a super cool mathy thing to do - see http://www.asprs.org/a/publications/pers/2005journal/august/2005_aug_917-926.pdf

Determining how many spheres we can render at once is a ridiculously interesting computational problem

