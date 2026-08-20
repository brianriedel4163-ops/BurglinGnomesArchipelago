from typing import Dict, NamedTuple, Optional

from BaseClasses import ItemClassification

from .Ids import ITEM_ID_BASE

# Pulled straight from the shipped game's AllItems.asset (via AssetRipper
# export), so these are real, in-game item names. Not every item in this
# list is necessarily what TryAddItem's internal lookup expects verbatim --
# these are asset filenames (minus .asset), which should match ItemData.Name
# in most cases but hasn't been confirmed against a live game.
REAL_ITEMS = [
    "fart potion", "health potion", "confusion potion", "mobility potion",
    "sleep potion", "stamina potion", "strength potion", "flight potion",
    "metal bat", "ceramics", "chemicals", "fabric", "metal", "plastic",
    "backpack", "spring shoes", "gnomium gloves", "gnomium", "glider",
    "eyeglass", "glue gloves", "poopling", "scraplings", "pickaxe",
    "branch", "cigarette", "papyrus", "helmet", "grappling hook",
    "slingshot", "marble gun", "marble", "boxing gloves", "music box",
    "tusk", "bee", "fairywings", "blowgun", "confusion dart", "fart dart",
    "flight dart", "health dart", "mobility dart", "sleep dart",
    "stamina dart", "strength dart",
]

# Synthetic progression token -- not a real in-game item. Represents "your
# crew banked another completed High-Gnome task." See client.py /
# GameHooks.cs ApplyReceivedItem for how this is handled on receipt (it
# also grants real in-game "gnomium" as a side effect, so it's not a dead
# item client-side).
PAYOUT_ITEM = "Gnomium Payout"


class ItemData(NamedTuple):
    code: Optional[int]
    classification: ItemClassification


def _build_item_table() -> Dict[str, ItemData]:
    table: Dict[str, ItemData] = {}
    next_id = ITEM_ID_BASE

    table[PAYOUT_ITEM] = ItemData(next_id, ItemClassification.progression)
    next_id += 1

    for name in REAL_ITEMS:
        table[name] = ItemData(next_id, ItemClassification.filler)
        next_id += 1

    return table


item_table: Dict[str, ItemData] = _build_item_table()
item_name_to_id: Dict[str, int] = {
    name: data.code for name, data in item_table.items() if data.code is not None
}
