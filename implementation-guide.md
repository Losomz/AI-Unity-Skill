# AI-Unity Communication: Simplest Implementation Guide

## Overview

This guide provides the absolute simplest way to establish communication between an AI agent and Unity. The solution uses JSON file polling, which requires minimal setup and no external dependencies.

## Quick Start

### Step 1: Unity Setup

1. Create a new Unity project or open an existing one
2. Create a folder named `Editor` in your `Assets` folder (if it doesn't exist)
3. Copy `minimal-ai-unity-bridge.cs` into the `Assets/Editor` folder
4. Open Unity Editor
5. Go to `AI > Open Unity Bridge` to open the communication window

The bridge will automatically create two folders:
- `AICommands` - Where AI writes commands
- `AIResponses` - Where Unity writes responses

### Step 2: AI Client Setup

1. Install Python 3.7+ if not already installed
2. Save `simple-ai-unity-client.py` to your desired location
3. Run the Python script to test the connection

### Step 3: Test Communication

1. In Unity, make sure the "AI Unity Bridge" window is open
2. Click "Create Test Command" in the Unity window
3. Run the Python client script
4. You should see responses printed in the Python console

## How It Works

### Communication Flow

```
AI Agent → Write JSON to AICommands folder → Unity Bridge → Execute → Write JSON to AIResponses folder → AI Agent reads response
```

### JSON Command Format

```json
{
  "id": "unique-command-id",
  "action": "createObject",
  "params": {
    "name": "GameObjectName",
    "type": "cube",
    "position": [0, 1, 0],
    "components": ["Rigidbody"]
  }
}
```

### JSON Response Format

```json
{
  "id": "unique-command-id",
  "status": "success",
  "result": {
    "message": "Created GameObject 'GameObjectName'",
    "objectId": "12345"
  },
  "timestamp": "2023-01-01 12:00:00"
}
```

## Supported Commands

### Basic Commands

1. **createObject** - Create a new GameObject
   - `name` (required): Object name
   - `type` (optional): "cube", "sphere", or "empty"
   - `position` (optional): [x, y, z] coordinates
   - `components` (optional): Array of component names to add

2. **deleteObject** - Delete GameObject(s) by name
   - `name` (required): Name of objects to delete

3. **getSceneInfo** - Get information about the current scene
   - No parameters required

### Examples

#### Python Client Usage

```python
from simple_ai_unity_client import UnityBridgeClient

# Initialize client
client = UnityBridgeClient()

# Create a cube with physics
response = client.create_object(
    name="PhysicsCube",
    obj_type="cube",
    position=[0, 2, 0],
    components=["Rigidbody"]
)

# Get scene information
scene_info = client.get_scene_info()
print(f"Scene has {scene_info['result']['objectCount']} objects")

# Delete an object
client.delete_object("PhysicsCube")
```

#### Manual JSON Command

Create a file named `test.json` in the `AICommands` folder:
```json
{
  "id": "manual-test",
  "action": "createObject",
  "params": {
    "name": "ManualCube",
    "type": "cube",
    "position": [1, 1, 1]
  }
}
```

Unity will process this automatically and create a response file in `AIResponses`.

## Implementation Details

### Unity Bridge Script

The `minimal-ai-unity-bridge.cs` script provides:
- Automatic file polling every 0.5 seconds
- JSON command parsing and execution
- Response generation
- Error handling
- Simple UI for monitoring and testing

### Python Client

The `simple-ai-unity-client.py` script provides:
- Simple command sending interface
- Automatic response waiting
- Type hints for better IDE support
- Example usage

## Advantages of This Approach

1. **Minimal Setup**: Only 2 files required (1 Unity script, 1 Python client)
2. **No Dependencies**: Uses built-in Unity and Python capabilities
3. **Easy Debugging**: Just read/write JSON files
4. **Extensible**: Easy to add new commands
5. **Cross-Platform**: Works on Windows, macOS, and Linux
6. **Version Independent**: Works with any modern Unity version

## Limitations

1. **Polling Delay**: Commands are processed with up to 0.5 second delay
2. **No Real-time**: Not suitable for real-time game control
3. **File System Based**: Requires write access to folders
4. **Single Process**: Only one Unity instance can process commands at a time

## Extending the System

### Adding New Commands

1. Add a new case in the `ProcessCommand` method in the Unity script:
```csharp
case "myNewCommand":
    response.result = MyNewCommand(command.@params);
    response.status = "success";
    break;
```

2. Implement the command method:
```csharp
object MyNewCommand(CommandParams parameters)
{
    // Your command logic here
    return new { message = "Command executed" };
}
```

3. Add a convenience method to the Python client:
```python
def my_new_command(self, param1, param2):
    return self.send_command("myNewCommand", {"param1": param1, "param2": param2})
```

### Adding New Parameters

Extend the `CommandParams` class in the Unity script:
```csharp
[System.Serializable]
public class CommandParams
{
    // Existing parameters...
    public string newParameter;
    public float[] newPosition;
}
```

## Troubleshooting

### Unity Not Responding

1. Make sure the "AI Unity Bridge" window is open
2. Check Unity Console for errors
3. Verify `AICommands` and `AIResponses` folders exist
4. Ensure Unity has permission to access the folders

### Python Client Timeout

1. Increase the timeout parameter in `send_command()`
2. Check that Unity is running and processing commands
3. Verify folder paths are correct

### Commands Not Executing

1. Check JSON format is valid
2. Ensure required parameters are provided
3. Look for error messages in Unity Console
4. Check response files in `AIResponses` folder

## Alternative Approaches

If this simple file polling approach doesn't meet your needs, consider:

1. **HTTP Server**: For real-time communication
2. **Unity Command Line**: For batch operations
3. **PlayerPrefs**: For simple state storage
4. **ScriptableObjects**: For complex data structures

See `ai-unity-communication-research.md` for more details on these alternatives.