# Unity AI Agent Skill

基于OpenAI Function Calling标准的Unity开发AI Agent Skills系统，提供全面的Unity项目开发支持。

## 项目概述

本项目是一个专门为Unity开发设计的AI Agent技能，允许AI助手直接与Unity项目交互，执行场景管理、组件编程、资源处理和项目构建等操作。该项目遵循[agentskills.io](https://agentskills.io)标准规范，实现了一个完整且可扩展的Unity开发自动化解决方案。

## AgentSkills.io 标准

[Agent Skills](https://agentskills.io)是一个简单、开放的标准格式，用于赋予AI代理新的能力和专业知识。本项目严格遵循该标准：

- **标准格式**: 符合agentskills.io规范要求
- **目录结构**: 遵循推荐的技能目录组织方式
- **元数据规范**: 标准化的技能描述和配置格式
- **兼容性**: 可与所有支持agentskills.io的AI代理产品集成

## 系统架构设计

### 混合交互模式

本系统采用**命令行调用与编辑器脚本相结合**的混合方案：

- **命令行接口**: 适合批量操作、项目构建、导入导出等宏观任务
- **编辑器脚本**: 适合细粒度的场景操作、对象管理、资源处理等微观任务
- **通信协议**: 使用JSON文件作为agent与Unity之间的通信介质

### 核心架构

```
┌─────────────────┐    JSON通信协议    ┌──────────────────┐
│   AI Agent      │◄─────────────────►│  Unity编辑器脚本  │
└─────────────────┘                  └──────────────────┘
         │                                    │
         │ 命令行接口                        │ Unity Editor API
         ▼                                    ▼
┌─────────────────┐                  ┌──────────────────┐
│  Unity命令行    │                  │  Unity项目       │
└─────────────────┘                  └──────────────────┘
```

## 核心能力

- 🎮 **场景操作**: 创建、编辑、删除Unity场景及游戏对象
- 🧩 **组件编程**: 脚本开发、组件配置、交互逻辑实现
- 🎨 **资源管理**: 预制体、材质、动画等Unity资源的处理
- 🚀 **构建部署**: 项目构建配置、多平台支持、自动化部署
- 🧪 **测试自动化**: 单元测试、集成测试、性能分析

## 功能模块详细说明

### 1. 场景管理模块
- 场景创建、打开、保存和关闭
- 游戏对象(GameObject)的添加、修改和删除
- 组件(Component)的动态添加和配置
- 层级关系管理和场景组织

### 2. 脚本编写模块
- C#脚本模板生成和创建
- 代码语法检查和自动完成
- 脚本间依赖关系管理
- 脚本组件的自动挂载和配置

### 3. 资源管理模块
- 资源导入、导出和组织
- 预制体(Prefab)的创建和修改
- 材质(Material)和纹理(Texture)的配置
- 动画(Animation)和音频(Audio)资源的处理

### 4. 构建部署模块
- 多平台构建配置(Windows、macOS、Linux、iOS、Android等)
- 构建参数自动化设置
- CI/CD集成支持
- 部署包生成和管理

### 5. 测试自动化模块
- 单元测试和集成测试执行
- 性能分析和优化建议
- 测试报告生成
- 回归测试自动化

## 技术特点

- 兼容Unity 2022.1及以上版本
- 基于agentskills.io标准规范
- 支持C#脚本自动生成
- 可与现有CI/CD流程集成
- 异步任务处理机制
- 插件化架构设计

## 实施计划

### 开发阶段
1. **核心框架搭建**: 建立基础的通信协议和任务处理机制
2. **基础功能实现**: 实现场景管理和脚本编写等核心功能
3. **高级功能扩展**: 添加资源管理和构建部署等高级特性
4. **测试和优化**: 全面测试和性能优化
5. **文档和示例**: 完善文档和使用示例

### 文件结构规划
```
unity-agent-skill/
├── SKILL.md              # 技能说明文档(符合agentskills.io标准)
├── README.md              # 项目说明文档
├── scripts/              # 脚本工具
│   ├── unity-bridge.py   # 与Unity通信的Python桥梁
│   └── task-processor.py # 任务处理器
├── unity-editor/         # Unity编辑器脚本
│   ├── EditorScripts/    # 编辑器脚本源码
│   │   ├── Core/         # 核心功能脚本
│   │   ├── Scene/        # 场景管理脚本
│   │   ├── Script/       # 脚本处理脚本
│   │   ├── Asset/        # 资源管理脚本
│   │   ├── Build/        # 构建部署脚本
│   │   └── Test/         # 测试自动化脚本
│   └── EditorResources/  # 资源和配置文件
│       ├── Templates/     # 脚本模板
│       └── Config/        # 配置文件
├── examples/             # 使用示例
│   ├── BasicScene/       # 基础场景操作示例
│   ├── ScriptGeneration/ # 脚本生成示例
│   └── BuildAutomation/   # 构建自动化示例
├── tests/                # 测试用例
└── docs/                 # 详细文档
    ├── api/              # API文档
    └── tutorials/        # 教程文档
```

## 通信协议设计

### 任务描述格式
```json
{
  "taskId": "unique-task-id",
  "type": "scene/create-object",
  "parameters": {
    "name": "Player",
    "position": [0, 0, 0],
    "components": ["Renderer", "Rigidbody"]
  },
  "priority": "normal",
  "timeout": 30000
}
```

### 结果返回格式
```json
{
  "taskId": "unique-task-id",
  "status": "completed",
  "result": {
    "objectId": "12345",
    "message": "GameObject created successfully"
  },
  "executionTime": 1250,
  "errors": []
}
```

## 快速开始

### 环境配置
1. 安装Unity Hub 3.0+
2. 安装Unity Editor 2022.1+
3. 克隆此项目到本地
4. 将unity-editor文件夹中的脚本复制到您的Unity项目中

### 使用步骤
1. 将技能配置添加到你的AI Agent系统
2. 配置Unity项目路径和相关设置
3. 启动Unity编辑器(可选择命令行模式)
4. 开始使用自然语言指令进行Unity开发

## 使用示例

### 场景操作示例
```
"创建一个名为Player的立方体，并添加移动控制脚本"
"将当前场景配置为可运行状态，设置分辨率为1920x1080"
"为游戏对象添加物理材质，设置摩擦系数为0.5"
```

### 脚本生成示例
```
"生成一个简单的旋转脚本，使对象每秒旋转30度"
"创建一个FPS控制脚本，包含WASD移动和鼠标视角控制"
```

### 构建自动化示例
```
"为当前项目创建Windows平台的构建配置"
"设置Android构建参数：API级别30、最小API级别26、纹理压缩ASTC"
```

## 开发指南

### 自定义功能扩展
1. 在`unity-editor/EditorScripts/`中创建新的功能模块
2. 实现标准的任务处理接口
3. 更新通信协议以支持新功能
4. 编写相应的测试用例

### 与AI Agent集成
本技能使用标准的agentskills.io格式，可以轻松与支持该标准的AI Agent系统集成：

1. 将整个技能目录复制到Agent的skills目录
2. 确保Agent系统支持文件读写操作
3. 配置Unity编辑器路径和项目路径
4. 开始使用自然语言指令进行Unity开发

## 兼容性

- Unity Hub 3.0+
- Unity Editor 2022.1+
- Windows 10/11, macOS 10.15+, Ubuntu 20.04+
- Python 3.7+ (用于桥梁脚本)
- 支持agentskills.io标准的AI Agent系统

## 参考资源

- [Agent Skills 官方文档](https://agentskills.io)
- [Agent Skills 规范说明](https://agentskills.io/specification.md)
- [Unity 编辑器脚本API文档](https://docs.unity3d.com/Manual/EditorScripting.html)
- [Unity 命令行参数文档](https://docs.unity3d.com/Manual/EditorCommandLineArguments.html)