# HexWaterproofBuilding

Adds waterproof versions of vanilla wood-based build pieces to Valheim.

## Features

- Adds a new **Waterproof** hammer build tab
- Clones vanilla wood, darkwood, and ashwood building pieces
- Waterproof pieces do not take rain damage
- Uses vanilla build requirements plus **Resin**
- Supports normal placement, snapping, removal, persistence, and no-build-cost mode
- Vanilla-friendly gameplay integration

---

## Configuration

Config file location:

```text
BepInEx/config/hex.waterproofbuilding.cfg
```

Example configuration:

```ini
[General]

## Enable or disable the mod
# Setting type: Boolean
# Default value: true
Enabled = true
```

⚠️ Changes to the Enabled setting require a full game restart.

Waterproof pieces are registered during game initialization and cannot be safely removed at runtime.

---

## Requirements

- BepInExPack Valheim
- Jotunn

---

## Installation

### Thunderstore / r2modman

Install using a Thunderstore-compatible mod manager such as r2modman.

### Manual Installation

1. Install BepInExPack Valheim.
2. Install Jotunn.
3. Extract this package.
4. Place the DLL inside:

```text
BepInEx/plugins/HexWaterproofBuilding/
```

Example:

```text
BepInEx/plugins/HexWaterproofBuilding/HexWaterproofBuilding.dll
```

---

## Multiplayer

⚠️ Multiplayer compatibility has NOT been fully tested.

This mod has primarily been tested in single-player/client-side environments.

If using in multiplayer, installing the mod on both the server and all clients is recommended for the best chance of consistent behavior.

Potential multiplayer issues may include:

- Missing build pieces
- Incorrect persistence behavior
- Rendering inconsistencies
- Placement synchronization issues

Use in multiplayer at your own risk until additional testing is completed.

---

## Compatibility

- Uses Jotunn for prefab and piece registration
- Lightweight Harmony patching
- Designed to minimize conflicts with other mods
- Waterproof variants are separate prefabs and do not modify vanilla assets directly

---

## Notes

- Only buildable pieces are affected
- Existing vanilla pieces are unchanged
- Safe to add to an existing world
- Removing the mod may leave placed waterproof pieces unavailable or non-interactable

---