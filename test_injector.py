#!/usr/bin/env python3
"""
Test script for Unity AI Service Injector

This script can test the injector with a mock Unity project or use the current directory.
"""

import os
import sys
import tempfile
import shutil
import argparse
from pathlib import Path

# Add the scripts directory to the Python path
current_dir = os.path.dirname(os.path.abspath(__file__))
scripts_dir = os.path.join(current_dir, 'scripts')
if scripts_dir not in sys.path:
    sys.path.insert(0, scripts_dir)

try:
    from unity_injector import main, create_directories, create_service_script, validate_unity_project
except ImportError as e:
    print(f"Error importing unity_injector: {e}")
    print(f"Scripts directory: {scripts_dir}")
    print(f"Files in scripts directory: {os.listdir(scripts_dir) if os.path.exists(scripts_dir) else 'Directory not found'}")
    sys.exit(1)


def create_mock_unity_project():
    """Create a mock Unity project structure for testing."""
    temp_dir = tempfile.mkdtemp(prefix="test_unity_project_")
    
    # Create Unity project structure
    assets_dir = Path(temp_dir) / "Assets"
    assets_dir.mkdir()
    
    project_settings_dir = Path(temp_dir) / "ProjectSettings"
    project_settings_dir.mkdir()
    
    return temp_dir


def test_with_mock_project():
    """Test the injector with a mock Unity project."""
    print("Creating mock Unity project...")
    mock_project_path = create_mock_unity_project()
    print(f"Mock project created at: {mock_project_path}")
    
    try:
        # Test the injector with the mock project
        sys.argv = ['unity_injector.py', '--project-path', mock_project_path]
        main()
        
        # Verify the results
        ai_service_path = Path(mock_project_path) / "Assets" / "Editor" / "AI_Internal" / "AIService.cs"
        if ai_service_path.exists():
            print("✓ Test passed: AI service script was created successfully")
            with open(ai_service_path, 'r') as f:
                content = f.read()
                print(f"✓ Script content verification:")
                print(content)
        else:
            print("✗ Test failed: AI service script was not created")
        
    except Exception as e:
        print(f"✗ Test failed with error: {e}")
    finally:
        # Clean up
        print(f"Cleaning up mock project at: {mock_project_path}")
        shutil.rmtree(mock_project_path, ignore_errors=True)


def test_with_current_dir():
    """Test the injector using the current directory as a mock Unity project."""
    current_dir = Path.cwd()
    print(f"Using current directory as test project: {current_dir}")
    
    # Create minimal Unity project structure in current directory if it doesn't exist
    assets_dir = current_dir / "Assets"
    if not assets_dir.exists():
        print("Creating Assets directory in current location...")
        assets_dir.mkdir()
    
    project_settings_dir = current_dir / "ProjectSettings"
    if not project_settings_dir.exists():
        print("Creating ProjectSettings directory in current location...")
        project_settings_dir.mkdir()
    
    try:
        # Test the injector with the current directory
        sys.argv = ['unity_injector.py', '--project-path', str(current_dir)]
        main()
        
        # Verify the results
        ai_service_path = current_dir / "Assets" / "Editor" / "AI_Internal" / "AIService.cs"
        if ai_service_path.exists():
            print("✓ Test passed: AI service script was created successfully in current directory")
            with open(ai_service_path, 'r') as f:
                content = f.read()
                print(f"✓ Script content verification:")
                print(content)
        else:
            print("✗ Test failed: AI service script was not created")
        
    except Exception as e:
        print(f"✗ Test failed with error: {e}")


def main():
    """Main function to handle command line arguments and execute tests."""
    parser = argparse.ArgumentParser(description='Test Unity AI Service Injector')
    parser.add_argument('--use-current-dir', action='store_true', 
                        help='Use current directory as mock Unity project')
    
    args = parser.parse_args()
    
    if args.use_current_dir:
        test_with_current_dir()
    else:
        test_with_mock_project()


if __name__ == "__main__":
    main()