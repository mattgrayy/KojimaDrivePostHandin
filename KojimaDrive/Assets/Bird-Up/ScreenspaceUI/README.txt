-= SCREENSPACE UI FOR KOJIMADRIVE =-
KJD Screenspace UI is a replacement for Unity's Canvas that scales our UI elements and shifts their position based on the game's aspect ratio.
Everything is created using an internal resolution, which adjusts the scale of the ObjectContainer to allow for a true 1:1 position-pixel ratio
to the desired resolution. By doing this instead of using Canvas and UI cameras, we decouple the screenspace position and the actual rendering
resolution (which can be whatever we want).

Kojima Drive's internal UI resolution is 1920x1080, with 0,0 being the bottom left of the screen. To create a quad that fills the screen,
create the quad as a child of ObjectContainer and set its scale to 1920x1080, with a position of 960x540 (screen center). In order for objects
to be properly scaled, they must have a "Screenspace UI Object" script. This will allow you to configure how you wish it to scale; by scale,
just by position, or whether Typogenic Text values should be adjusted. This will be honoured when changing the aspect ratio of the game.