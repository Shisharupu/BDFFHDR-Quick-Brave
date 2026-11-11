# QuickBrave

**RT / F = Brave x3** for the character currently selecting a command in
*Bravely Default: Flying Fairy HD Remaster*.

One tap queues three Brave presses (one per frame — the game consumes one
input per frame), giving the turn character 4 actions. It calls the game's
own Brave handler, so all normal limits apply: it stops early if the game
rejects a Brave (e.g. BP floor reached, character can't act).

## Controls

| Input | Effect |
|-------|--------|
| Gamepad Right Trigger | Brave x3 (turn character) |
| Keyboard F | Brave x3 (turn character) |

Inputs only act during the battle command-selection phase. Outside of battle
(or in sub-menus) they do nothing.

## Install

1. Install BepInEx 6 (Unity IL2CPP x64) into the game directory (next to
   `BDFFHD.exe`) and launch the game once so it generates interop assemblies.
2. Copy `QuickBrave.dll` into `BepInEx\plugins\QuickBrave\`.
3. Launch the game. (Code mods disable online play.)

## Build

```bash
dotnet build
```

Edit `<GameDir>` in `QuickBrave.csproj` if your Steam library is elsewhere.
The DLL is output straight into `BepInEx\plugins\QuickBrave\`.

## Notes

- v1.0.0 — release. Tap-only; no Default shortcut, no party-wide mode.
- Developed against the IL2CPP dump preserved in the toolkit repo
  (`BDFFHD-Modding/_GameDump/`); see `BDFFHD-Modding/docs/brave-logic.md`
  for the reverse-engineering notes.
