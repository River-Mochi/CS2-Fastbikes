# Fast Bikes (Cities: Skylines II)

Fast Bikes speeds up **bicycles** and **electric scooters**.  
It can also raise **path speed limits** so bikes don’t get bottlenecked on paths.

## Features
- **Bike & scooter speed** multiplier (0.30x to 10.00x)
- **Accel/brake auto-scaled** for smoother high-speed behavior
- Optional stability tuning:
  - **Stiffness** (sway amplitude)
  - **Damping** (how fast sway settles)
- **Path speed limit** (1.00x to 5.00x)
  - Affects: bike paths, pedestrian+bike paths, and pedestrian-only paths (**not roads**)
- One-click reset buttons:
  - **Game defaults** (vanilla)
  - **Mod defaults** (recommended)
- **Status panel** (runs only while Options is open)
  - Bike group counts (bikes + e-scooters)
  - Car group counts (personal cars; bikes excluded)
  - Hidden parking breakdown (buildings vs border OC)
- Debug / logs:
  - **Bike debug report** → `Logs/FastBikes.log`
  - Includes **Scooter01 (fuel)** count + sample entity IDs (useful for bug reports)

---

## Options

### Enable Fast Bikes
Turns the mod ON/OFF.

- **ON**: applies tuning to bike/scooter prefabs and path speed limits.
- **OFF**: restores vanilla values.

### Bike & scooter speed
Scales **max speed** allowed. Acceleration/braking auto-adjust for the selected speed.

- **0.30 = 30%** of game default  
- **1.00 = game default**

Notes:
- Road speed limits and game conditions may still cap speeds.
- If you want faster bikes on roads too, consider a road speed limit mod (this mod handles **paths**, not roads).

### Stiffness
Scalar for **sway amplitude**.

- Higher = less leaning / tighter look
- Lower = more wobble

Good at high speed: **1.25–1.75**  
More wobble: **< 0.75**

### Damping
How quickly sway settles.

- Higher = oscillation dies faster
- **1.0 = vanilla**

Good at high speed: **1.25–2.0+**  
More wobble: **< 0.75**

### Path speed limit
Scales speed limits for **paths** (not roads).

- **1.00 = vanilla**
- Affects bike/ped paths and pedestrian-only paths
- Separate from road speed limit mods (they change roads only)

---

## Status: “Hidden parking” + “Log hidden cars”
Some parked vehicles are **Unspawned** (hidden) because they’re parked inside buildings/garages — normal.

Some cities also show lots of cars **hidden at the border (OC / Outside Connection)**.  
These show up as parked on an outside-connection lane (Connection Lane / OutsideConnection).

If you want to help investigate:
1. Open **Options → Fast Bikes → Status**
2. Click **Log hidden cars**
3. Open `Logs/FastBikes.log`
4. Use **Scene Explorer** mod → “Jump To” the listed **VehicleIndex:Version**
   - Look at `ParkedCar.m_Lane` (Connection Lane is the border/OC case)

---

## Languages
- Supports **11 languages**

---

## Remove mod (clean uninstall)
Best uninstall method:
1. Set **Enable Fast Bikes = OFF**
2. Make sure **Path speed limit = 1.00**
3. Load the city once, save once
4. Uninstall the mod

If you uninstall without resetting Path Speed to 1.00:
- existing paths can keep the modified speed limits (new paths will be vanilla)

---

## Troubleshooting
- Load a city first (don’t test from main menu).
- Toggle **Enable Fast Bikes** OFF then ON.
- Run **Bike debug report** and check `Logs/FastBikes.log`.

If settings get corrupted:
- Delete `ModsSettings/FastBikes/FastBikes.coc`
- Start the game (a fresh settings file will be recreated)

After a game update:
- Optional: run **Bike debug report** again and compare behavior.

---

## Links
- Discord: https://discord.gg/HTav7ARPs2  
- GitHub: https://github.com/River-Mochi/CS2-Fastbikes  

## Credits
**River-mochi** — mod author
