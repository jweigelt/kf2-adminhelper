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
namespace KF2Admin.Admin.Commands
{
    public class PlayerCommand : Command
    {
        public string OnNoPlayer { get; set; } = "Can't find player : {0}";
        public string OnAction { get; set; } = "{0} was affected by {1}";
        public string OnActionReason { get; set; } = "{0} was affected by {1} for {2}";

        public override bool CheckSyntax(string[] command, Player player)
        {
            if (command.Length < 2) return false;
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

            if (command.Length > 2)
            {
                Say(OnActionReason, affectedPlayer.PlayerName, player.PlayerName, string.Join(" ", command, 2, command.Length - 2));
            }
            else
            {
                Say(OnAction, affectedPlayer.PlayerName, player.PlayerName);
            }

            AffectPlayer(player, affectedPlayer);

            return true;
        }
        public virtual void AffectPlayer(Player admin, Player player)
        {
        }
    }
}