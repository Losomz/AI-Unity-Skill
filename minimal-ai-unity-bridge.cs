using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System;

// Minimal AI-Unity Communication Bridge
// Save as Assets/Editor/AIUnityBridge.cs
// This is the simplest possible implementation using JSON file polling

public class AIUnityBridge : EditorWindow
{
    private string commandsFolder = "AICommands";
    private string responsesFolder = "AIResponses";
    private float pollInterval = 0.5f;
    private float lastPollTime;
    private bool isProcessing = false;
    
    [MenuItem("AI/Open Unity Bridge")]
    public static void ShowWindow()
    {
        GetWindow<AIUnityBridge>("AI Unity Bridge");
    }
    
    void OnEnable()
    {
        // Create folders if they don't exist
        if (!Directory.Exists(commandsFolder))
            Directory.CreateDirectory(commandsFolder);
        if (!Directory.Exists(responsesFolder))
            Directory.CreateDirectory(responsesFolder);
            
        lastPollTime = (float)EditorApplication.timeSinceStartup;
    }
    
    void OnInspectorUpdate()
    {
        if (isProcessing) return;
        
        if (EditorApplication.timeSinceStartup - lastPollTime > pollInterval)
        {
            CheckForCommands();
            lastPollTime = (float)EditorApplication.timeSinceStartup;
        }
    }
    
    void CheckForCommands()
    {
        if (!Directory.Exists(commandsFolder)) return;
        
        try
        {
            var commandFiles = Directory.GetFiles(commandsFolder, "*.json");
            foreach (var file in commandFiles)
            {
                isProcessing = true;
                ProcessCommand(file);
                File.Delete(file); // Remove after processing
                isProcessing = false;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"AI Bridge Error: {e.Message}");
            isProcessing = false;
        }
    }
    
    void ProcessCommand(string filePath)
    {
        try
        {
            string json = File.ReadAllText(filePath);
            var command = JsonUtility.FromJson<AICommand>(json);
            
            var response = new AIResponse
            {
                id = command.id,
                timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };
            
            // Process different command types
            switch (command.action.ToLower())
            {
                case "createobject":
                    response.result = CreateObject(command.@params);
                    response.status = "success";
                    break;
                    
                case "deleteobject":
                    response.result = DeleteObject(command.@params);
                    response.status = "success";
                    break;
                    
                case "getsceneinfo":
                    response.result = GetSceneInfo();
                    response.status = "success";
                    break;
                    
                default:
                    response.status = "error";
                    response.error = $"Unknown action: {command.action}";
                    break;
            }
            
            // Write response
            string responseJson = JsonUtility.ToJson(response, true);
            string responsePath = Path.Combine(responsesFolder, $"{command.id}_response.json");
            File.WriteAllText(responsePath, responseJson);
            
            Debug.Log($"AI Bridge: Processed command {command.id} - {command.action}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"AI Bridge Error processing command: {e.Message}");
            
            // Write error response
            var errorResponse = new AIResponse
            {
                id = "error",
                status = "error",
                error = e.Message,
                timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };
            
            string errorJson = JsonUtility.ToJson(errorResponse, true);
            string errorPath = Path.Combine(responsesFolder, "error_response.json");
            File.WriteAllText(errorPath, errorJson);
        }
    }
    
    object CreateObject(CommandParams parameters)
    {
        if (parameters == null || string.IsNullOrEmpty(parameters.name))
        {
            throw new System.Exception("Object name is required");
        }
        
        GameObject go = new GameObject(parameters.name);
        
        // Set position if provided
        if (parameters.position != null && parameters.position.Length >= 3)
        {
            go.transform.position = new Vector3(
                parameters.position[0], 
                parameters.position[1], 
                parameters.position[2]
            );
        }
        
        // Add default mesh if type is specified
        if (!string.IsNullOrEmpty(parameters.type))
        {
            switch (parameters.type.ToLower())
            {
                case "cube":
                    var meshFilter = go.AddComponent<MeshFilter>();
                    meshFilter.mesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");
                    go.AddComponent<MeshRenderer>();
                    break;
                case "sphere":
                    var sphereFilter = go.AddComponent<MeshFilter>();
                    sphereFilter.mesh = Resources.GetBuiltinResource<Mesh>("Sphere.fbx");
                    go.AddComponent<MeshRenderer>();
                    break;
                case "empty":
                default:
                    // Just leave as empty GameObject
                    break;
            }
        }
        
        // Add components if specified
        if (parameters.components != null)
        {
            foreach (var componentName in parameters.components)
            {
                try
                {
                    var componentType = System.Type.GetType($"UnityEngine.{componentName}, UnityEngine");
                    if (componentType != null)
                    {
                        go.AddComponent(componentType);
                    }
                }
                catch
                {
                    // Component addition failed, continue with others
                }
            }
        }
        
        return new { 
            message = $"Created GameObject '{parameters.name}'",
            objectId = go.GetInstanceID().ToString()
        };
    }
    
    object DeleteObject(CommandParams parameters)
    {
        if (parameters == null || string.IsNullOrEmpty(parameters.name))
        {
            throw new System.Exception("Object name is required for deletion");
        }
        
        GameObject[] objects = GameObject.FindObjectsOfType<GameObject>();
        int deletedCount = 0;
        
        foreach (GameObject go in objects)
        {
            if (go.name == parameters.name)
            {
                Undo.DestroyObjectImmediate(go);
                deletedCount++;
            }
        }
        
        return new { 
            message = $"Deleted {deletedCount} objects named '{parameters.name}'",
            count = deletedCount
        };
    }
    
    object GetSceneInfo()
    {
        var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        var objects = GameObject.FindObjectsOfType<GameObject>();
        
        var objectList = new List<object>();
        foreach (var go in objects)
        {
            objectList.Add(new {
                name = go.name,
                id = go.GetInstanceID().ToString(),
                position = new float[] { go.transform.position.x, go.transform.position.y, go.transform.position.z },
                components = GetComponentNames(go)
            });
        }
        
        return new {
            sceneName = scene.name,
            objectCount = objects.Length,
            objects = objectList
        };
    }
    
    string[] GetComponentNames(GameObject go)
    {
        var components = go.GetComponents<Component>();
        var names = new string[components.Length];
        
        for (int i = 0; i < components.Length; i++)
        {
            names[i] = components[i].GetType().Name;
        }
        
        return names;
    }
    
    void OnGUI()
    {
        GUILayout.Label("AI-Unity Communication Bridge", EditorStyles.boldLabel);
        GUILayout.Space(10);
        
        GUILayout.Label($"Commands Folder: {commandsFolder}");
        GUILayout.Label($"Responses Folder: {responsesFolder}");
        GUILayout.Label($"Poll Interval: {pollInterval} seconds");
        GUILayout.Label($"Status: {(isProcessing ? "Processing..." : "Waiting for commands")}");
        
        GUILayout.Space(20);
        
        if (GUILayout.Button("Open Commands Folder"))
        {
            EditorUtility.RevealInFinder(commandsFolder);
        }
        
        if (GUILayout.Button("Open Responses Folder"))
        {
            EditorUtility.RevealInFinder(responsesFolder);
        }
        
        if (GUILayout.Button("Create Test Command"))
        {
            CreateTestCommand();
        }
    }
    
    void CreateTestCommand()
    {
        var testCommand = new AICommand
        {
            id = "test_" + DateTime.Now.Ticks,
            action = "createObject",
            @params = new CommandParams
            {
                name = "TestCube",
                type = "cube",
                position = new float[] { 0, 1, 0 },
                components = new string[] { "Rigidbody" }
            }
        };
        
        string json = JsonUtility.ToJson(testCommand, true);
        string testPath = Path.Combine(commandsFolder, $"{testCommand.id}.json");
        File.WriteAllText(testPath, json);
        
        Debug.Log($"AI Bridge: Created test command at {testPath}");
    }
    
    // Data structures for JSON serialization
    [System.Serializable]
    public class AICommand
    {
        public string id;
        public string action;
        public CommandParams @params;
    }
    
    [System.Serializable]
    public class CommandParams
    {
        public string name;
        public string type;
        public float[] position;
        public string[] components;
        // Add more parameters as needed
    }
    
    [System.Serializable]
    public class AIResponse
    {
        public string id;
        public string status;
        public object result;
        public string error;
        public string timestamp;
    }
}