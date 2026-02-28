<img width="2057" height="1222" alt="image" src="https://github.com/user-attachments/assets/fa5add64-a760-4c1e-a7e7-690dd481946e" />


# EscapeTheCatacombs
A top-down survival horror game developed in Unity within a 3-week game jam.  
Built independently while learning Unity and C# from scratch.

# Overview

Escape the Catacombs is a fast-paced survival experience where the player navigates hostile underground environments while avoiding and fighting a single, dynamically scaling malevolent entity.

# Feature Overview

- Single handcrafted level
- Player controller with responsive movement and collision handling
- Enemy AI implemented using a finite state machine (Patrol, Investigate, Chase)
- Scaling enemy AI as game progresses that responds to player actions
- Randomized object spawning for dynamic experience
- Staircases that allow fast yet risky travel across the map
- Scarce "saferooms" that prevent the enemy from entering

# Performance and Optimizations

- Caching of frequently accessed components such as enemy and player sprite and audio components
- Removal of unnecessary calculations from update loop
- Minimum distance threshold before updating enemy pathfinding to prevent constant checks
- Modular script architecture
- Stable 120 FPS in Unity Editor

# Enemy AI Technical Details

- Enemy difficulty increases with each key collected
- NavMesh Agent for enemy pathfinding
- Collider + Raycast (to ensure enemy can actually "see" the player) for player detection

## Difficulty Scaling Levels

Keys Collected:
- 1,2: speed increase
- 3: larger FOV for player detection
- 4: speed and acceleration increase
- 5: sound detection enabled (sprinting, breaking objects)
- 6: longer chase timer
- 7: speed, acceleration, and angular acceleration increase
- 8: enemy stair use enabled
- 9: removal of lighting
- 10: removal of saferooms

# Controls

- WASD - Movement  
- Shift - Sprint
- Hold E - Break object
- Q - Use PowerUp
- R - Use Staircase



Playable build available at:  
https://hayden-42.itch.io/escape-the-catacombs

Developed as part of Switzerland Game Jam 2025 (Webster Geneva Campus).
