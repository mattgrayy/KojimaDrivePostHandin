-= TRANSITIONS FOR KOJIMADRIVE =-
Transitions are a complicated way of doing screen transitions. Generally, a transition will be a Transition_Generic object, which will be set
up to interact with a PostFXStack and PostFX_Generic objects. Not only can all material properties of a PostFX_Generic be altered, but 
they can be lerped or even applied across an animation curve using the tickboxes on the left side of the screen.

PostFXStacks are accessed by their 'global static name', as are the PostFX_Generic objects themselves.

Transitions are intended for screen transitions, but could be used for a variety of things; for instance, altering PostFX with a could
produce better results if performed via a hand-made transition instead of a simple lerp.

Transitions operate on 'channels' allowing multiple to play at once; however, if one attempts to play on an already filled channel, the initial
transition will be overridden and its interrupt event fired.

Currently, transitions use the event system, but additional functionality will be added to this.


TRANSITION EDITOR WINDOW:
There's a basic editor window available under the Kojima Drive menu which shows currently running transitions.