# Fast Bikes (Cities: Skylines II)

Fast Bikes speeds up **bicycles** and **electric scooters**.  
It can also raise **path speed limits** so bikes don’t get bottlenecked on paths.

## Features
- **Bike & scooter speed** multiplier (0.30x to 10.00x)
- **Accel/brake auto-scaled** for smoother high-speed behavior
- **Path speed limit** (1.00x to 5.00x)
  - Affects: bike paths, pedestrian+bike paths, and pedestrian-only paths (**not roads**)
- One-click reset buttons:
  - **Game defaults** (vanilla)
  - **Mod defaults** (recommended)
- **Status panel** (runs only while Options is open)
  - Bike group counts (bikes + e-scooters)
  - Car group counts (personal cars; bikes excluded)
  - “Total at OC border” count for hidden parked cars at Outside Connection
- Logs / debug:
  - **Log hidden cars** → writes A/B/C bucket breakdown + sample entity IDs to `Logs/FastBikes.log`
  - **Bike debug report** → detailed report to `Logs/FastBikes.log` (good for patch-day sanity checks)

> Compatibility note: the optional “sway/stability tuning” UI has been removed for now to avoid conflicts with mods that also modify stiffness/damping.

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
- If faster bikes on normal roads are needed too, use a road speed limit mod (this mod handles **paths**, not roads).

### Path speed limit
Scales speed limits for **paths** (not roads).

- **1.00 = vanilla**
- Affects bike/ped paths and pedestrian-only paths
- Separate from road speed limit mods (they change roads only)

---

## Status: “Hidden parked cars” + “Log hidden cars”
Some parked vehicles are **Unspawned** (hidden) because they’re parked inside buildings/garages — normal.

Some cities also show lots of cars **hidden at the border (OC / Outside Connection)**.  
In the Status panel, the row shows **Total at OC border**.

If you want to help investigate:
1. Open **Options → Fast Bikes → Status**
2. Click **Log hidden cars**
3. Open `Logs/FastBikes.log`
4. The log prints:
   - **Total OC Hidden Cars**
   - Bucket **A/B/C** breakdown
   - Sample **VehicleIndex:Version** IDs
5. Use **Scene Explorer** mod → “Jump To” the listed vehicle entity IDs
   - Look at `ParkedCar.m_Lane` and owner/citizen panels

---

## Performance notes
Fast Bikes is built to be lightweight.

- Applies tuning when settings change (or when a city loads), then goes idle.
- Status report refreshes only when the Options menu is open.
- No constant per-frame city work for the tuning features.

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

---

## Links
- Discord: https://discord.gg/HTav7ARPs2  
- GitHub: https://github.com/River-Mochi/CS2-Fastbikes  

## Credits
**River-mochi** — mod author
