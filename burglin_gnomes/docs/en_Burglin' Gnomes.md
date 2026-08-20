# Burglin' Gnomes

## Where is the settings page?

Check out the [player options page](../player-options) for the options you can set: `tasks_required` and
`caught_link`.

## What does randomization do to this game?

This is a real integration for the Steam game **Burglin' Gnomes** (by fobri), played through the
[BurglinGnomesArchipelago](https://github.com/) BepInEx mod (or the fallback standalone client if you're
not using the mod). Every one of the game's 50 real High-Gnome tasks -- pulled directly from the shipped
game's own `taskCollection.asset` -- is an Archipelago location. Complete a task in-game and it sends a
location check.

## What is the goal of Burglin' Gnomes when randomized?

Collect `tasks_required` copies of the "Gnomium Payout" progression item. One copy is placed at exactly
that many of the 50 task locations; the rest hold real in-game filler items (potions, crafting materials,
gear, weapons -- also pulled from the shipped game's `AllItems.asset`).

## What items and locations get shuffled?

- **Locations:** the 50 real tasks -- toilet mischief, breaking things, explosions, violence, stealing,
  stunning enemies, misc chaos (Kiss Garden Gnomes, Break TV, etc.), and blowdart shenanigans.
- **Items:** "Gnomium Payout" (progression, synthetic) plus 46 real items as filler.

## What does another world's item look like in Burglin' Gnomes?

Same as local filler -- completing a task sends whatever was placed there, yours or someone else's.
