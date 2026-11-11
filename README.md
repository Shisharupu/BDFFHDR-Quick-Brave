# QuickBrave
**Only for Bravely Default: Flying Fairy HD Remaster on Steam**

Braves three times for the character currently selecting a command. Uses RT on controller and F on keyboard.

Made using https://github.com/Corgan/BDFFHD-Modding/tree/main

# IMPORTANT NOTE
This is a **code mod**, meaning you cannot use online play with this mod installed. Ignore this warning at your own risk!

## Install

1. Install BepInEx 6 (Unity IL2CPP x64) into the game directory (next to `BDFFHD.exe`) and launch the game once so it generates interop assemblies.
2. Disable online mode in settings.
3. Copy `QuickBrave.dll` into `BepInEx\plugins\QuickBrave\`.
4. Brave a lot without breaking your fingers. 

## Build

```bash
dotnet build
```

Edit `<GameDir>` in `QuickBrave.csproj` if your game install is not at `C:\Program Files (x86)\Steam\steamapps\common\BDFFHD`

The DLL is automatically put into the game install location under `BepInEx\plugins\QuickBrave\`.
