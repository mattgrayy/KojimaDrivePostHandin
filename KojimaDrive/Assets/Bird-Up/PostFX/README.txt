-= POSTFX STACKS FOR KOJIMADRIVE =-
Post FX Stacks are simply an automated way of rendering screen effects in a certain order, and comes with a "generic" script that will work
for most postfx shaders. Throw a shader into a PostFX_Generic object, configure it, and throw it onto a PostFX Stack and it'll render in
the specified order. PostFX are rendered using the built-in camera system.

PostFX_Generic objects can have their values modified by altering their shaderProperty_t objects, which map exactly to the shader properties
of the given shader. These are classes, not structs, and thus references can be stored to them allowing for easy modification - for instance,
we will be adjusting the speed blur effect in this way based on the forward momentum of the player.

PostFX_Generic objects can be modified by the Generic Transitions.

Custom PostFX Stack members can be authored which inherit from PostFXObject.