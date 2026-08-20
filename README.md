# Burglin' Gnomes — Archipelago APWorld & BepInEx Mod

> Multiworld randomizer integration for [Burglin' Gnomes](https://store.steampowered.com/app/3844970/Burglin_Gnomes/) (Steam, by fobri), built on [Archipelago](https://archipelago.gg).

---

## What's in this repo

| Folder | What it is |
|---|---|
| `burglin_gnomes/` | The Archipelago apworld (Python, drives the generator) |
| `BurglinGnomesArchipelago/` | The BepInEx mod (C#, runs in-game) |
| `burglin_gnomes.apworld` | Pre-built apworld — drop into Archipelago's `custom_worlds` folder |

---

## How it works

- Every one of the game's **50 real High-Gnome tasks** (pulled from the game's own `taskCollection.asset`) is an Archipelago location
- Completing a task in-game sends a location check to the server
- Items received from the multiworld queue in a **Pending Rewards panel** (F6 overlay) — claim them when you have inventory space
- Goal: collect enough **Gnomium Payout** progression tokens (`tasks_required` option, 5–50)

---

## Installation

### Step 1 — Install the apworld

1. Download `burglin_gnomes.apworld` from [Releases](https://github.com/brianriedel4163-ops/BurglinGnomesArchipelago/releases/latest)
2. Drop it into your Archipelago `custom_worlds` folder (or unzip into `worlds/burglin_gnomes` if running from source)
3. Restart the Archipelago launcher — "Burglin' Gnomes" will appear as a game option

### Step 2 — Generate a multiworld

Create a YAML and generate your seed:

```yaml
name: YourName
game: Burglin' Gnomes
Burglin' Gnomes:
  tasks_required: 25
  caught_link: false
```

### Step 3 — Install the BepInEx mod

Install via [Thunderstore Mod Manager](https://www.overwolf.com/app/Thunderstore-Thunderstore_Mod_Manager):
- Search for **ArchipelagoClient** by **coollb1** in the Burglin' Gnomes community

Or manually:
1. Install [BepInEx 5.x](https://docs.bepinex.dev/articles/user_guide/installation/index.html) into your game folder
2. Download `BurglinGnomesArchipelago.dll` from [Releases](https://github.com/brianriedel4163-ops/BurglinGnomesArchipelago/releases/latest)
3. Copy it (and its companion DLLs) into `BepInEx/plugins/BurglinGnomesArchipelago/`

### Step 4 — Play

1. Host your room on [archipelago.gg](https://archipelago.gg) or locally
2. Launch Burglin' Gnomes through the mod manager
3. Press **F6**, enter your server address, port, and slot name — hit Connect
4. Complete High-Gnome tasks in-game — checks fire automatically
5. Open F6 to claim received items from the Pending Rewards panel

---

## Options

| Option | Default | Description |
|---|---|---|
| `tasks_required` | 25 | How many of the 50 tasks need a Gnomium Payout to win (5–50) |
| `caught_link` | false | DeathLink-style option — reserved for future use |

---

## Location & Item Reference

### Locations (50 total)

| Group | Tasks |
|---|---|
| Toilet | Travel through toilet, Flood Toilet, Flush item toilet |
| Breaking | Break Stealables, Blender, Window Shatter, Break seagull egg |
| Explosion | Cause Explosion, Mine Explode, Mine Trigger, Microwave Metal, RPG shot |
| Violence | Stab/Kill/Shoot/Explode Random Enemy, DartStab, ForkStab, Cut entity with shears |
| Gather/Steal | Gather Random, Steal From Random Room, Steal Random Category, Steal Random Specific |
| Stun | Tase Random Enemy, Mace entity |
| Misc | Kiss Garden Gnomes, Boar charge/tusk, Water Plants, Sauna Water, Slip, and 16 more |
| Blowdart | Blowdart fart, Blowdart fly, Blowdart sleep |

### Items

- **Gnomium Payout** (progression) — the goal token, grants gnomium on claim
- **46 real filler items** — potions, crafting materials, gear, weapons (from the game's own `AllItems.asset`)

---

## Building the mod from source

Requires [.NET SDK](https://dotnet.microsoft.com/download) (v6+) and BepInEx installed in your game folder.

```bash
cd BurglinGnomesArchipelago
dotnet build -p:GamePath="C:\path\to\Burglin' Gnomes" -p:BepInExProfilePath="C:\path\to\your\profile"
```

The built DLL auto-copies into your BepInEx plugins folder.

---

## Roadmap

- [ ] Crafting sanity (each equipment craft = 1 check)
- [ ] Potion sanity (each potion craft = 1 check)
- [ ] Dart sanity (each dart craft = 1 check)
- [ ] House upgrade sanity (each home tier = 1 check)
- [ ] Time sanity (start with reduced timer, receive time extensions)
- [ ] Map sanity (each unique map completed = 1 check)
- [ ] Map randomizer (randomize which map generates each day)
- [ ] Task granularity (individual sub-variants as separate checks)
- [ ] Task priority option (force remaining unchecked tasks into generation)
- [ ] DeathLink
- [ ] Remaining checks display in F6 overlay
- [ ] Multiplayer verification

---

## Credits

- **brianriedel4163-ops** — mod development
- **fobri** — Burglin' Gnomes
- **Archipelago team** — [archipelago.gg](https://archipelago.gg)

---

## License

MIT
"# BurglinGnomesArchipelago" 
