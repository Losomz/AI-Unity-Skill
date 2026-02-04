---
name: Unity-skill
description: A Unity development agent skill that injects AI services into Unity projects through Python scripts.
---

# Unity AI Service Injection Skill

This skill enables AI agents to inject AI services into Unity projects by creating necessary directory structures and service scripts using Python scripts.

## 功能特性

### 服务注入
- 创建Unity项目所需的目录结构（Assets/Editor/AI_Internal）
- 在指定目录中生成基础的AI服务脚本
- 使用Python脚本自动化注入过程

### Python脚本工具
- unity_injector.py：主要的注入工具
- 支持命令行参数指定Unity项目路径
- 自动验证Unity项目有效性
- 提供详细的错误处理和反馈

### 模板系统
- assets/templates/AIService.cs：C#服务脚本模板
- 遵循agentskills规范的组织结构
- 支持模板的轻松修改和扩展

## 使用方式

### 方法1：直接使用Python脚本
```bash
python scripts/unity_injector.py --project-path "Unity项目路径"
```

### 方法2：使用测试脚本（验证功能）
```bash
python test_injector.py
```

### Python脚本参数
- `--project-path`：Unity项目根目录路径（必需）

### 注入结果
成功注入后，会在Unity项目中创建：
```
Assets/Editor/AI_Internal/AIService.cs
```

### C#服务脚本模板
位于 `assets/templates/AIService.cs`：
```csharp
using UnityEngine;
using UnityEditor;

namespace AI_Internal
{
    public class AIService
    {
        // 基础服务类，后续可扩展
    }
}
```

## 文件结构

符合agentskills规范的项目结构：
```
unity-agent-skill/
├── SKILL.md                    # 技能说明文档
├── scripts/
│   └── unity_injector.py      # 主要的Python注入工具
├── assets/
│   └── templates/
│       └── AIService.cs        # C#服务脚本模板
└── test_injector.py           # 测试脚本
```

## 系统要求

- Python 3.6或更高版本
- 有效的Unity项目
- 对目标Unity项目目录的写权限

## 注意事项

- 确保指定的路径是有效的Unity项目（包含Assets和ProjectSettings目录）
- 确保有足够的权限在目标目录中创建文件
- 注入前请备份重要的Unity项目文件
- 确保Python已安装并添加到系统PATH（Windows用户）