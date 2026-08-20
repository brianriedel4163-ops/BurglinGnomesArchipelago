from dataclasses import dataclass

from Options import Range, Toggle, PerGameCommonOptions

from .Locations import ALL_TASK_NAMES

TOTAL_TASKS = len(ALL_TASK_NAMES)  # 50, from the real game's taskCollection.asset


class TasksRequired(Range):
    """How many High-Gnome tasks your crew needs to complete (i.e. how many
    of the 50 real task locations need a "Gnomium Payout" progression item
    collected) to reach your goal."""
    display_name = "Tasks Required"
    range_start = 5
    range_end = TOTAL_TASKS
    default = 25


class CaughtLink(Toggle):
    """If enabled, when one of your gnomes gets caught, every other
    CaughtLink-enabled player's gnome gets caught too. Flavor/DeathLink-style
    option -- no hook is wired for this yet on the client side (see README);
    it's here for future use once a "player caught" event is found in the
    game's code."""
    display_name = "CaughtLink"


@dataclass
class BurglingGnomesOptions(PerGameCommonOptions):
    tasks_required: TasksRequired
    caught_link: CaughtLink
