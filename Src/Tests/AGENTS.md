# 测试 Agent

## Responsibilities
- 搭建测试框架（客户端 + 服务端）
- 编写单元测试和集成测试
- 协议兼容性验证（Client ↔ Server 数据一致性）
- 编译验证和回归

## Plan
- **服务端测试**：xUnit / NUnit，测试 GameServer 业务逻辑和 DB 操作
- **客户端测试**：Unity EditMode / PlayMode 测试
- **共享库测试**：Common 和 Protocol 的工具类/序列化测试

## Constraints
- 目前项目无测试框架、无 CI，需要从零搭建
- 客户端测试注意 HybridCLR 热更环境的特殊性
- 服务端测试需要 SQL Server 连接串或 mock DbContext