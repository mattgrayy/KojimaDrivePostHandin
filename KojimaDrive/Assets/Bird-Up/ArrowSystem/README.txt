1. Creating the arrow system
   a. Prefabs included
   b. Hooking up the camera
   c. Hooking up the checkpoint managers
   d. Creating checkpoints

1. Creating the arrow system

The arrow system has 3 parts to it - the Checkpoint Manager/Checkpoints, the
UI camera and UI arrow, and the rotation object on the car that the car uses to
point the UI arrow to the goal. Each of these systems will be provided in prefabs
but linking them to your specific car and camera systems should not be too difficult.

1a. Prefabs included

 - Checkpoint Manager: base manager object for holding checkpoint data. Works as a singleton
 - RotatePoint: to be placed on the car as a child object. Used by NonWorldUICam to tell the sky
 arrow where to look, and holds arrows specific checkpoints
 - NonWorldUICam: Camera positioned at y:5000. Holds an arrow object that rotates dependant
 on the car it's linked to. A gameobject/script that contains the rotation object and the car's
 number/camera number should be sent to this car to set what to rotate from/the masking layer
 to render (WorldUIP1 to P4)
 
1b. Hooking up the camera

The camera objects should be instantiated when the splitscreens are created, and should take the same
Rect properties as the car camera they are being attached to. The script Non World UI Cam will
require the car's ArrowViewpoint/Rotation point to be sent to it to Base Arrow, as well as the number
of the camera in relation to what player it is for (1-4).

1c. Hooking up the checkpoint managers

There are two types of managers: the overall manager that holds all checkpoints and the individual
arrowCheckpoint managers on each ArrowViewpoint/Rotation point. These are both included on prefabs - the
first, the base manager, can be placed anywhere in the scene. The arrow managers need to be parented to
the car objects. The base manager currently has options to change how checkpoints are listed (by distance
or preset list) and also to set the distance the car has to be to trigger OnCheckpoint() within each
arrowCheckpoint script.

Checkpoints are created by dragging and dropping the Checkpoint script on any gameobject you want to
be a checkpoint. The checkpoint list on the Checkpoint Manager will automatically propogate itself with
any object with this script. Destroying the checkpoint script on the gameobject or destroying the object
itself will remove the checkpoint from the list. You can also call RemoveCheckpoint() on the 
Checkpoint Manager to remove specific checkpoints.

OnCheckpoint() will be called every frame the user is within a specific range of the checkpoint.
