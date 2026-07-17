# 服务端 Agent（.NET C#）

## Responsibilities
- GameServer 业务逻辑（登录、战斗、社交、活动等系统）
- EF Core 数据库访问（SQL Server）
- 协议解析与处理
- 服务器启动/配置管理

## Scope
- `GameServer/GameServer/` — 主项目，目标 `net10.0`
- 依赖 `Lib/Common/`（目标 `net48`）和 `Lib/Protocol/`（目标 `net48`）

## Constraints
- 数据库访问必须用 `using (DBService.Instance.BeginScope()) { ... }`，Dispose 时自动 `SaveChanges()`
- 新 Service/Manager **无需手动注册** — 反射自动扫描 `IInitializable` 实现并调用 `Init()`
- 服务器代码 **不能依赖 Unity 类型**（Common 引用了 UnityEngine.dll 但仅供共享工具类）
- Common/Protocol 目标 .NET Framework 4.8（C# 8.0），不能用 .NET Core+ 独有 API
- 编译后 DLL 自动复制到 `Client/Assets/References/`
- 日志用 `Log.Info()` / `Log.Error()`，不用 `Console.WriteLine()`
- 修改 .proto 后：`Tools/genproto.bat` → `dotnet build Lib/Protocol/Protocol.csproj` → `dotnet build Lib/Common/Common.csproj`

## Shared Library Constraints (Src/Lib/)
- `Lib/Common/` 和 `Lib/Protocol/` 为两端共享，**只读不可修改**
- 需要在 Lib 层面改协议/公共逻辑时，通知我协调 Client 端，双方达成一致后由我操作

## Tools
- **codegraph** — C# 符号查询和调用链分析