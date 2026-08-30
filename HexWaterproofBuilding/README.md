# HexWaterproofBuilding

Adds rain-resistant building pieces, wood and bone stacks, automatic pier supports, and optional extended-range building functionality to Valheim. Also removes the roof requirement for vanilla and waterproof workbenches.

> ## ⚠️ Important
>
> **Extended-range functionality for vanilla building pieces is optional and disabled by default.**
>
> To enable it, set `VanillaPiecesEnabled = true` in the configuration file.
>
> Changes to configuration settings require a full game restart.

## Features

- Adds a new **Waterproof Building** hammer build tab
- Adds a new **Pier** hammer build tab
- Adds rain-resistant versions of the vanilla workbench, wood, darkwood, and ashwood building pieces
- Wood and bone stacks do not take rain damage by default
- Removes the roof requirement for vanilla and waterproof workbenches by default
- Uses vanilla build requirements plus **Resin** for waterproof pieces
- Adds a **4m Vertical Pier Support** that automatically extends to the seabed
- Pier supports cannot be placed on dry land
- Supports vanilla placement, snapping, snap cycling, removal, and copy-piece functionality
- Optional extended-range placement, removal, hover highlighting, and copy-piece support
  - Waterproof building pieces (enabled by default)
  - Vanilla building pieces (disabled by default)
- Configuration options to restore rain damage for wood and bone stacks or require roofs for workbenches

### Screenshots

![Waterproof Pieces](https://raw.githubusercontent.com/guillenjgg/valheim-hex-mod-images/main/hexwaterproofbuilding/hexwaterproof_1.png)

![Extended Placement](https://raw.githubusercontent.com/guillenjgg/valheim-hex-mod-images/main/hexwaterproofbuilding/hexwaterproof_2.png)

![Extended Removal](https://raw.githubusercontent.com/guillenjgg/valheim-hex-mod-images/main/hexwaterproofbuilding/hexwaterproof_3.png)

![Copy Piece](https://raw.githubusercontent.com/guillenjgg/valheim-hex-mod-images/main/hexwaterproofbuilding/hexwaterproof_4.png)

![Waterproof Workbench](https://raw.githubusercontent.com/guillenjgg/valheim-hex-mod-images/main/hexwaterproofbuilding/hexwaterproof_5.png)

![Pier Support](https://raw.githubusercontent.com/guillenjgg/valheim-hex-mod-images/main/hexwaterproofbuilding/hexwaterproof_6.png)

---

## Configuration

Config file location:

```text
BepInEx/config/hex.waterproofbuilding.cfg
```

Example configuration:

```ini
[General]

## Enable or disable the Waterproof Building mod.
# Setting type: Boolean
# Default value: true
Enabled = true

## Require a roof for workbenches to function.
# Setting type: Boolean
# Default value: false
WorkBenchRequireRoof = false

## Wood and bone stacks will take rain damage if this value is set to true.
# Setting type: Boolean
# Default value: false
WoodAndBoneStacksTakeRainDamage = false

[Extended Placement Range]

## Enable extended placement range for waterproof pieces.
# Setting type: Boolean
# Default value: true
Enabled = true

## Enable extended placement range for vanilla building pieces.
# Setting type: Boolean
# Default value: false
VanillaPiecesEnabled = false
```

> ⚠️ Changes to configuration settings require a full game restart.
>
> Waterproof pieces and pier supports are registered during game initialization and cannot be safely added or removed at runtime.

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

This mod has not been extensively tested in multiplayer or on a dedicated server.

Install this mod on the server and all clients.

Custom waterproof pieces and pier supports are synchronized through Jotunn and use Valheim's standard building systems.

---

## Compatibility

- Uses Jotunn for prefab and piece registration
- Uses Harmony to extend placement, workbench, and rain-damage behavior
- Waterproof pieces are separate prefabs and do not replace vanilla building pieces
- Vanilla building pieces are only affected by extended-range functionality when `VanillaPiecesEnabled = true`
- Wood and bone stack rain damage can be restored through configuration
- Workbench roof requirements can be restored through configuration

---

## Notes

- Pier supports automatically generate additional support sections based on water depth
- Removing the mod may leave placed waterproof pieces and pier supports unavailable or non-interactable

---

## Support and Feedback

Report bugs, request features, or provide feedback:

- Discord: [https://discord.gg/wU2FXD94v4](https://discord.gg/wU2FXD94v4)

## Source Code

- GitHub: https://github.com/guillenjgg/valheim-hex-water-proof-objects