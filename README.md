

# MMORPG 游戏项目

[![License](https://img.shields.io/badge/license-CC%20BY--NC--SA%204.0-blue.svg)](https://creativecommons.org/licenses/by-nc-sa/4.0/)

一款基于 Unity 引擎开发的大型多人在线角色扮演游戏（MMORPG）客户端项目。

## 项目简介

本项目是一个完整的 MMORPG 游戏客户端，旨在为玩家提供丰富的游戏体验。包含多种职业选择、完整的角色动画系统、NPC 交互系统、任务系统、背包系统、社交聊天系统以及多人副本与地图等核心功能。

## 技术栈

| 类别 | 技术 |
|------|------|
| 游戏引擎 | Unity |
| 动画系统 | FBX 模型动画 |
| UI 框架 | Unity UI (UGUI) |
| 动画插件 | DOTween |
| 序列化 | JsonDotNet (Newtonsoft.Json) |
| 网络协议 | Protobuf |
| 日志系统 | log4net |

## 项目结构

```
├── Art/
│   ├── Models/
│   │   ├── Characters/     # 角色模型
│   │   │   ├── Archer/     # 弓手
│   │   │   ├── Warrior/   # 战士
│   │   │   └── Wizard/    # 法师
│   │   ├── Monsters/      # 怪物模型
│   │   └── NPCs/          # NPC 模型
│   │
│   └── UI/
│       ├── Common/        # 通用 UI 组件
│       ├── Fonts/         # 字体文件
│       ├── Icons/         # 图标资源
│       └── Login/         # 登录界面
│
└── Src/Client/Assets/
    ├── Editor/            # Unity 编辑器扩展
    ├── Plugins/          # 第三方插件
    │   └── DOTween/       # 动画插件
    ├── References/        # 引用库
    └── Resources/         # 游戏资源
        ├── FX/            # 特效资源
        └── Levels/        # 游戏场景
```

## 职业系统

项目提供三种各具特色的职业供玩家选择：

### 战士 (Warrior)
- **定位**: 近战物理攻击职业
- **技能**: 普通攻击 A/B、技能 A/B/C、待机、死亡

### 弓手 (Archer)
- **定位**: 远程物理攻击职业
- **技能**: 普通攻击 A/B、跳跃、移动、死亡

### 法师 (Wizard)
- **定位**: 远程魔法攻击职业
- **技能**: 普通攻击 A/B、魔法技能、死亡

## NPC 系统

游戏中包含多种类型的 NPC：

- **DeliveryNPC**: 快递/传送 NPC
- **TaskNPC**: 任务发布 NPC
- **TreasureNPC**: 宝藏相关 NPC

## 游戏场景

| 场景名称 | 描述 |
|----------|------|
| MainCity | 主城场景 |
| Map01 | 游戏关卡地图 |
| Map02 | 游戏关卡地图 |
| Map03 | 游戏关卡地图 |
| CharSelect | 角色选择界面 |
| Loading | 加载界面 |

## UI 功能

项目实现了完整的 UI 系统，包括但不限于：

- 登录/注册界面
- 聊天系统（世界频道、队伍频道）
- 背包系统
- 好友系统
- 小地图
- 经验值/血量/魔法值显示

## 安装说明

1. 安装 Unity 编辑器（建议版本 2017+）
2. 克隆项目到本地
3. 使用 Unity 打开 `Src/Client` 目录
4. 等待资源导入完成
5. 构建并运行项目

## 开发说明

项目使用 `MapTools.cs` 提供了以下编辑器功能：

- **导出传送点** (`Map Tools/Export Teleporters`)
- **导出刷新点** (`Map Tools/Export Spawn Points`)
- **生成导航数据** (`Map Tools/Generate NavData`)

## 许可证

本项目仅供学习交流使用。

---

*本项目正在持续开发中，欢迎关注！*