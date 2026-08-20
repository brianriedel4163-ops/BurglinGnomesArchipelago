# Burglin' Gnomes Setup Guide

## Required Software

- Archipelago, with the `burglin_gnomes.apworld` installed.
- The game itself (Steam), with the BurglinGnomesArchipelago BepInEx mod installed (see that mod's own
  README for build/install steps) -- or the fallback standalone `client.py` if you're not modding the game
  yet.

## Installation

1. Copy `burglin_gnomes.apworld` into your Archipelago install's `custom_worlds` folder, or unzip it into
   `worlds/burglin_gnomes` if running from source.
2. Restart the Archipelago launcher.
3. "Burglin' Gnomes" should appear as a selectable game when building a YAML.

## Generating a YAML

`game: Burglin' Gnomes`, with `tasks_required` (5-50, default 25) and `caught_link` options.

## Playing

With the BepInEx mod: press F6 in-game, connect, then just play -- completing High-Gnome tasks sends
location checks automatically, and received items get granted to your inventory.

Without the mod (fallback client): launch "Burglin' Gnomes Client" from the Archipelago launcher, connect,
then use `/tasks` to see what's left and `/complete <task name>` to manually mark one done (useful for
testing before the mod's real hooks are finished).

## Known Limitations

The task list and item list come from a real AssetRipper export of the shipped game, so location/item
*names* should be accurate. What's not yet verified against a live game: whether `TryAddItem`'s internal
name matching is exact-string or something looser, and whether all 50 tasks are always available or some
are locked behind story/room progression the apworld doesn't currently model.
