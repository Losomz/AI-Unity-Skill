#!/usr/bin/env python3
"""
Unity AI Service Injector

This script injects a basic AI service into Unity projects by creating the necessary
directory structure and service script files.
"""

import os
import sys
import argparse
from pathlib import Path


def create_directories(project_path):
    """Create the required directory structure in Unity project."""
    try:
        # Create Assets/Editor/AI_Internal directory
        ai_internal_dir = Path(project_path) / "Assets" / "Editor" / "AI_Internal"
        ai_internal_dir.mkdir(parents=True, exist_ok=True)
        return str(ai_internal_dir)
    except Exception as e:
        print(f"Error creating directories: {e}")
        return None


def create_service_script(target_dir):
    """Create Unity service script in the specified directory."""
    try:
        # Read C# template from assets/templates/AIService.cs
        script_path = Path(__file__).parent.parent / "assets" / "templates" / "AIService.cs"
        with open(script_path, 'r') as f:
            script_content = f.read()
        
        # Write the template to the target directory
        target_script_path = Path(target_dir) / "AIService.cs"
        with open(target_script_path, 'w') as f:
            f.write(script_content)
        return str(target_script_path)
    except Exception as e:
        print(f"Error creating service script: {e}")
        return None


def validate_unity_project(project_path):
    """Validate if the specified path is a valid Unity project."""
    project_path = Path(project_path)
    
    # Check if project directory exists
    if not project_path.exists():
        print(f"Error: Project path does not exist: {project_path}")
        return False
    
    # Check for Assets directory
    assets_dir = project_path / "Assets"
    if not assets_dir.exists():
        print(f"Error: Assets directory not found. This may not be a Unity project: {project_path}")
        return False
    
    # Check for ProjectSettings directory
    project_settings_dir = project_path / "ProjectSettings"
    if not project_settings_dir.exists():
        print(f"Warning: ProjectSettings directory not found. This may not be a Unity project: {project_path}")
        return False
    
    return True


def main():
    """Main function to handle command line arguments and execute injection."""
    parser = argparse.ArgumentParser(description='Inject AI service into Unity project')
    parser.add_argument('--project-path', required=True, 
                        help='Path to Unity project root directory')
    
    args = parser.parse_args()
    
    print(f"Unity AI Service Injector")
    print(f"Target project: {args.project_path}")
    
    # Validate Unity project
    if not validate_unity_project(args.project_path):
        print("Failed to validate Unity project. Aborting.")
        sys.exit(1)
    
    # Create directories
    ai_internal_dir = create_directories(args.project_path)
    if not ai_internal_dir:
        print("Failed to create directories. Aborting.")
        sys.exit(1)
    
    print(f"Created directory structure: {ai_internal_dir}")
    
    # Create service script
    script_path = create_service_script(ai_internal_dir)
    if not script_path:
        print("Failed to create service script. Aborting.")
        sys.exit(1)
    
    print(f"Created service script: {script_path}")
    print("AI service injection completed successfully!")


if __name__ == "__main__":
    main()