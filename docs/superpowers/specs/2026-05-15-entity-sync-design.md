# 实体位置同步优化方案

日期: 2026-05-15

## 问题

其他玩家角色在本地客户端的位置与在其他客户端看到的位置不一致，存在持续偏差。

## 根因

当前 `EntityController.SyncEntityTransform()` 使用**速度驱动**的开环移动方式：

```csharp
this.characterController.Move(dir * speed * Time.deltaTime);
```

问题在于：

1. **双重移动互相拉扯**：`Entity.OnUpdate()` 每帧外推逻辑坐标，`SyncEntityTransform()` 又用 speed 追赶视觉坐标。两者都在推进位置，但时机和精度不同，视觉层永远在"追"逻辑层，从未真正到达。

2. **`Time.deltaTime` 不统一**：不同客户端帧率不同，即使 speed 相同，每帧移动距离也不同，不同客户端同一实体视觉位置必然不同。

3. **纯开环无收敛**：移动量只依赖 speed 和 local delta，不依赖当前位置与目标位置的误差。误差永远存在，不会随帧数增加而消除。

## 方案

将 `SyncEntityTransform` 从**速度驱动（开环）**改为**位置驱动（闭环）**，使用指数衰减平滑（Exponential Smoothing）。

### 核心公式

```csharp
float t = 1.0f - Mathf.Exp(-k * Time.deltaTime);
Vector3 newPos = Vector3.Lerp(transform.position, targetPosition, t);
this.characterController.Move(newPos - transform.position);
```

其中 `k` 为收敛速度系数，建议值 `10`～`20`。

### 原理

- 不再用 `speed` 字段驱动移动，而是每帧向目标位置消除固定比例的剩余距离
- 距离远时移动快，距离近时移动慢，但**每帧都更接近目标**
- 收敛时间与帧率无关：`k` 决定时间常数（`1/k` 秒内收敛到约 63%），帧率只影响平滑程度

### 效果对比

| | 当前（速度驱动） | 新方案（位置驱动） |
|---|---|---|
| 收敛性 | 开环，误差可能永久存在 | 闭环，一定收敛到目标 |
| 帧率依赖 | `speed * dt` 随帧率变化 | 指数衰减，帧率只影响平滑度 |
| 速度来源 | 依赖 `entity.Speed` 字段 | 由位置差和时间差自然导出 |
| 抖动处理 | 新包到达时可能跳变 | 新包到达时平滑改变目标 |

### 改动范围

- **只改一个方法**：`EntityController.SyncEntityTransform()`（第 172-201 行）
- 瞬移保护（`distance > 5f` 直接 snap）保持不变
- 方向同步（`transform.forward`）保持不变
- 服务端无需任何修改
- `Entity.OnUpdate()` 的外推可保留，两层可以共存

### 收敛速度参考

| k | 1/k (秒) | 肉眼收敛帧数 (60fps) |
|---|---|---|
| 10 | 0.1 | ~15帧 |
| 15 | 0.067 | ~10帧 |
| 20 | 0.05 | ~7帧 |

建议从 `k=15` 开始调。
