from typing import Dict, List

from .Ids import LOCATION_ID_BASE

# Pulled from the shipped game's taskCollection.asset (via AssetRipper),
# grouped exactly as the game groups them for its own random task-picking.
# Group structure isn't used for AP logic (everything's in one flat region
# -- see Regions.py) but is kept here for reference/documentation.
TASK_GROUPS: Dict[str, List[str]] = {
    "Toilet related": ["Travel through toilet", "Flood Toilet", "Flush item toilet"],
    "Breaking": ["Break Stealables", "Blender", "Window Shatter", "Break seagull egg"],
    "Explosion": ["Cause Explosion", "Mine Explode", "Mine Trigger", "Microwave Metal", "RPG shot"],
    "Violence": [
        "Stab Random Enemy", "Kill Random Enemy", "Shoot Random Enemy",
        "Explode Random Enemy", "DartStab", "ForkStab", "Cut entity with shears",
    ],
    "Gather/Steal": [
        "Gather Random", "Steal From Random Room",
        "Steal Random Category", "Steal Random Specific",
    ],
    "Stun entity": ["Tase Random Enemy", "Mace entity"],
    "Misc": [
        "Kiss Garden Gnomes", "Boar charge", "Boar Tusk", "Water Plants",
        "Sauna Water", "Slip", "GarageDoorOpen", "Jonathan spawned",
        "JonathanMusicBox", "Break TV", "Watering Can filled", "Cut Grass",
        "Cut plants", "EnterGreenhouse", "EnterShed", "Build minicopter",
        "Free dog", "Steal dog toy", "Window Open Task", "Open Cabinet",
        "Bee sting", "Unlock weaponsafe",
    ],
    "Blowdart": ["Blowdart fart", "Blowdart fly", "Blowdart sleep"],
}

ALL_TASK_NAMES: List[str] = [name for names in TASK_GROUPS.values() for name in names]


def location_name_for_task(task_name: str) -> str:
    # Must match the naming convention client-side (GameHooks.cs:
    # $"Task: {taskRef.name}") exactly.
    return f"Task: {task_name}"


def _build_location_table() -> Dict[str, int]:
    table: Dict[str, int] = {}
    next_id = LOCATION_ID_BASE
    for task_name in ALL_TASK_NAMES:
        table[location_name_for_task(task_name)] = next_id
        next_id += 1
    return table


location_name_to_id: Dict[str, int] = _build_location_table()
