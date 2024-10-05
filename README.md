# publicized upon request, most are unchanged

# PlayerReplace
Originally ["DCReplace Port"](https://github.com/DatPanDat/DCReplace) attempt that I've decided to try expend a bit.

# Features
- Replaces a player with a random spectator if they get disconnected. With everything from inventories to status effects kept the same.
- If they're a 079, the replaced player will gets the correct EXP, AP, and the Room they were in.
- On the case of no replacement, their items and ragdoll will spawn with customizable reasons, and a broadcast to the server.
- Changing the broadcast message, time, and roles to ignore in the config.
- Abilities to replace external role, currently support [UserNonExist/Spies-SL](https://github.com/UserNonExist/Spies-SL).

Code quite literally taken from ["Cyanox62/DCReplace"](https://github.com/Cyanox62/DCReplace) two years old build and is quite literally my first project in C#, feel free to publicly shame me.

Special thanks to @UserNonExist for suffering through my stupidity and helping me get this to work.

# Installation

**[EXILED](https://github.com/Exiled-Team/EXILED) must be installed for this to work.**

Place the "DCReplace.dll" file in your Plugins folder.
Set `disconnect_drop: false` in your gameplay config if you don't want stuffs to get wacky.

