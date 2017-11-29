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
using KF2Admin.Database;
using System.Xml.Serialization;
namespace KF2Admin.Admin.Commands
{
    public class CmdRmGroup : CmdPutGroup
    {
        [XmlIgnore]
        public new const string FILE_NAME = "./cfg/cmd/rmgroup.xml";

        [XmlIgnore]
        public new const string RESOURCE_NAME = "KF2Admin.Resources.cfg.cmd.rmgroup.xml";

        public CmdRmGroup()
        {
            CommandAlias = "rmgroup";
            CommandName = "rmgroup";
            CommandPermission = "rmgroup";
            CommandDescrption = "Removes a player from a servergroup";
            ParameterSyntax = "!rmgroup <player> <group>";
            OnModifyGroup = "Removed '{0}' from '{1}'";
            OnAlreadyDone = "'{0}' is not a member of '{1}'";
        }

 

        public override bool ModifyGroup(PlayerGroup group, Player player)
        {
            if (!IsAlreadyMember(group, player))
            {
                Say(OnAlreadyDone, player.PlayerName, group.Name);
                return false;
            }
            Say(OnModifyGroup, player.PlayerName, group.Name);
            Tool.Database.SetPlayerGroup(player, group, true);
            return true;
        }

    }
}