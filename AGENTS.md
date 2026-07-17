# AGENTS.md

MMORPG — Unity 2022.3 + .NET Server。

## 必须遵守的规则

你必须严格遵守以下规则，只写最少、最必要的代码，禁止无用代码：

1. 语言规范：使用中文回答我，回答简短易懂又不失专业性
2. 编码前思考：不猜测需求，有歧义先问，不隐藏困惑。
3. 简洁优先：只解决当前问题，不提前设计、不加抽象、不加未要求功能。
4. 精准修改：只改必须改的代码，不优化注释、不重构、不乱改格式。
5. 目标驱动：先明确成功标准，用可验证结果闭环，不盲目写代码。
6. 全面思考：不应该只局限当前文件，应该代码联系上下文，确定当前业务解决的问题，从而解决我提出的问题。所有代码：够用即可、拒绝臃肿、拒绝过度工程、拒绝无关改动。

## 必须知道的命令

```bash
# 修改 .proto 后：代码生成 → 重新编译共享库（顺序不可反）
cd Tools && genproto.bat
dotnet build Src/Lib/Protocol/Protocol.csproj
if ($?) { dotnet build Src/Lib/Common/Common.csproj }

# 修改 Excel 配置表后生成 JSON
cd Src/Data && Excel2Json.cmd

# 启动服务器（需先配好 appsettings.json 中的 SQL Server 连接串）
dotnet run --project Src/Server/GameServer/GameServer/GameServer.csproj
```

## 框架兼容性陷阱

- **Common / Protocol 目标 `net48`**（C# 8.0），不要使用 .NET Core+ 独有 API
- **Common 引用了 `UnityEngine.dll`**（通过 HintPath），服务器代码不要依赖 Unity 类型
- **GameServer 目标 `net10.0`**，可以正常使用 EF Core 9.x
- **编译 Common/Protocol 后 DLL 会自动复制到** `Src/Client/Assets/References/`（PostBuild 事件）

## 两种 Singleton，别混用

```csharp
// 共享库（Common/Protocol）用这个 —— 纯 C#，new() 约束
public class MyManager : Singleton<MyManager>, IInitializable { }

// 客户端 Unity 侧用这个 —— MonoBehaviour，挂 GameObject
public class MyComp : MonoSingleton<MyComp> { }
```

## 服务器 vs 客户端：启动注册机制完全不同

| | 服务器 | 客户端 |
|---|---|---|
| 新 Service/Manager 注册 | **无需手动注册** — `GameServer.ServiceInit()` 通过反射自动扫描 `IInitializable` 实现并调用 `Init()` | **必须手动添加** — 在 `GameEntry._initializables` 列表中显式注册，顺序决定初始化先后 |
| 文件位置 | `GameServer.cs:37` `ServiceInit()` | `GameEntry.cs:18` `_initializables` |

## 数据库访问模式（强制）

```csharp
// 使用 using 包裹，Dispose 时自动 SaveChanges()
// DBService 内部使用 ThreadLocal<DbContext>，每线程独立上下文
using (DBService.Instance.BeginScope())
{
    var player = DBService.Instance.Entities.TPlayers.FirstOrDefault(...);
    player.Level = 10;
}
// 绝对不要在 using 块外访问 DBService.Instance.Entities
```

## 没有测试、没有 CI、没有 lint

此项目无测试框架、无 CI 流水线、无 editorconfig。改代码后通过 `dotnet build` 验证编译即可。

## CodeGraph 可用

此仓库已初始化 `.codegraph/`。查符号定义、调用链、影响范围等结构性问题时优先用 codegraph 工具，比 grep 快且更准确。

## 代码规范速记

- 注释用中文
- 字段 `_camelCase`，属性/方法 `PascalCase`
- 一个文件一个类（紧密关联的 POCO 例外）
- 服务器日志用 `Log.Info()`/`Log.Error()`，不用 `Console.WriteLine()`
- 客户端跨模块通信用 `EventManager.Publish()`/`Subscribe()`，记得 `OnDestroy`/`Dispose` 取消订阅
- T4 模板 `Entities.tt` 生成 EF Core 实体 `Entities.cs`（不要手动编辑生成文件）

## 不要随意重构的文件

- `GameEntry.cs` — Core 层通过反射按固定类名/方法名加载
- `message.proto` — 顶层消息封套，改动即协议断裂
- `*.asmdef` 文件 — Unity 程序集边界定义
