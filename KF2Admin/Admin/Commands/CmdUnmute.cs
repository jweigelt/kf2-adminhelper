﻿/* 
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
using System.Xml.Serialization;
namespace KF2Admin.Admin.Commands
{
    public class CmdUnmute : PlayerCommand
    {
        [XmlIgnore]
        public const string FILE_NAME = "./cfg/cmd/unmute.xml";

        [XmlIgnore]
        public const string RESOURCE_NAME = "KF2Admin.Resources.cfg.cmd.unmute.xml";

        public CmdUnmute()
        {
            CommandAlias = "unmute";
            CommandName = "unmute";
            CommandPermission = "unmute";
            CommandDescrption = "Unutes a player's voip-chat";
            ParameterSyntax = "!unmute <player> [<reason>]";

            OnAction = "{0}'s VOIP was unmuted by {1}";
            OnActionReason = "{0}'s VOIP was unmuted by {1} for {2}";
            OnNoPlayer = "Can't unmute : {0}";
        }

        public override bool CheckSyntax(string[] command, Player player)
        {
            if (command.Length < 2) return false;
            return true;
        }

        public override void AffectPlayer(Player admin, Player player)
        {
            Tool.Web.PlayerAction(player, Web.PlayerAction.UnmuteVoice);
        }
    }
}