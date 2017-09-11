A simple administration helper for Killing Floor 2 dedicated servers.
- Works using Killing Floor 2's web interface.
- No mods required, no effect on perk progression.
- Remote management possible

Features:
- SQLite based Database backend
- player logging
- group & permission system
- easy to use ingame !-commands
- fully configurable templates

Commands:
- !kick <player> [<reason>] : kicks a player
- !ban  <player> [<reason>] : bans a player' unique ID
- !ipban <player> [<reason>] : bans a player's IP-address
- !sban player> [<reason>] : bans a player from the current session
- !mute player> [<reason>] : mutes a player's voice chat
- !unmute player> [<reason>] : unmutes a player's voice chat
- !map <map> [<mode>] : loads the given map immediately, (and changes gamemode if specified) 
- !difficulty <difficulty> : changes game difficulty
- !gimmeadmin : grants all permissions on the first user to execute this command
- !putgroup <user> <group> : adds a user to a group
- !rmgroup <user> <group> : removes a user from a group

Setup:
- Ensure your KF2-server has webadmin enabled
- Open ./cfg/core.xml
- Adjust WebAdminURL (http://localhost:8080 if the server is using default confuration and runs on the same machine)
- Adjust Username/Password (Username=admin by default)
- Save core.xml
- Run KF2Admin.exe
- join your server, enter !gimmeadmin in chat
- done