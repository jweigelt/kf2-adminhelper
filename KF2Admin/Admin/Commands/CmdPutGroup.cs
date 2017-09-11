/* 
 * This file is part of kf2 adminhelper.
 * 
 * SWBF2 SADS-Administation Helper is free software: you can redistribute it and/or modify
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
using System.Collections.Generic;
namespace KF2Admin.Admin.Commands
{
    public class CmdPutGroup : Command
    {
        public string OnNoPlayer { get; set; } = "Can't set group : {0}";
        public string OnNoGroup { get; set; } = "Group '{0}' doesn't exist.";
        public string OnAlreadyDone { get; set; } = "'{0}' is already a member of '{1}'";
        public string OnModifyGroup { get; set; } = "'{0}' added to group '{1}'";

        public CmdPutGroup()
        {
            CommandAlias = "putgroup";
            CommandName = "putgroup";
            CommandPermission = "putgroup";
            CommandDescrption = "Adds a player to a servergroup";
            ParameterSyntax = "!putgroup <player> <group>";
        }

        public override bool CheckSyntax(string[] command, Player player)
        {
            if (command.Length != 3) return false;
            return true;
        }

        public override bool RunCommand(string[] command, Player player)
        {
            Player affectedPlayer = null;
            try
            {
                affectedPlayer = Tool.Players.GetPlayerByNameMatch(command[1]);
            }
            catch (PlayerSearchException e)
            {
                Say(OnNoPlayer, e.Message);
                return false;
            }

            List<PlayerGroup> playerGroups = Tool.Database.GetAllPlayerGroups();

            foreach (PlayerGroup pg in playerGroups)
            {
                if (pg.Name.ToLower().Equals(command[2].ToLower()))
                {          
                    return ModifyGroup(pg, player);
                }
            }

            Say(OnNoGroup, command[2]);
            return false;
        }

        public virtual bool ModifyGroup(PlayerGroup group, Player player)
        {
            if (IsAlreadyMember(group, player))
            {
                Say(OnAlreadyDone, player.PlayerName, group.Name);
                return false;
            }
            Say(OnModifyGroup, player.PlayerName, group.Name);
            Tool.Database.SetPlayerGroup(player, group);
            return true;
        }


        public bool IsAlreadyMember(PlayerGroup group, Player player)
        {
            List<PlayerGroup> groups = Tool.Database.GetPlayerGroups(player);
            foreach (PlayerGroup pg in groups)
            {
                if (pg.Id == group.Id) return true;
            }
            return false;
        }

    }
}