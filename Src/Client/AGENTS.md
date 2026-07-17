# 客户端 Agent（Unity C#）

## Responsibilities
- Unity 客户端逻辑：UI、战斗、场景、特效、音效
- HybridCLR 热更脚本 `Assets/Scripts/HotUpdate/`
- 协议对接（Proto → C# 的序列化/反序列化）
- AssetBundle 资源管理与加载

## Scope
- `Assets/Scripts/Core/` — 固化代码（非热更）
- `Assets/Scripts/HotUpdate/` — 热更代码（HybridCLR）
- 目录划分：Battle / UI / Services / Managers / Models / Entities / Scene / Effect / FX / Const / Dispose / GameObject / InterFace

## Constraints
- 跨模块通信用 `EventManager.Publish<T>()` / `Subscribe<T>()`，在 `OnDestroy` / `Dispose` 取消订阅
- 挂 GameObject 的单例继承 `MonoSingleton<T>`
- 热更层不能直接引用编辑器 API
- 新的 Manager/Service 必须在 `GameEntry._initializables` 显式注册并确认初始化顺序
- 注释用中文，字段 `_camelCase`，属性/方法 `PascalCase`
- 不改 `GameEntry.cs` 的结构（Core 层通过反射按固定类名/方法名加载）

## Shared Library Constraints (Src/Lib/)
- `Lib/Common/` 和 `Lib/Protocol/` 为两端共享，**只读不可修改**
- 需要在 Lib 层面改协议/公共逻辑时，通知我协调 Server 端，双方达成一致后由我操作
- 编译共享库后 DLL 自动复制到 `Assets/References/`

## Tools
- **unityMCP** — 场景操作、资源管理、Console 读取、脚本编辑
- **codegraph** — C# 符号查询和调用链分析