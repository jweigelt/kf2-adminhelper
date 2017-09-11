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
using System.Collections.Generic;
using KF2Admin.Utility;
namespace KF2Admin.Admin
{
    public class PlayerHandler
    {
        private List<Player> playerList = null;
        private AdminTool tool = null;

        public PlayerHandler(AdminTool tool)
        {
            this.tool = tool;
        }

        public void Update()
        {
            List<Player> newPlayerList = tool.Web.GetPlayerList();
            foreach (Player p in newPlayerList)
            {
                if (!IsOnline(p)) OnNewPlayerJoin(p);
            }

            playerList = newPlayerList;
        }

        public Player GetPlayerByNameMatch(string expression, bool exact = false)
        {
            if (playerList == null) throw new PlayerSearchException("Playerlist not available.");
            List<Player> matchingPlayers = new List<Player>();

            foreach (Player p in playerList)
            {
                if (exact)
                {
                    if (p.PlayerName.Equals(expression)) matchingPlayers.Add(p);
                }
                else
                {
                    if (p.PlayerName.Contains(expression)) matchingPlayers.Add(p);
                }
            }

            if (matchingPlayers.Count == 0) throw new PlayerSearchException("No player matching '{0}' could be found.", expression);
            else if (matchingPlayers.Count > 1) throw new PlayerSearchException("Multiple players matching '{0}' found.", expression);
            return matchingPlayers[0];
        }

        public bool IsOnline(Player player)
        {
            if (playerList == null) return false;
            foreach (Player p in playerList)
            {
                if (p.UqNetId == player.UqNetId)
                {
                    return true;
                }
            }
            return false;
        }

        private void OnNewPlayerJoin(Player p)
        {

            if (!tool.Database.PlayerExists(p))
            {
                Logger.Log("[PLH] Registering player '{0}', Steam-ID: '{1}'", LogLevel.Verbose, p.PlayerName, p.SteamId);
                tool.Database.InsertPlayer(p);
            }
            else
            {
                Logger.Log("[PLH] Updating player '{0}', Steam-ID: '{1}'", LogLevel.Verbose, p.PlayerName, p.SteamId);
                tool.Database.UpdatePlayer(p);
            }

            if (playerList != null)
            {
                Logger.Log("[PLH] New player '{0}' joined.", LogLevel.Verbose, p.PlayerName);
            }
        }

    }
}