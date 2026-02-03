#!/usr/bin/env python3
"""
Simple AI-Unity Communication Client
This is the simplest possible Python client to communicate with Unity using the JSON file polling method
"""

import json
import time
import os
import uuid
from pathlib import Path
from typing import Dict, Any, Optional

class UnityBridgeClient:
    def __init__(self, commands_folder="AICommands", responses_folder="AIResponses"):
        """
        Initialize the Unity bridge client
        
        Args:
            commands_folder: Path to folder where Unity reads commands
            responses_folder: Path to folder where Unity writes responses
        """
        self.commands_folder = Path(commands_folder)
        self.responses_folder = Path(responses_folder)
        
        # Create folders if they don't exist
        self.commands_folder.mkdir(exist_ok=True)
        self.responses_folder.mkdir(exist_ok=True)
    
    def send_command(self, action: str, params: Optional[Dict[str, Any]] = None, wait_for_response: bool = True, timeout: float = 10.0) -> Optional[Dict[str, Any]]:
        """
        Send a command to Unity and optionally wait for response
        
        Args:
            action: The action to perform (e.g., "createObject", "deleteObject")
            params: Parameters for the action
            wait_for_response: Whether to wait for Unity's response
            timeout: Maximum time to wait for response (seconds)
            
        Returns:
            Unity's response or None if not waiting
        """
        command_id = str(uuid.uuid4())
        
        command = {
            "id": command_id,
            "action": action,
            "params": params or {}
        }
        
        # Write command file
        command_file = self.commands_folder / f"{command_id}.json"
        with open(command_file, 'w') as f:
            json.dump(command, f, indent=2)
        
        print(f"Sent command {command_id}: {action}")
        
        if not wait_for_response:
            return None
        
        # Wait for response
        response_file = self.responses_folder / f"{command_id}_response.json"
        start_time = time.time()
        
        while time.time() - start_time < timeout:
            if response_file.exists():
                with open(response_file, 'r') as f:
                    response = json.load(f)
                os.remove(response_file)  # Clean up response file
                return response
            
            time.sleep(0.1)  # Poll every 100ms
        
        print(f"Timeout waiting for response to command {command_id}")
        return None
    
    def create_object(self, name: str, obj_type: str = "empty", position: Optional[list] = None, components: Optional[list] = None) -> Dict[str, Any]:
        """Create a GameObject in Unity"""
        params = {
            "name": name,
            "type": obj_type
        }
        
        if position:
            params["position"] = list(position) if position else []
        if components:
            params["components"] = list(components) if components else []
            
        response = self.send_command("createObject", params)
        return response or {}
    
    def delete_object(self, name: str) -> Dict[str, Any]:
        """Delete a GameObject in Unity"""
        response = self.send_command("deleteObject", {"name": name})
        return response or {}
    
    def get_scene_info(self) -> Dict[str, Any]:
        """Get information about the current Unity scene"""
        response = self.send_command("getSceneInfo")
        return response or {}

# Example usage
if __name__ == "__main__":
    # Initialize client
    client = UnityBridgeClient()
    
    print("Simple AI-Unity Communication Example")
    print("=====================================")
    
    # Example 1: Create a simple cube
    print("\n1. Creating a cube...")
    response = client.create_object(
        name="MyCube",
        obj_type="cube",
        position=[0, 1, 0],
        components=["Rigidbody"]
    )
    print(f"Response: {response}")
    
    # Example 2: Create an empty GameObject
    print("\n2. Creating an empty GameObject...")
    response = client.create_object(
        name="EmptyObject",
        position=[2, 0, 0]
    )
    print(f"Response: {response}")
    
    # Example 3: Get scene information
    print("\n3. Getting scene information...")
    response = client.get_scene_info()
    if response:
        print(f"Scene: {response.get('result', {}).get('sceneName', 'Unknown')}")
        print(f"Objects: {response.get('result', {}).get('objectCount', 0)}")
    
    # Example 4: Delete an object
    print("\n4. Deleting the cube...")
    response = client.delete_object("MyCube")
    print(f"Response: {response}")
    
    print("\nDone! Check Unity to see the changes.")