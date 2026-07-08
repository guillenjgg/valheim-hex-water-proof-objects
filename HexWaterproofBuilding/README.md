# HexWaterproofBuilding

Adds rain-resistant versions of the vanilla workbench, wood, darkwood, and ashwood build pieces to Valheim. Includes optional extended-range placement, removal, hover highlighting, and copy-piece support for waterproof pieces, with optional support for vanilla building pieces.

> ## ⚠️ Important
> **Extended-range placement for vanilla building pieces is an optional feature and is disabled by default.**
>
> To enable it, set `VanillaPiecesEnabled = true` in the configuration file.

## Features

* Adds a new **Waterproof Building** hammer build tab
* Adds a waterproof version of the vanilla workbench
* Clones vanilla wood, darkwood, and ashwood building pieces
* Waterproof pieces do not take rain damage
* Uses vanilla build requirements plus **Resin**
* Supports vanilla placement, snapping, snap cycling, removal, and copy-piece functionality
* Optional extended-range placement, removal, hover highlighting, and copy-piece support
  * Waterproof pieces (enabled by default)
  * Vanilla building pieces (optional configuration)

### Screenshots

![Waterproof Pieces](https://raw.githubusercontent.com/guillenjgg/valheim-hex-mod-images/main/hexwaterproofbuilding/hexwaterproof_1.png)

![Extended Placement](https://raw.githubusercontent.com/guillenjgg/valheim-hex-mod-images/main/hexwaterproofbuilding/hexwaterproof_2.png)

![Extended Removal](https://raw.githubusercontent.com/guillenjgg/valheim-hex-mod-images/main/hexwaterproofbuilding/hexwaterproof_3.png)

![Copy Piece](https://raw.githubusercontent.com/guillenjgg/valheim-hex-mod-images/main/hexwaterproofbuilding/hexwaterproof_4.png)

![Waterproof Workbench](https://raw.githubusercontent.com/guillenjgg/valheim-hex-mod-images/main/hexwaterproofbuilding/hexwaterproof_5.png)

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

[Extended Placement Range]

## Enable extended-range placement, removal, hover highlighting, and copy-piece functionality for waterproof pieces
# Setting type: Boolean
# Default value: true
Enabled = true

## Enable extended-range placement, removal, hover highlighting, and copy-piece functionality for vanilla building pieces
# Setting type: Boolean
# Default value: false
VanillaPiecesEnabled = false
```

⚠️ Changes to configuration settings require a full game restart.

Waterproof pieces are registered during game initialization and cannot be safely removed at runtime.

---

## Requirements

* BepInExPack Valheim
* Jotunn

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

This mod has been tested in single-player.

The waterproof pieces themselves should synchronize normally because they are registered through Jotunn and use standard Valheim building systems.

For multiplayer, installing the mod on both the server and all clients is recommended.

---

## Compatibility

* Uses Jotunn for prefab and piece registration
* Harmony patching
* Waterproof variants are separate prefabs and do not modify vanilla assets directly
* Extended-range functionality is supported for waterproof pieces.
* Vanilla building pieces can optionally use extended-range functionality through configuration.

---

## Notes

* Only buildable pieces are affected
* Existing vanilla pieces are unchanged
* Safe to add to an existing world
* Waterproof pieces are separate prefabs and do not replace vanilla pieces
* Removing the mod may leave placed waterproof pieces unavailable or non-interactable

## Support and Feedback
Report bugs, request features, or provide feedback:

- Discord: https://discord.gg/wU2FXD94v4

## Source Code

- GitHub: https://github.com/guillenjgg/valheim-hex-water-proof-objects
