from typing import List

from BaseClasses import Item, Location, Tutorial
from worlds.AutoWorld import World, WebWorld
from worlds.LauncherComponents import Component, Type, components, launch_subprocess

from .Items import REAL_ITEMS, PAYOUT_ITEM, item_table, item_name_to_id
from .Locations import location_name_to_id, ALL_TASK_NAMES
from .Options import BurglingGnomesOptions
from .Regions import create_regions
from .Rules import set_rules


def launch_client():
    from .client import launch
    launch_subprocess(launch, name="BurglinGnomesClient")


components.append(
    Component("Burglin' Gnomes Client", func=launch_client, component_type=Type.CLIENT)
)


class BurglingGnomesItem(Item):
    game = "Burglin' Gnomes"


class BurglingGnomesLocation(Location):
    game = "Burglin' Gnomes"


class BurglingGnomesWeb(WebWorld):
    theme = "partyTime"
    tutorials = [
        Tutorial(
            "Multiworld Setup Guide",
            "A guide to setting up Burglin' Gnomes for an Archipelago multiworld game.",
            "English",
            "setup_en.md",
            "setup/en",
            ["Claude"],
        )
    ]


class BurglingGnomesWorld(World):
    """
    Real integration for Burglin' Gnomes (Steam, by fobri). Your crew's
    High-Gnome hands out tasks -- steal things, cause chaos, break stuff.
    Every one of the game's 50 real tasks is an Archipelago location;
    complete enough of them (tasks_required) to reach your goal.
    """

    game = "Burglin' Gnomes"
    web = BurglingGnomesWeb()

    options_dataclass = BurglingGnomesOptions
    options: BurglingGnomesOptions

    item_name_to_id = item_name_to_id
    location_name_to_id = location_name_to_id

    item_name_groups = {
        "Filler": set(REAL_ITEMS),
    }

    def create_regions(self) -> None:
        create_regions(self)

    def create_location(self, name: str, region) -> BurglingGnomesLocation:
        return BurglingGnomesLocation(
            self.player, name, self.location_name_to_id[name], region
        )

    def create_item(self, name: str) -> BurglingGnomesItem:
        data = item_table[name]
        return BurglingGnomesItem(name, data.classification, data.code, self.player)

    def create_items(self) -> None:
        required = self.options.tasks_required.value
        total_locations = len(ALL_TASK_NAMES)  # 50
        pool: List[BurglingGnomesItem] = []

        for _ in range(required):
            pool.append(self.create_item(PAYOUT_ITEM))

        remaining = total_locations - required
        for _ in range(remaining):
            pool.append(self.create_item(self.random.choice(REAL_ITEMS)))

        self.multiworld.itempool += pool

    def set_rules(self) -> None:
        set_rules(self)

    def fill_slot_data(self):
        return {
            "tasks_required": self.options.tasks_required.value,
            "caught_link": bool(self.options.caught_link.value),
        }
