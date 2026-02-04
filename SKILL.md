---
name: Unity-skill
description: A Unity development agent skill that injects AI services into Unity projects through Python scripts.
---

# Unity AI Service Injection Skill

This skill enables AI agents to inject AI services into Unity projects by directly copying template assets.

## Usage

When the user requests to use this skill, the agent should:

1. Identify the `assets/templates` directory within this skill's folder.
2. Recursively copy all contents from `assets/templates` to the User's Unity Project Root.
3. Validate that critical files (e.g., `Assets/Editor/AI_Internal/AIService.cs`) have been successfully created.

## Behavior
Recursively copies all contents from `assets/templates` into the target Unity project root.

## Requirements
- Valid Unity Project