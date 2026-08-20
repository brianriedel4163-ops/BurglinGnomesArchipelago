# Burglin' Gnomes — Archipelago Client

Your gnomes aren't just burgling houses anymore. They're burgling the **multiworld**.

Connect Burglin' Gnomes to an [Archipelago](https://archipelago.gg) multiworld randomizer session — every High-Gnome task your crew completes sends a location check, and items from other games land in your F6 overlay ready to claim.

---

## Setup

### 1. Get the apworld
Download `burglin_gnomes.apworld` from the [GitHub releases page](https://github.com/brianriedel4163-ops/BurglinGnomesArchipelago/releases/latest) and drop it into your Archipelago `custom_worlds` folder. Restart the Archipelago launcher.

### 2. Generate a seed
Make a YAML and generate your multiworld:
```yaml
name: YourName
game: Burglin' Gnomes
Burglin' Gnomes:
  tasks_required: 25   # how many of 50 tasks needed to win (5-50)
  caught_link: false
```

### 3. Host your room
Upload your `.archipelago` file to [archipelago.gg](https://archipelago.gg) or host locally.

### 4. Connect in-game
Launch through Mod Manager, press **F6**, fill in your server address, port, and slot name. Hit Connect.

---

## How it plays

- Your crew gets assigned High-Gnome tasks as normal
- **Complete a task → location check fires automatically**
- Items from the multiworld appear in the **Pending Rewards** panel (F6 overlay)
- Hit **Claim Next** or **Claim All** when you have inventory space — nothing gets lost if you're full
- Unclaimed items survive disconnects and session restarts

---

## What's randomized

**50 real task locations** pulled straight from the game's own data:

| Category | Examples |
|---|---|
| 🚽 Toilet | Travel through toilet, Flood toilet, Flush item |
| 💥 Explosion | Cause explosion, Microwave metal, RPG shot |
| ⚔️ Violence | Stab/kill/shoot/explode random enemy, Fork stab |
| 🎯 Blowdart | Blowdart fart, fly, or sleep dart |
| 🏠 Misc | Kiss garden gnomes, Free the dog, Build minicopter, Bee sting, and more |
| 🔨 Breaking | Break stealables, Blender, Window shatter |
| 👜 Steal | Steal from random room, Steal random category |
| ⚡ Stun | Tase random enemy, Mace entity |

**46 real filler items** as multiworld rewards — potions, gear, weapons, crafting materials, all from the game's own item list.

---

## F6 Overlay

| Panel | What it does |
|---|---|
| Left | Server connection (address, port, slot name, password) |
| Right | Pending Rewards — scrollable list of queued items, Claim Next / Claim All buttons |

---

## Coming soon

Crafting sanity · Potion sanity · House upgrade sanity · Time sanity · Map randomizer · DeathLink · Remaining checks display · Multiplayer polish

---

## Source & apworld download

[github.com/brianriedel4163-ops/BurglinGnomesArchipelago](https://github.com/brianriedel4163-ops/BurglinGnomesArchipelago)
