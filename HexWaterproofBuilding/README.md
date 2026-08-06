# HexWaterproofBuilding

- Adds rain-resistant versions of the vanilla workbench, wood, darkwood, and ashwood build pieces to Valheim.
- Adds a **4m Vertical Pier Support** that automatically extends to the seabed and cannot be placed on dry land.
- Includes optional extended-range placement, removal, hover highlighting, and copy-piece support for both waterproof and vanilla building pieces.
- Removes the roof requirement for the vanilla and waterproof workbench.

> ## ⚠️ Important
>
> **Extended-range placement for vanilla building pieces is optional and disabled by default.**
>
> To enable it, set `VanillaPiecesEnabled = true` in the configuration file.
>
> Changes to configuration settings require a full game restart.

## Features

- Adds a new **Waterproof Building** hammer build tab
- Adds a new **Pier** hammer build tab
- Adds a waterproof version of the vanilla workbench
- Removes the roof requirement for the vanilla and waterproof workbench
- Clones vanilla wood, darkwood, and ashwood building pieces
- Waterproof pieces do not take rain damage
- Adds a **4m Vertical Pier Support** that automatically extends to the seabed
- Pier supports cannot be placed on dry land
- Uses vanilla build requirements plus **Resin** for waterproof pieces
- Supports vanilla placement, snapping, snap cycling, removal, and copy-piece functionality
- Optional extended-range placement, removal, hover highlighting, and copy-piece support
  - Waterproof building pieces (enabled by default)
  - Vanilla building pieces (optional configuration)

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

## Require a roof for vanilla and waterproof workbenches
# Setting type: Boolean
# Default value: false
WorkBenchRequireRoof = false
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

This mod has not been extensively tested in multiplayer, or on a dedicate server.

Install this mod on the server and all clients.

Custom waterproof pieces and pier supports are synchronized through Jotunn and use Valheim's standard building systems.

---

## Compatibility

- Uses Jotunn for prefab and piece registration
- Uses Harmony to extend placement and workbench behavior
- Waterproof pieces are separate prefabs and do not modify vanilla building pieces
- Pier supports are custom building pieces that automatically extend to the seabed
- Extended-range functionality is available for waterproof pieces by default
- Vanilla building pieces can optionally use extended-range functionality through configuration

---

## Notes

- Waterproof pieces are separate prefabs and do not replace vanilla building pieces
- Pier supports automatically generate additional support sections based on water depth
- Removing the mod may leave placed waterproof pieces and pier supports unavailable or non-interactable

## Support and Feedback

Report bugs, request features, or provide feedback:

- Discord: https://discord.gg/wU2FXD94v4

## Source Code

- GitHub: https://github.com/guillenjgg/valheim-hex-water-proof-objects