using UnityEngine;
using UnityEditor;
using System;
using System.Net;
using System.Text;
using System.Threading;
using System.IO;
using System.Collections.Generic;

namespace AI_Internal
{
    [InitializeOnLoad]
    public class AIBridgeServer
    {
        private static HttpListener listener;
        private static Thread listenerThread;
        private static bool isRunning = false;
        private const int PORT = 8081;
        
        // 主线程任务队列
        private static Queue<Action> mainThreadQueue = new Queue<Action>();
        private static object queueLock = new object();

        static AIBridgeServer()
        {
            // Unity 启动时自动初始化
            EditorApplication.update += Initialize;
        }

        private static void Initialize()
        {
            EditorApplication.update -= Initialize;
            EditorApplication.update += ProcessMainThreadQueue;  // 注册主线程处理器
            StartServer();
        }
        
        // 主线程队列处理器（每帧调用）
        private static void ProcessMainThreadQueue()
        {
            lock (queueLock)
            {
                while (mainThreadQueue.Count > 0)
                {
                    var action = mainThreadQueue.Dequeue();
                    try
                    {
                        action?.Invoke();
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"[AI Bridge] Main thread error: {e.Message}");
                    }
                }
            }
        }

        [MenuItem("AI/Start Server")]
        public static void StartServer()
        {
            if (isRunning)
            {
                Debug.LogWarning("[AI Bridge] Server already running");
                return;
            }

            try
            {
                listener = new HttpListener();
                listener.Prefixes.Add($"http://127.0.0.1:{PORT}/");
                listener.Start();
                isRunning = true;

                listenerThread = new Thread(ListenLoop);
                listenerThread.IsBackground = true;
                listenerThread.Start();

                Debug.Log($"[AI Bridge] Server started on http://127.0.0.1:{PORT}");
            }
            catch (Exception e)
            {
                Debug.LogError($"[AI Bridge] Failed: {e.Message}");
            }
        }

        [MenuItem("AI/Stop Server")]
        public static void StopServer()
        {
            if (!isRunning) return;

            isRunning = false;
            listener?.Stop();
            listener?.Close();

            Debug.Log("[AI Bridge] Server stopped");
        }

        private static void ListenLoop()
        {
            while (isRunning)
            {
                try
                {
                    var context = listener.GetContext();
                    ThreadPool.QueueUserWorkItem(_ => HandleRequest(context));
                }
                catch (Exception e)
                {
                    if (isRunning) Debug.LogError($"[AI Bridge] Error: {e.Message}");
                }
            }
        }

        private static void HandleRequest(HttpListenerContext context)
        {
            var request = context.Request;
            var response = context.Response;

            try
            {
                string responseText = "";

                if (request.HttpMethod == "GET")
                {
                    // 健康检查
                    responseText = "{\"status\":\"ok\",\"service\":\"Unity AI Bridge\"}";
                }
                else if (request.HttpMethod == "POST")
                {
                    // 读取请求体
                    using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                    {
                        string requestBody = reader.ReadToEnd();
                        Debug.Log($"[AI Bridge] Received: {requestBody}");
                        
                        // 解析并执行命令
                        responseText = ExecuteCommand(requestBody);
                    }
                }

                // 返回响应
                byte[] buffer = Encoding.UTF8.GetBytes(responseText);
                response.ContentLength64 = buffer.Length;
                response.ContentType = "application/json";
                response.OutputStream.Write(buffer, 0, buffer.Length);
            }
            catch (Exception e)
            {
                response.StatusCode = 500;
                byte[] buffer = Encoding.UTF8.GetBytes($"{{\"error\":\"{e.Message}\"}}");
                response.OutputStream.Write(buffer, 0, buffer.Length);
            }
            finally
            {
                response.OutputStream.Close();
            }
        }

        private static string ExecuteCommand(string json)
        {
            try
            {
                // 简单解析 command 字段
                string command = ParseField(json, "command");
                Debug.Log(json);
                Debug.Log($"[AI Bridge] Executing command: {command}");
                
                if (command == "CreateCube")
                {
                    // 解析参数
                    float x = ParseFloat(json, "x");
                    float y = ParseFloat(json, "y");
                    float z = ParseFloat(json, "z");
                    
                    Debug.Log($"[AI Bridge] Parameters: x={x}, y={y}, z={z}");
                    
                    // 在主线程执行
                    string result = null;
                    Exception error = null;
                    var resetEvent = new ManualResetEventSlim(false);
                    
                    lock (queueLock)
                    {
                        mainThreadQueue.Enqueue(() =>
                        {
                            try
                            {
                                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                                cube.transform.position = new Vector3(x, y, z);
                                
                                int id = cube.GetInstanceID();
                                string name = cube.name;
                                
                                Debug.Log($"[AI Bridge] Created Cube at ({x}, {y}, {z}), ID: {id}");
                                
                                result = $"{{\"status\":\"success\",\"id\":{id},\"name\":\"{name}\"}}";
                            }
                            catch (Exception ex)
                            {
                                error = ex;
                            }
                            finally
                            {
                                resetEvent.Set();
                            }
                        });
                    }
                    
                    // 等待主线程执行完成（最多5秒）
                    if (resetEvent.Wait(5000))
                    {
                        if (error != null)
                        {
                            return $"{{\"status\":\"error\",\"message\":\"{error.Message}\"}}";
                        }
                        return result;
                    }
                    else
                    {
                        return "{\"status\":\"error\",\"message\":\"Execution timeout\"}";
                    }
                }
                else
                {
                    return $"{{\"status\":\"error\",\"message\":\"Unknown command: {command}\"}}";
                }
            }
            catch (Exception e)
            {
                return $"{{\"status\":\"error\",\"message\":\"{e.Message}\"}}";
            }
        }

        // 简单的 JSON 字段解析（支持有引号和无引号两种格式）
        private static string ParseField(string json, string fieldName)
        {
            // 尝试方式1: "fieldName":"value" (标准 JSON)
            string pattern1 = $"\"{fieldName}\":\"";
            int start = json.IndexOf(pattern1);
            
            if (start >= 0)
            {
                start += pattern1.Length;
                int end = json.IndexOf("\"", start);
                if (end >= 0)
                {
                    return json.Substring(start, end - start);
                }
            }
            
            // 尝试方式2: fieldName:value (无引号格式，curl 在 Windows 上可能产生)
            string pattern2 = $"{fieldName}:";
            start = json.IndexOf(pattern2);
            
            if (start >= 0)
            {
                start += pattern2.Length;
                
                // 跳过可能的空格
                while (start < json.Length && json[start] == ' ')
                {
                    start++;
                }
                
                // 找到值的结束位置（逗号或右花括号）
                int end = json.IndexOfAny(new char[] { ',', '}' }, start);
                
                if (end >= 0)
                {
                    string value = json.Substring(start, end - start).Trim();
                    return value;
                }
            }
            
            return "";
        }

        private static float ParseFloat(string json, string fieldName)
        {
            string pattern = $"\"{fieldName}\":";
            int start = json.IndexOf(pattern);
            if (start < 0) return 0;
            
            start += pattern.Length;
            int end = json.IndexOfAny(new char[] { ',', '}' }, start);
            
            string value = json.Substring(start, end - start).Trim();
            float.TryParse(value, out float result);
            
            return result;
        }
    }
}
