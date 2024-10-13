# Xonix - Unity Game Project

## Table of Contents
- [Introduction](##introduction)
- [Demo](##demo)
- [Features](#features)
- [Gameplay](#gameplay)
- [How to Run](#how-to-run)
- [Project Structure](#project-structure)
- [Contributing](#contributing)
- [License](#license)

## Introduction

**Xonix** is a modern twist on the classic arcade game where players claim territory on a grid while avoiding enemies. In this version, you play as a student racing to claim areas while running away from 'F' (failed exams). As you claim new areas, the background changes to fun locations, like beaches or parties, representing the places a student would escape to.

The game is built in Unity and offers a unique challenge, combining arcade-style gameplay with the theme of escaping academic stress.

## Demo

[Click here](https://hadaromer.itch.io/xonix)

## Features

- **Dynamic Gameplay**: Capture areas while avoiding enemies.
- **Thematic Twist**: The player’s character is a student running from failed exams, adding a humorous and relatable storyline.
- **Changing Backgrounds**: Progression changes the game's background to different student-themed locations (e.g., beach, party).
- **Customizable Levels**: Easily extend or modify levels by altering the 2D grid layout.
- **Smooth Player and Enemy Movements**: Optimized movement across a 2D array grid.
- **Collision Detection**: Implemented for enemies and the player, with potential for additional challenges.

## Gameplay

- Control the player to paint areas on the map, claiming them as your territory.
- Avoid being caught by the enemies (F's), which chase you down as you progress.
- Each level introduces new challenges and enemy types, making it harder to paint large areas without getting caught.
- Successfully claiming enough area will unlock the next level, where the background changes to more exciting places, symbolizing a student’s fun distractions.

## How to Run

### Prerequisites

- **Unity**: Version 2021.3 or later.
- **Git**: To clone the repository.

### Steps

1. Clone the repository:
   ```bash
   git clone https://github.com/hadaromer/Xonix.git
   ```
2. Open the project in Unity.
3. Press the Play button in Unity to run the game in the editor.

## Project Structure

```bash
Xonix/
│
├── Assets/               # Game assets including scripts, textures, and prefabs
│   ├── Scripts/          # All C# scripts for gameplay mechanics
│   ├── Sprites/          # Sprites for player, enemies, and environment
│   └── Scenes/           # Game scenes
├── README.md             # Project readme
└── .gitignore            # Files and directories to ignore in Git
```

### Key Scripts

- GameManager.cs: Handles game logic, such as level progression and player state management.
- PlayerController.cs: Manages player movement and interactions.
- EnemyController.cs: Controls enemy behavior and movement across the grid.

## Contributing
Contributions are welcome! Please submit pull requests with detailed descriptions of any changes or improvements. Feel free to open issues for bug reports or feature requests.

# License
This project is licensed under the MIT License - see the LICENSE file for details.
