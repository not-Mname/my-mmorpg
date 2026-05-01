# MMORPG Game Project

A client-side MMORPG (Massively Multiplayer Online Role-Playing Game) project developed using the Unity engine.

## Project Overview

This project is a complete MMORPG client featuring rich game content:
- Multiple character classes (Warrior, Mage, Archer)
- Comprehensive character animation system
- NPC interaction system
- Quest system
- Inventory system
- Social chat system
- Multiplayer dungeons and maps

## Technology Stack

- **Engine**: Unity
- **Animation System**: FBX model animations
- **UI Framework**: Unity UI (UGUI)
- **Animation Plugin**: DOTween
- **Serialization**: JsonDotNet (Newtonsoft.Json)
- **Network Protocol**: Protobuf
- **Logging**: log4net

## Project Structure

```
Art/
├── Models/
│   ├── Characters/     # Character models
│   │   ├── Archer/     # Archer
│   │   ├── Warrior/    # Warrior
│   │   └── Wizard/     # Wizard
│   ├── Monsters/       # Monster models
│   └── NPCs/           # NPC models
│
UI/
└── UI/                 # UI resources
    ├── Common/         # Common UI components
    ├── Fonts/          # Font files
    ├── Icons/          # Icon resources
    └── Login/          # Login interface

Src/Client/Assets/
├── Editor/             # Unity editor extensions
├── Plugins/            # Third-party plugins
│   └── DOTween/        # Animation plugin
├── References/         # Reference libraries
└── Resources/          # Game resources
    ├── FX/             # Effect resources
    └── Levels/         # Game scenes
```

## Character Classes

### Warrior
- Melee physical damage class
- Skills: Basic Attack A/B, Skill A/B/C, Idle, Death

### Archer
- Ranged physical damage class
- Skills: Basic Attack A/B, Jump, Move, Death

### Wizard
- Ranged magical damage class
- Skills: Basic Attack A/B, Magic Skill, Death

## NPC System

- **DeliveryNPC**: Delivery/Teleport NPC
- **TaskNPC**: Quest-giving NPC
- **TreasureNPC**: Treasure-related NPC

## Game Scenes

- **MainCity**: Main city scene
- **Map01/Map02/Map03**: Game level maps
- **CharSelect**: Character selection interface
- **Loading**: Loading screen

## UI Features

- Login/Registration interface
- Chat system (World channel, Party channel)
- Inventory system
- Friend system
- Mini-map
- HP/MP/XP display

## Installation Instructions

1. Install Unity Editor (recommended version: 2017+)
2. Clone the project to your local machine
3. Open the `Src/Client` directory in Unity
4. Wait for asset import to complete
5. Build and run the project

## Development Notes

The project uses `MapTools.cs` to provide the following editor tools:
- Export Teleporters
- Export Spawn Points
- Generate NavData

## License

This project is intended solely for learning and communication purposes.

---

*This project is under active development...*