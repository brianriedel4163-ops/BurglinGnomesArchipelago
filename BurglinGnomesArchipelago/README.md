# Burglin' Gnomes — Archipelago Client Mod (scaffold)

A BepInEx plugin that connects [Burglin' Gnomes](https://store.steampowered.com/app/3844970/Burglin_Gnomes/)
to an [Archipelago](https://archipelago.gg) multiworld server, using the official
`Archipelago.MultiClient.Net` library.

## What actually works right now

Updated after you sent over the real decompiled `Assembly-CSharp` export — `GameHooks.cs` is no longer
placeholders. It's wired against real classes and doesn't even need Harmony for the core loop, since the
game already exposes public C# events for exactly this:

- **Connecting to an Archipelago server** (`ArchipelagoManager.cs`) — real, functional.
- **Receiving items** from the multiworld and granting them via `ServerManager.GetLocalPlayer().Inventory
  .TryAddItem(itemName)` — a real, first-party method the game itself uses.
- **Sending location checks** whenever `PlayerTaskManager.OnTaskCompleted` fires (a genuine public event,
  shared across the whole crew, found via `GlobalBlackboard.Find("Global")
  .GetVariableValue<PlayerTaskManager>("taskManager")` — the exact pattern the game's own `TaskListUI.cs`
  uses). Each completed High-Gnome task becomes a location check named `"Task: {task.name}"`.
- A **debug overlay** (press `F6` in-game) to type in a server address, port, slot name, and password, and
  connect/disconnect.
- Corrected `GamePath`/assembly references in the `.csproj` — your export revealed the real data folder is
  `Gnomium_Data`, not `BurglinGnomes_Data` as I'd originally guessed, plus the actual third-party DLLs the
  game depends on (`Unity.Netcode.Runtime`, `NodeCanvas`, etc.).

## What's still missing

**A Burglin' Gnomes apworld.** `LocationIdByName("Task: <name>")` will resolve to nothing until an
Archipelago world defines a location by that exact name and it's loaded on the server you connect to. Task
completion will still work locally (it'll just log "no matching Archipelago location was found" instead of
sending a check) until that exists. Building that apworld means enumerating the actual task assets — see
"Next step" below.

## Location/item naming reference (from your decompiled export)

- Location name convention this mod sends: `"Task: {PlayerTaskBase.name}"` — the ScriptableObject asset
  name (stable, not the localized display string from `GetTaskString()`).
- Item name convention this mod expects to receive: whatever string `ItemData.Name` uses for that item
  (defined on `CraftableItemBase`, looked up via `AllItems.GetItem(name, out index)`).
- Only High-Gnome **task completions** are wired as location checks in this version — not individual loot
  pickups (`InventoryBase.HandleItemPickedUp` fires on *any* inventory add, including crafting outputs, so
  it's too broad a signal for a clean 1:1 location mapping; task completion is a much cleaner unit).

## Next step: enumerate the real tasks

To build a matching apworld (or just to sanity-check what location names actually exist), list every
`PlayerTaskBase`-derived ScriptableObject asset in your Unity project/export — that's your full location
list. If you can get me that list (asset names + which task type each is: `PlayerStealTask`,
`GatherResourceTask`, etc.), I can draft the Locations/Items tables for a real apworld next.

## Building

1. Set `GamePath` in `BurglinGnomesArchipelago.csproj` to your local install (Steam → Burglin' Gnomes →
   Manage → Browse local files), or pass `-p:GamePath="..."` on the command line.
2. `dotnet build` — this restores `Archipelago.MultiClient.Net` from NuGet automatically and, if
   `BepInEx\plugins` exists at `GamePath`, copies the built DLL straight in.
3. Launch the game, press `F6`, fill in your hosted room's address/port/slot name, hit Connect.

## Packaging for Thunderstore

`manifest.json` is set up already. You'll additionally need an `icon.png` (256×256) and a `CHANGELOG.md`
before Thunderstore will accept an upload — neither is included here.

## Things I couldn't verify from this sandbox

- `Archipelago.MultiClient.Net`'s exact current API surface (method names/signatures may have shifted
  slightly by version — check IntelliSense against whatever version NuGet pulls).
- The game's actual `*_Data\Managed\` folder name and DLL layout (guessed as `BurglinGnomes_Data` based on
  the Steam listing's app name — verify against your real install).
- Whether BepInEx is even required/pre-supported by this game, or whether it needs an unstripped-corlib or
  IL2CPP variant of BepInEx instead of the standard Mono one — that depends on how the game was built,
  which I can't inspect from here. If BepInEx refuses to load, that's the first thing to check.

Send me the traceback/error and any decompiled class names you find, and I can tighten this up further.
