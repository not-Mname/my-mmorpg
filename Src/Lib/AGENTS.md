# Lib Agent（共享库）

## 授权与边界

- `Src/Lib` 是 Client/Server 的共同契约；未获得用户明确授权时全部只读。
- 获权后仅可写 `Src/Lib/proto/`、`Src/Lib/Protocol/`、`Src/Lib/Common/` 中已批准的范围；Client、Server 与 `Src/Tests` 源码只读，构建产物 `Src/Client/Assets/References/` 仅可在子任务明确授权时更新。
- 修改前由 `orchestrator` 在 `docs/contracts/` 稳定下游契约，说明 Client/Server 兼容性、迁移顺序和回滚影响。

## 生成与兼容

- 顺序：`.proto` -> `Tools/genproto.bat` -> `Protocol.csproj` -> `Common.csproj`；构建后 DLL 复制到 Client `Assets/References/`。
- `Tools/genproto.bat` 未跟踪。协议修改时若生成器缺失，立即报告阻塞；禁止手工修改 `Protocol` 生成代码。
- `Protocol` 与 `Common` 使用 `net48`、C# 8.0；不得使用 .NET Core+ 专有 API。Server 消费方不得依赖 Unity 类型。

## 验证与 handoff

- 在授权和生成器可用时，按构建顺序进行最小充分验证；生成器或下游环境不可用须记录。
- handoff：变更文件、授权与契约变更、关键决策、验证结果、风险、剩余工作。
