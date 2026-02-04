# Unity 通信桥接 MVP 方案

## 目标
实现 AI Agent 与 Unity 编辑器之间的基础通信能力。

---

## 最小化架构

```mermaid
sequenceDiagram
    participant Agent as AI Agent (PowerShell/Curl)
    participant Server as Unity HTTP Server
    participant Main as Unity 主线程

    Agent->>Server: POST http://127.0.0.1:8081/execute
    Note right of Agent: {"command":"ping"}
    
    Server->>Main: 压入命令队列
    Main->>Main: 执行命令 (EditorApplication.update)
    Main-->>Server: 返回结果
    Server-->>Agent: {"status":"ok"}
```

## 下一步

通信验证成功后，继续实现：
1. **主线程调度器** (MainThreadDispatcher)
2. **命令解析和执行** (ExecuteCommand)
3. **5个基础命令** (ping, CreateCube, DeleteObject, GetHierarchy, Log)

参考完整方案: `docs/full_implementation_plan.md`
