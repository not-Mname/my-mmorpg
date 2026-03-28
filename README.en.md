# MMORPG Game Client

This is an MMORPG (Massively Multiplayer Online Role-Playing Game) client project developed using the Unity engine. The project includes core modules such as a complete character system, combat system, UI interface, and network communication.

## Project Structure

### Character Models (`Art/Models/Characters/`)

- **Warrior** - Melee physical damage class
- **Wizard** - Ranged magical damage class
- **Archer** - Ranged physical damage class

Each class includes a full set of animation models:
- Idle
- Move_Forward/Back/Left/Right
- AtkA/AtkB (Attack)
- SkillA/SkillB/SkillC (Skills)
- Hurt
- Death

### Monster Models (`Art/Models/Monsters/`)

- M1001 - Skeleton Monster
- M1002, M1003, M1004, M1005 - Various monsters
- R001 - Mount model

### NPC Models (`Art/Models/NPCs/`)

- **DeliveryNPC** - Delivery NPC
- **TaskNPC** - Quest NPC
- **Treasure_poter** - Treasure Merchant
- **Treasure_kingskull** - Treasure Chest Monster
- **Treasure_redmob** - Red Monster

### Map Scenes (`Src/Client/Assets/Levels/`)

- **MainCity** - Main city scene
- **Map01/Map02/Map03** - Outdoor maps
- **CharSelect** - Character selection scene
- **Loading** - Loading screen

### Core Scripts (`Src/Client/Assets/Scripts/`)

#### Combat System (`Battle/`)

- `Skill.cs` - Skill class managing cooldown, casting, and hit detection
- `SkillManager.cs` - Skill manager

#### Entity System (`Entities/`)

- `Entity.cs` - Base entity class
- `BattleUnit.cs` - Battle unit base class (for characters and monsters)
- `Character.cs` - Player character
- `Monster.cs` - Monster

#### GameObject Control (`GameObject/`)

- `PlayerController.cs` - Player controller handling movement, jumping, and navigation
- `EntityController.cs` - Entity animation controller
- `MainPlayerCamera.cs` - Main player camera follower
- `NPCController.cs` - NPC interaction controller
- `GameObjectManager.cs` - Game object manager

#### System Managers (`Managers/`)

- `CharacterManager.cs` - Character management
- `BagManager.cs` - Inventory management
- `EquipManager.cs` - Equipment management
- `ItemManager.cs` - Item management
- `QuestManager.cs` - Quest management
- `ShopManager.cs` - Shop management
- `ChatManager.cs` - Chat management
- `FriendManager.cs` - Friend management
- `GuildManager.cs` - Guild management
- `TeamManager.cs` - Team management
- `SoundManager.cs` - Sound management
- `UIManager.cs` - UI management

#### Network Communication (`Network/`)

- `NetClient.cs` - Network client responsible for server communication

#### Business Services (`Services/`)

- `UserService.cs` - User service (login, registration, character creation)
- `MapService.cs` - Map service
- `BattleService.cs` - Combat service
- `ChatService.cs` - Chat service
- `ItemService.cs` - Item service
- `QuestService.cs` - Quest service
- `GuildService.cs` - Guild service
- `TeamService.cs` - Team service
- `FriendService.cs` - Friend service

#### UI System (`UI/`)

- **UILogin.cs** - Login interface
- **UICharacterSelect.cs** - Character selection interface
- **UIMain.cs** - Main interface
- **UIBag/** - Inventory interface
- **UIEquip/** - Equipment interface
- **UIFriends/** - Friends interface
- **UIGuild/** - Guild interface
- **UIChat/** - Chat interface

## Technology Stack

- **Engine**: Unity
- **Language**: C#
- **Animation System**: Mecanim Animation
- **Network Protocol**: Protobuf
- **JSON Library**: JsonDotNet (Newtonsoft.Json)
- **Animation Plugin**: DOTween

## Features

### Character System
- 3 class options (Warrior, Wizard, Archer)
- Character creation and selection
- Equipment system
- Inventory system

### Combat System
- Skill casting and cooldown management
- Skill hit detection
- Damage calculation

### Social System
- Chat channels (World, Team, Private)
- Friend system
- Guild system
- Team system

### Quest System
- Quest dialogue
- Quest rewards

### Item System
- Store purchases
- Item usage
- Equipment equipping

## Usage Instructions

1. Open the `Src/Client/` directory in Unity
2. Ensure Unity 2017+ is installed
3. After importing the project, configure the server address
4. Build and run the client

## Directory Structure

```
Src/Client/
├── Assets/
│   ├── Art/          # Art assets (models, textures, UI)
│   ├── Editor/       # Editor utility scripts
│   ├── FX/           # Effect resources
│   ├── Levels/       # Scene files
│   ├── Models/       # 3D models
│   ├── Plugins/      # Third-party plugins
│   ├── References/   # Reference libraries (Protocol.dll, etc.)
│   ├── Resources/    # Runtime assets
│   └── Scripts/      # Source code
└── ...
```

## Notes

- This project contains only the client side and requires a corresponding server to function
- Server address configuration must be updated in `NetClient`
- Resource files are large; some files are not explicitly referenced in the code