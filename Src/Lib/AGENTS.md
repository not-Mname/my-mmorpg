# 共享库（只读）

此目录为 Client 和 Server 的共同依赖，禁止直接修改。

## Directory Structure
- `proto/` — `.proto` 协议定义文件（.proto → genproto.bat → C#）
- `Protocol/` — Proto 生成的 C# 代码，目标 `net48`
- `Common/` — 共享工具类（Singleton、Buffer、Network 等），目标 `net48`

## Modification Workflow
1. 向用户提出修改请求，说明内容和原因
2. 用户确认两端无冲突后授权修改
3. 修改后依次执行：
   ```bash
   cd Tools && genproto.bat
   dotnet build Lib/Protocol/Protocol.csproj
   dotnet build Lib/Common/Common.csproj
   ```
4. DLL 自动复制到 `Src/Client/Assets/References/`

## Notes
- Common/Protocol 目标 `net48`（C# 8.0），不要使用 .NET Core+ 独有 API
- Common 引用了 `UnityEngine.dll`，但服务器代码不要依赖 Unity 类型