# HexWaterproofBuilding

Adds rain-resistant building pieces, wood and bone stacks, automatic pier supports, and optional extended-range building functionality to Valheim.

Also removes the roof requirement for vanilla and waterproof workbenches by default.

> ## ⚠️ Important
>
> Extended-range functionality for vanilla building pieces is disabled by default.
>
> Enable it with `VanillaPiecesEnabled = true`.
>
> Configuration changes require a full game restart.

## Features

- Adds a **Waterproof Building** hammer build tab
- Adds a **Pier** hammer build tab
- Adds rain-resistant versions of the vanilla workbench, wood, darkwood, and ashwood building pieces
- Prevents vanilla wood and bone stacks from taking rain damage by default
- Removes the roof requirement for vanilla and waterproof workbenches by default
- Uses vanilla build requirements plus **Resin** for waterproof pieces
- Adds a **4m Vertical Pier Support** that automatically extends to the seabed
- Pier supports cannot be placed on dry land
- Supports normal placement, snapping, snap cycling, removal, and copy-piece functionality
- Supports optional extended-range placement, removal, hover highlighting, and copy-piece functionality
  - Waterproof building pieces: enabled by default
  - Vanilla building pieces: disabled by default

### Screenshots

![Waterproof Pieces](https://raw.githubusercontent.com/guillenjgg/valheim-hex-mod-images/main/hexwaterproofbuilding/hexwaterproof_1.png)

![Extended Placement](https://raw.githubusercontent.com/guillenjgg/valheim-hex-mod-images/main/hexwaterproofbuilding/hexwaterproof_2.png)

![Extended Removal](https://raw.githubusercontent.com/guillenjgg/valheim-hex-mod-images/main/hexwaterproofbuilding/hexwaterproof_3.png)

![Copy Piece](https://raw.githubusercontent.com/guillenjgg/valheim-hex-mod-images/main/hexwaterproofbuilding/hexwaterproof_4.png)

![Waterproof Workbench](https://raw.githubusercontent.com/guillenjgg/valheim-hex-mod-images/main/hexwaterproofbuilding/hexwaterproof_5.png)

![Pier Support](https://raw.githubusercontent.com/guillenjgg/valheim-hex-mod-images/main/hexwaterproofbuilding/hexwaterproof_6.png)

## Configuration

Config file:

`BepInEx/config/hex.waterproofbuilding.cfg`

Key settings:

- `VanillaPiecesEnabled` - Enables extended-range functionality for vanilla building pieces
- `WoodAndBoneStacksTakeRainDamage` - Restores normal rain damage for vanilla wood and bone stacks
- `WorkBenchRequireRoof` - Restores the roof requirement for vanilla and waterproof workbenches

Configuration changes require a full game restart.

## Requirements

- BepInExPack Valheim
- Jotunn

## Installation

### Thunderstore / r2modman

Install using a Thunderstore-compatible mod manager.

### Manual Installation

1. Install BepInExPack Valheim.
2. Install Jotunn.
3. Place `HexWaterproofBuilding.dll` in:

`BepInEx/plugins/HexWaterproofBuilding/`

## Multiplayer

Install the mod on the server and all clients.

Multiplayer and dedicated server testing is limited, so feedback is welcome.

## Notes

- Pier supports automatically generate additional support sections based on water depth
- Removing the mod may leave placed waterproof pieces and pier supports unavailable or non-interactable

## Support and Feedback

Discord: https://discord.gg/wU2FXD94v4

## Source Code

GitHub: https://github.com/guillenjgg/valheim-hex-water-proof-objects