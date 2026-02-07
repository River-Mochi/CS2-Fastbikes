# Fast Bikes (Cities: Skylines II)

Fast Bikes speeds up **bicycles** and **electric scooters** in Cities: Skylines II.

It applies a configurable speed multiplier and automatically scales acceleration/braking so higher speeds stay usable instead of looking like “teleport + panic braking”.

## Features
- **Bike & scooter speed** multiplier (0.30x to 10.00x)
- **Accel/brake auto-scaled** for stability at higher speeds
- Optional Stability tuning:
  - **Stiffness** (sway amplitude)
  - **Damping** (how fast sway settles)
- One-click:
  - **Game defaults** (reset to vanilla values)
  - **Mod defaults** (reset to recommended values)
- Debug:
  - **Bicycle Prefab Dump** (logs detailed prefab values)

> Default install values:
> - Enabled: ON  
> - Speed: 2x, Stiffness: 1.25x, Damping: 1.25x  

### Enable Fast Bikes
Turns the mod ON/OFF.

- **ON**: applies the selected tuning to bicycle/scooter prefabs.
- **OFF**: restores vanilla values.

### Bike & scooter speed
Scales **top speed**.
Acceleration and braking are also adjusted for the selected speed.

- 0.30 = 30% of game default
- 1.00 = game default

Note: 
- Road speed limits and game conditions may still apply.
- Recommend installing **Road Speed Adjuster** mod to double road speeds and see even better Bike results.

### Stiffness
Scalar for **sway amplitude**.

- Higher = less leaning / tighter look
- Lower = more wobble

Recommended at high speed: **1.25–1.75**

### Damping
How quickly sway settles.

- Higher = oscillation dies faster
- 1.0 = game defaults
- Recommended setting at high speed: **1.25–2.0+**

## Compatibility

- Edits bicycle/scooter prefab data.
- If another mod edits the same values, whichever mod applies last will win.

## Troubleshooting

- **Nothing changes after adjusting sliders**
  - Load a city first.
  - Toggle **Enable Fast Bikes** OFF then ON.
  - Use **Bicycle Prefab Dump** and confirm values change in the log.

- **After a game update**
  - Run **Bicycle Prefab Dump** and compare baseline values.
  - Report issues with logs.

## Links

- Discord: https://discord.gg/HTav7ARPs2
- GitHub: https://github.com/River-Mochi/CS2-Fastbikes
