# MyMMORPG - 3D卡通风格MMORPG游戏引擎

**MyMMORPG** 是一款基于Unity和.NET Framework开发的3D卡通风格大型多人在线角色扮演游戏(MMORPG)引擎。项目采用前后端分离架构，支持法师、战士、弓箭手三种职业，包含主城、野外、副本、竞技场等丰富场景，以角色养成、PVE副本配合、PVP对战为核心玩法。

## 🏗️ 技术架构

### 前端 (Unity客户端)
- **游戏引擎**: Unity 2018.4
- **编程语言**: C#
- **网络通信**: 自定义TCP协议 + Protobuf序列化
- **UI框架**: Unity UGUI + TextMeshPro
- **依赖管理**: Unity Package Manager
- **关键组件**:
  - AI Navigation (寻路系统)
  - AssetBundle (资源管理)
  - Physics & Particles (物理和粒子效果)
  - Video & Audio (多媒体支持)

### 后端 (游戏服务器)
- **运行环境**: .NET Framework 3.5
- **编程语言**: C#
- **数据库**: SQL Server 2017
- **ORM框架**: Entity Framework 6.2.0
- **网络库**: 自定义异步TCP服务器
- **日志系统**: Log4net
- **序列化**: Protobuf-net
- **JSON处理**: Newtonsoft.Json

### 网络通信协议
- **协议格式**: Google Protocol Buffers (Protobuf)
- **传输层**: TCP Socket
- **消息分发**: 基于消息ID的路由分发机制
- **心跳机制**: 客户端定期发送Ping包维持连接
- **包结构**: 4字节头部(长度) + Protobuf序列化数据

## 📁 项目结构

## 🧩 核心系统设计

### 分层架构
项目采用清晰的分层架构设计：

1. **Service层**: 处理网络通信，大部分服务在客户端和服务端都有对应实现

2. **Manager层**: 
   - **客户端**: 管理游戏实体、控制Unity系统 
   - **服务端**: 实现核心业务逻辑和实体管理

3. **Entities层**: 存储游戏中所有实体的数据结构

4. **Model层**: 存储被管理层管理的对象数据

5. **GameObject层** (仅客户端): 定义实体控制器和Unity特定工具

### 数据驱动设计
- 所有游戏数据通过Excel配置，转换为JSON格式
- 使用统一的数据定义类 (`CharacterDefine.cs`, `ItemDefine.cs`, `SkillDefine.cs`等)
- 服务端通过 [DataManager] 加载所有配置数据

### 网络同步机制
- 客户端通过 [NetClient]连接服务器
- 服务端使用多线程处理网络请求
- 消息通过 [MessageDispatch] 和 [MessageDistributer] 进行路由分发
- 支持断线重连和连接状态管理

## 🚀 快速开始

### 环境要求
- **客户端**: Unity 2018.4 或更高版本
- **服务端**: .NET Framework 3.5, SQL Server 2017
- **开发工具**: Visual Studio 2019+

### 构建步骤
1. **初始化数据库**: 配置SQL Server并导入数据库结构
2. **生成协议**: 运行 `Tools/genproto.cmd` 生成Protobuf协议文件
3. **启动服务器**: 编译并运行 `Src/Server/GameServer/GameServer.sln`
4. **启动客户端**: 在Unity中打开 `Src/Client` 项目并运行

### 配置文件
- **服务器配置**: `GameServer/Properties/Settings.settings`
  - [ServerIP]: 服务器IP地址 (默认: 127.0.0.1)
  - [ServerPort]: 服务器端口 (默认: 8000)

## 🎮 功能特性

### 核心玩法
- **角色系统**: 三种职业(法师、战士、弓箭手)，完整的属性和装备系统
- **战斗系统**: 实时战斗，技能释放，Buff/Debuff机制
- **社交系统**: 好友、公会、组队、聊天
- **任务系统**: 主线任务、支线任务、日常任务
- **经济系统**: 商店、背包、物品交易
- **场景系统**: 主城、野外、副本、竞技场

### 技术特性
- **高性能网络**: 异步非阻塞I/O，支持大量并发连接
- **数据持久化**: Entity Framework + SQL Server，确保数据一致性
- **跨平台协议**: Protobuf保证前后端数据格式统一
- **模块化设计**: 高内聚低耦合，易于扩展和维护
- **资源管理**: AssetBundle动态加载，减少内存占用

## 📊 性能指标
- **网络延迟**: < 100ms (局域网环境)
- **并发用户**: 支持数百人同时在线
- **内存占用**: 客户端约200-500MB，服务端约100-300MB
- **CPU使用**: 平均占用率 < 30% (中等配置服务器)

## 📄 注意
 - 本项目仅供学习和研究使用，目前项目资源比较混乱，过段时间我会优化资源结构并加入打包和热更新功能。
 - 本项目业务逻辑基本完成，战斗系统正在研究，预计4月完成。

---