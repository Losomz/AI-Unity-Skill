---
name: Unity-skill
description: A Unity development agent skill for creating, modifying, and debugging Unity projects, including scene management, component programming, and asset handling.
---

# Unity Development Agent Skill

This skill enables AI agents to interact with Unity projects through programmatic operations, providing comprehensive Unity development capabilities.

## 功能特性

### 场景管理
- 创建、修改和删除场景
- 场景对象(GameObject)的添加、配置和删除
- 层级关系管理
- 场景保存和加载

### 组件编程
- 脚本组件的创建和编辑
- 内置Unity组件的配置
- 自定义组件属性设置
- 组件间的交互逻辑实现

### 资源处理
- 预制体(Prefab)的创建和修改
- 材质和纹理的配置
- 动画剪辑的编辑
- 音频资源的处理

### 构建与部署
- 项目构建配置
- 平台特定设置
- 构建过程自动化
- 部署包生成

## 使用方式

### 场景操作示例
```csharp
// 创建新的游戏对象
var gameObject = new GameObject("NewObject");
// 添加组件
var renderer = gameObject.AddComponent<Renderer>();
// 设置位置
transform.position = new Vector3(0, 1, 0);
```

### 组件编程示例
```csharp
// 创建自定义脚本
public class CustomController : MonoBehaviour {
    public float speed = 5.0f;
    void Update() {
        transform.Translate(0, 0, speed * Time.deltaTime);
    }
}
```

## 注意事项

- 操作前请确认项目结构
- 重大修改前建议备份
- 跨平台兼容性需考虑目标平台限制
- 性能敏感操作需适当优化