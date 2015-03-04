## Altspace
A Unity project for AltspaceVR interview process.

### Goals
- Develop a technique for manipulating objects that might be useful in VR.
- Demonstrate coding style and ability and understanding of object transformations in a 3D space.

### Result
I perhaps took "object manipulation" too literally.  I wrote some scripts in Unity that allow a user to "pick up" physics objects and inspect them by rotating them.  Objects that can be picked up and manipulated are highlighted when they get within "reach" of the user.

### Instructions
Download the project from GitHub.  Extract the three zip files.  They contain all the textures and baked lighting for the scene.  Bring it all into a Unity4 project.  Open up the "test" scene, and run it.  There are objects on the ground that you can pick up and manipulate.  To pick up an object, walk up to it and look at it.  Objects that you can pick up are highlighted, and the strength of the highlight corresponds to which object you will pick up from a group of objects.  Pressing the "E" key once picks up the object for "inspection", putting it right in front of the camera.  Pressing the "E" key again will drop the object down slightly, allowing you to carry it without obstructing your vision.  Pressing "E" on last time will drop the object.  If you hold "Shift" while holding an object, mouse movements will translate to movements of the object, allowing you to look at it from different angles.

### What I Made
I wrote all the code/logic for object manipulation in the ManipulateObject and ManipulateController scripts.  I also modified the MouseLook script, which was originally provided by a Unity package, in order to add the object rotation code when "Shift" is pressed.  Finally, I had to make a custom shader for highlighting objects that can be picked up.  However, I didn't come up with the logic for making "outlines" around objects on my own, but rather copied it from a shader I found on a Unity forum somewhere.

### What I Didn't Make
Most all of the character movement code is from the Unity "Character Controller" package.  All of the assets are from some Unity tutorials and/or free assets from the Unity Asset Store.

### Todo
- When an object is "picked up", the distance that it is "held" from the camera is set as a variable on the object.  The same is true for how far to drop it down when the user hits "E" again.  This isn't a terrible solution, as it allows for customization of these values depending on if an object is large or small.  However, one could see a different solution that actually calculates these distances for you based on the size of the object.  Similarly, smoothing the object motion can be adjusted for each object, so that small objects have "snappy" motion and large objects have more lag.
- When an object is "dropped", it has no velocity, and simply falls down due to gravity.  It shouldn't be hard to actually keep track of what the velocity should be for an object when a user hits "E" to drop it, and apply that velocity.  This would give the illusion that momentum is preserved, and allow for behaviors such as throwing objects.
- Modify the Outline shader to have fallback SubShaders for other architectures (such as older consoles).
- Clean up the project organization.  It got a little messy from the different packages imported in.
- It might be cool to blur the background when an object is being "inspected" (right in front of the camera).  However, I don't think I can do anything like that without Unity Pro.

