# Fast Bikes (Cities: Skylines II)

Fast Bikes speeds up **bicycles** and **electric scooters**.

It applies a speed multiplier and scales acceleration/braking so higher speeds stay usable instead of looking like “teleport + panic braking”.

## Features
- **Bike & scooter speed** multiplier (0.30x to 10.00x)
- **Accel/brake auto-scaled** for smoother high-speed behavior
- Optional stability tuning:
  - **Stiffness** (sway amplitude)
  - **Damping** (how fast sway settles)
- **Path speed limit** (1.00x to 5.00x)
  - Affects: bike paths, pedestrian+bike paths, and pedestrian-only paths (not roads)
- One-click reset buttons:
  - **Game defaults** (reset to vanilla)
  - **Mod defaults** (recommended values)
- Debug:
  - **Bicycle Prefab Report** (writes to `Logs/FastBikes.log`)

> Default install values:
> - Enabled: ON  
> - Speed: 2.0x  
> - Stiffness: 1.50x  
> - Damping: 1.50x  
> - Path speed limit: 2.0x  

## Options

### Enable Fast Bikes
Turns the mod ON/OFF.

- **ON**: applies the selected tuning to bicycle/scooter prefabs and path speed limits.
- **OFF**: restores vanilla values.

### Bike & scooter speed
Scales **max speed** allowed.  
Acceleration and braking are auto adjusted for the selected speed.

- 0.30 = 30% of game default
- 1.00 = game default

Notes:
- Road speed limits and game conditions may still apply.
- For best results, consider installing **Road Speed Adjuster** mod to raise road limits too.

### Stiffness
Scalar for **sway amplitude**.

- Higher = less leaning / tighter look
- Lower = more wobble

Recommended at high speed: **1.25–1.75**  
For more wobble: **< 0.75**

### Damping
How quickly sway settles.

- Higher = oscillation dies faster
- 1.0 = game defaults

Recommended at high speed: **1.25–2.0+**  
For more wobble: **< 0.75**

### Path speed limit
Scales speed limits for **paths** (not roads).

- 1.00 = game default
- Affects bike/ped paths and pedestrian-only paths
- This setting is separate from road speed limit mods (as they change roads only and not paths).

## Languages
- Supports **11 languages**

## Remove mod
- Best way to uninstall: toggle **Enable Fast Bikes** OFF, load the city once, save once, then uninstall the mod.
    - This ensures all values are reset to vanilla defaults (reset sliders to 1.0x also does this).
    - If you never used Path Speed, then you can skip the load/save step and just uninstall after toggling off.
- If you uninstall the mod without resetting Path Speed to 1x, then existing paths keep the modified speed limit and all new paths will be vanilla speed limits.


## Troubleshooting

  - Load a city first.
  - Toggle **Enable Fast Bikes** OFF then ON.
  - Use **Bicycle Prefab Report** and confirm values change in the log.
  - Delete the ModsSettings/FastBikes/FastBikes.coc file > then start the game, a new settings file is created with default values.

- **After a game update**
  - Optinal: run **Bicycle Prefab Report** and compare baseline values.

## Links
- Discord: https://discord.gg/HTav7ARPs2  
- GitHub: https://github.com/River-Mochi/CS2-Fastbikes  

## Credits
**River-mochi** — mod author
