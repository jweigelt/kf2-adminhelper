/* 
 * This file is part of kf2 adminhelper. (https://github.com/jweigelt/kf2-adminhelper)
 * Copyright (C) 2017 Jan Weigelt (info@janelo.net)
 * 
 * kf2 adminhelper is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.

 * kf2 adminhelper is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.

 * You should have received a copy of the GNU General Public License
 * along with kf2 adminhelper.  If not, see <http://www.gnu.org/licenses/>.
 */
using System.Collections.Generic;
namespace KF2Admin.Admin.Commands
{
    public class CmdMap : Command
    {
        public string OnMap { get; set; } = "Loading {0}";
        public string OnMapGameType { get; set; } = "Loading {0}, changing gametype to {1}";

        public string OnMapNotFound { get; set; } = "No map matching '{0}' could be found.";
        public string OnMapNameAmbiguous { get; set; } = "'{0}' Ambiguous. More than {1} maps found.";
        public string OnMapNameAmbiguousHelp { get; set; } = "'{0}' Ambiguous. Found: {1}";

        public string OnModeNotFound { get; set; } = "No mode matching '{0}' could be found.";
        public string OnModeNameAmbiguous { get; set; } = "'{0}' Ambiguous. More than {1} mode found.";
        public string OnModeNameAmbiguousHelp { get; set; } = "'{0}' Ambiguous. Found: {1}";

        public string OnInvalidMode { get; set; } = "Invalid mode. Allowed: survival, versus, weekly";
        public int HelpMaxMaps { get; set; } = 3;
        public int HelpMaxModes { get; set; } = 3;

        public CmdMap()
        {
            CommandAlias = "map";
            CommandName = "map";
            CommandPermission = "map";
            CommandDescrption = "Changes the current map.";
            ParameterSyntax = "!map <map> [<mode>]";
        }

        public override bool CheckSyntax(string[] command, Player player)
        {
            if (command.Length != 2 && command.Length != 3) return false;
            return true;
        }

        public override bool RunCommand(string[] command, Player player)
        {
            string map = string.Empty;
            string gamemode = string.Empty;

            try
            {
                map = StringSearch(Tool.Status.InstalledMaps, command[1], HelpMaxMaps, OnMapNotFound, OnMapNameAmbiguous, OnMapNameAmbiguousHelp);
            }
            catch (StringSearchException e)
            {
                Say(e.Message);
                return false;
            }

            if (command.Length == 3)
            {
                try
                {
                    gamemode = StringSearch(Tool.Status.InstalledGamemodes, command[2], HelpMaxModes, OnModeNotFound, OnModeNameAmbiguous, OnModeNameAmbiguousHelp);
                }
                catch (StringSearchException e)
                {
                    Say(e.Message);
                    return false;
                }

                Say(OnMapGameType, map, gamemode);
                Tool.Web.ChangeCurrent(Web.ChangeCurrentAction.MapGameMode, map, gamemode);
            }
            else
            {
                Say(OnMap, map);
                Tool.Web.ChangeCurrent(Web.ChangeCurrentAction.Map, map);
            }

            return true;
        }
    }
}