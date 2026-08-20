from BaseClasses import Region

from .Locations import ALL_TASK_NAMES, location_name_for_task


def create_regions(world) -> None:
    """The real game is one shared house across a run, not discrete gated
    zones -- every task is available from the start (the game's own logic
    decides which tasks are active each run, not Archipelago). So this is
    intentionally flat: Menu -> House, no entrances, no access rules.
    """
    multiworld = world.multiworld
    player = world.player

    menu = Region("Menu", player, multiworld)
    multiworld.regions.append(menu)

    house = Region("House", player, multiworld)
    for task_name in ALL_TASK_NAMES:
        loc_name = location_name_for_task(task_name)
        house.locations.append(world.create_location(loc_name, house))
    multiworld.regions.append(house)

    menu.connect(house, "Start the shift")
