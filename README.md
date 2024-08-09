# Space Time Gravity
An Unity game based on space, time and gravity manipulation mechanics. This is a puzzle game in
a 3D world, where the main goal is to complete levels. The game features several singleplayer and
multiplayer levels with platforming elements.

## Core mechanics
### Space manipulation
The main character can change its size. Player can loop through the 3 sizes: medium, big and small.

<details>
  <summary>Size control GIF</summary>
  
![Size change](media/size_control.gif)
</details>

### Time manipulation
When in singleplayer, player can trigger time pause (freeze) and time rewind. Everything except player is affected. Positions, rotations, velocities of all physics objects, except the player character.

<details>
  <summary>Time manipulation GIF</summary>
  
![Time control](media/time_control.gif)
</details>

### Gravity manipulation
The player character can walk on special pads, which will change the direction of gravity and
the visualisation perspective. This enables walking on walls and ceiling of the player character(s).

<details>
  <summary>Gravity manipulation GIF</summary>
  
![Gravity control](media/gravity.gif)
</details>

## Other mechanics
### Movement
Player character is able to run, walk, swim in all directions, and jump upwards as well.

### Multiplayer
Multiplayer is either online (client-server direct connection) or split-screen. Split screen
requires at least two gamepads.


<details>
  <summary>Images</summary>
  
![Split-screen menu](media/splitscreen_menu.png)
![Host screen](media/waiting_partner.png)
</details>

#### Picture-in-Picture
It is supposed that in multiplayer, players can see each other's screens. While it is trivial for
split-screen mode, it could be a problem in online multiplayer. Thus, by pressing `TAB` in online
multiplayer game session, the local player will see the partner's screen in Picture-in-Picture mode.

<details>
  <summary>Image</summary>
  
![Picture-in-Picture](media/picture-in-picture.png)
</details>

### Gamepad support
The game supports keyboard+mouse or gamepad controls. Specifically, the game was tested with Xbox 360 for Windows compatible gamepads and Xbox Series X|S gamepads.



<details>
  <summary>Images</summary>
  
![Keyboard controls](media/keyboard_controls.png)
![Gamepad controls](media/gamepad_controls.png)
</details>

## Levels
The game features two Singleplayer and one Multiplayer level. All levels were built for demo purposes.

<details>
  <summary>Images</summary>
  
![Level 1](media/level1.png)
![Level 2](media/level2.png)
![Multiplayer Level](media/level1mp.png)
</details>
