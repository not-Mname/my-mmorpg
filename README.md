

# MMORPG 游戏客户端

这是一个基于 Unity 引擎开发的 MMORPG（大型多人在线角色扮演游戏）客户端项目。项目包含了完整的游戏角色系统、战斗系统、UI 界面、网络通信等核心模块。

## 项目结构

### 角色模型 (`Art/Models/Characters/`)

- **战士 (Warrior)** - 近战物理攻击职业
- **法师 (Wizard)** - 远程魔法攻击职业  
- **弓箭手 (Archer)** - 远程物理攻击职业

每个职业都包含完整的动画模型：
- 待机 (Idle)
- 移动 (Move_Forward/Back/Left/Right)
- 攻击 (AtkA/AtkB)
- 技能 (SkillA/SkillB/SkillC)
- 受伤 (Hurt)
- 死亡 (Death)

### 怪物模型 (`Art/Models/Monsters/`)

- M1001 - 骷髅怪
- M1002 M1003 M1004 M1005 - 各类怪物
- R001 - 坐骑模型

### NPC 模型 (`Art/Models/NPCs/`)

- **DeliveryNPC** - 快递NPC
- **TaskNPC** - 任务NPC
- **Treasure_poter** - 宝藏商人
- **Treasure_kingskull** - 宝箱怪
- **Treasure_redmob** - 红色怪物

### 地图场景 (`Src/Client/Assets/Levels/`)

- **MainCity** - 主城场景
- **Map01/Map02/Map03** - 野外地图
- **CharSelect** - 角色选择场景
- **Loading** - 加载场景

### 核心脚本 (`Src/Client/Assets/Scripts/`)

#### 战斗系统 (`Battle/`)

- `Skill.cs` - 技能类，管理技能的冷却、释放、命中判定
- `SkillManager.cs` - 技能管理器

#### 实体系统 (`Entities/`)

- `Entity.cs` - 实体基类
- `BattleUnit.cs` - 战斗单位，角色和怪物的基类
- `Character.cs` - 玩家角色
- `Monster.cs` - 怪物

#### 游戏对象控制 (`GameObject/`)

- `PlayerController.cs` - 玩家控制器，处理移动、跳跃、导航
- `EntityController.cs` - 实体动画控制器
- `MainPlayerCamera.cs` - 主玩家摄像机跟随
- `NPCController.cs` - NPC 交互控制器
- `GameObjectManager.cs` - 游戏物体管理器

#### 系统管理器 (`Managers/`)

- `CharacterManager.cs` - 角色管理
- `BagManager.cs` - 背包管理
- `EquipManager.cs` - 装备管理
- `ItemManager.cs` - 物品管理
- `QuestManager.cs` - 任务管理
- `ShopManager.cs` - 商店管理
- `ChatManager.cs` - 聊天管理
- `FriendManager.cs` - 好友管理
- `GuildManager.cs` - 公会管理
- `TeamManager.cs` - 队伍管理
- `SoundManager.cs` - 声音管理
- `UIManager.cs` - UI 管理

#### 网络通信 (`Network/`)

- `NetClient.cs` - 网络客户端，负责与服务器通信

#### 业务服务 (`Services/`)

- `UserService.cs` - 用户服务（登录、注册、创建角色）
- `MapService.cs` - 地图服务
- `BattleService.cs` - 战斗服务
- `ChatService.cs` - 聊天服务
- `ItemService.cs` - 物品服务
- `QuestService.cs` - 任务服务
- `GuildService.cs` - 公会服务
- `TeamService.cs` - 队伍服务
- `FriendService.cs` - 好友服务

#### UI 系统 (`UI/`)

- **UILogin.cs** - 登录界面
- **UICharacterSelect.cs** - 角色选择界面
- **UIMain.cs** - 主界面
- **UIBag/** - 背包界面
- **UIEquip/** - 装备界面
- **UIFriends/** - 好友界面
- **UIGuild/** - 公会界面
- **UIChat/** - 聊天界面

## 技术栈

- **引擎**: Unity
- **语言**: C#
- **动画系统**: Mecanim Animation
- **网络协议**: Protobuf
- **JSON 库**: JsonDotNet (Newtonsoft.Json)
- **动画插件**: DOTween

## 功能特性

### 角色系统
- 3种职业选择（战士、法师、弓箭手）
- 角色创建与选择
- 装备系统
- 背包系统

### 战斗系统
- 技能释放与冷却
- 技能命中判定
- 伤害计算

### 社交系统
- 聊天频道（世界、队伍、私聊）
- 好友系统
- 公会系统
- 组队系统

### 任务系统
- 任务对话
- 任务奖励

### 物品系统
- 商店购买
- 物品使用
- 装备穿戴

## 使用说明

1. 使用 Unity 打开 `Src/Client/` 目录
2. 确保已安装 Unity 2017+ 版本
3. 导入项目后配置服务器地址
4. 构建并运行客户端

## 目录说明

```
Src/Client/
├── Assets/
│   ├── Art/          # 美术资源（模型、贴图、UI）
│   ├── Editor/       # 编辑器工具脚本
│   ├── FX/           # 特效资源
│   ├── Levels/       # 场景文件
│   ├── Models/       # 3D模型
│   ├── Plugins/      # 第三方插件
│   ├── References/   # 引用库（Protocol.dll等）
│   ├── Resources/    # 运行时资源
│   └── Scripts/      # 源代码
└── ...
```

## 注意事项

- 本项目为客户端部分，需要配合服务器端使用
- 网络配置需要在 `NetClient` 中修改服务器地址
- 资源文件较大，部分文件未在代码中显示