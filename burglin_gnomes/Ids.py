# Base offset for this world's item/location IDs.
#
# Reused across the Burglin' Gnomes apworld and the BurglinGnomesArchipelago
# BepInEx mod -- keep IDs consistent between the two.
#
# IMPORTANT: Archipelago requires every game's item/location IDs to be globally
# unique across every world installed in a given Archipelago instance. This value
# is a placeholder pick — before using this alongside other custom worlds, check
# your local `Archipelago/worlds/*/Ids.py` (or equivalent) for collisions and
# change this if needed. There is no live registry I could check from this
# sandbox (no network access), so treat this as "probably fine, please verify."
BASE_ID = 4_269_000

ITEM_ID_BASE = BASE_ID
LOCATION_ID_BASE = BASE_ID + 1000
