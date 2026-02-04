# Unity AI Service Injection Skill

一个符合agentskills.io规范的Unity项目AI服务注入工具，通过Python脚本将AI服务动态注入到Unity项目中。

## 项目概述

本项目实现了Unity AI服务注入功能，允许AI Agent将服务脚本动态部署到Unity项目中。项目遵循[agentskills.io](https://agentskills.io)标准规范，提供简洁而强大的Unity项目自动化能力。

## 核心功能

### 服务注入
- 创建Unity项目所需的目录结构（Assets/Editor/AI_Internal）
- 在指定目录中生成基础的AI服务脚本
- 使用Python脚本自动化注入过程

### 模板系统
- assets/templates/AIService.cs：C#服务脚本模板
- 遵循agentskills规范的组织结构
- 支持模板的轻松修改和扩展

## 项目结构

符合agentskills规范的项目结构：
```
unity-agent-skill/
├── SKILL.md                    # 技能说明文档（符合agentskills规范）
├── README.md                   # 项目说明文档
├── scripts/
│   └── unity_injector.py      # 主要的Python注入工具
├── assets/
│   └── templates/
│       └── AIService.cs        # C#服务脚本模板
└── test_injector.py           # 测试脚本
```

## 使用方法

### 方法1：直接使用Python脚本
```bash
python scripts/unity_injector.py --project-path "Unity项目路径"
```

### 方法2：使用测试脚本
```bash
python test_injector.py
```
（这将创建一个模拟Unity项目并测试注入功能）

### 方法3：使用当前目录测试
```bash
python test_injector.py --use-current-dir
```
（这将使用当前目录作为模拟Unity项目进行测试）

### Python脚本参数
- `--project-path`：Unity项目根目录路径（必需）

## 注入结果

成功注入后，将在Unity项目中创建：
```
Assets/Editor/AI_Internal/AIService.cs
```

### C#服务脚本模板内容
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

## 技术特点

- 符合agentskills.io标准规范
- 简洁的Python注入工具
- 模块化的模板系统
- 支持命令行参数
- 包含完整的错误处理和项目验证

## 系统要求

- Python 3.6或更高版本
- 有效的Unity项目
- 对目标Unity项目目录的写权限

## 故障排除

1. **Python未找到错误**：确保Python已安装并添加到系统PATH
2. **权限错误**：确保有权限在Unity项目目录中创建文件和文件夹
3. **无效的Unity项目**：确保指定的路径包含有效的Unity项目结构（Assets和ProjectSettings文件夹）
4. **模板文件未找到**：确保 `assets/templates/AIService.cs` 文件存在

## 下一步

注入成功后，您可以在Unity编辑器中打开项目，并查看注入的服务脚本。该脚本位于Assets/Editor/AI_Internal/AIService.cs。您可以根据需要修改此脚本以添加更多功能。

## 符合agentskills规范

项目结构完全符合agentskills规范：
- 包含必需的 `SKILL.md` 文件
- 使用标准的 `scripts/` 目录存放可执行代码
- 使用标准的 `assets/` 目录存放模板资源
- 文件引用使用相对路径

## 参考资源

- [Agent Skills 官方文档](https://agentskills.io)
- [Agent Skills 规范说明](https://agentskills.io/specification.md)
- [Unity 编辑器脚本API文档](https://docs.unity3d.com/Manual/EditorScripting.html)