# WatershedSimulator

Guide:
	Install Unity 5 Personal from http://unity3d.com/get-unity

	Open Unity
	Open /Assets/BaseScene.unity either by double clicking in Explorer / Fider or loading it from the "Open Scene" menu in Unity
	Press the "Run" triangle (play button looking thingy) at the top center of the Unity scene window
	Press the "Pause" button next to the "Run" triangle to inspect the scene. Mouse wheel will move the camera in and out, and holding "Alt" or "Command" will cause a click and drag to pan the camera

	The current functionality procedurally creates a mesh from a *.TIN file (A common GIS file which is composed of triangles creating a surface - see http://www.xmswiki.com/wiki/TIN_Files for a breakdown of the file type). It applies a rigidbody, a texture/material, a non-kinematic physics model to the mesh and then drops a (currently, non procedural) ball on the mesh. 

Our final project would include the following:
	A GUI that allows the user to select certain parameters, as well as input a raster / TIN file to be created
	Create a TIN from an input raster (This could be omitted, it is the hardest part of the project)
	Create a terrain mesh from a TIN and load it to the scene
	Procedurally spawn spheres and drop them on the mesh
	A camera and lighting model that has nice shading

Reach goals:
	Allow the user to dynamically change where the spheres spawn (i.e. the spheres spawn from a 2d grid that is set above the terrain - let them define the center, length, and width of the grid, as well as the rotation of it)
	Allow the user camera control so they can navigate the scene
	Allow the user to set points where they can measure runoff, to get an idea of the percent of dropped water that goes through a certain section of the pourshed

Cool stuff this project includes:
	We can implement a simple lighting model and any type of static or dynamic camera we want
	We can apply textures to the environment based on height 
	It looks pretty
	It is a surprisingly useful tool for Geographers
	If we create a TIN from a raster, that would be a super cool mathy thing to do
	Determining how many spheres we can render at once is a ridiculously interesting computational problem

