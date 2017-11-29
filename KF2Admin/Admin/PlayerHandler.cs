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
using System;
using System.Collections.Generic;
using KF2Admin.Utility;
using KF2Admin.Config;

namespace KF2Admin.Admin
{
    public class PlayerHandler
    {
        private List<Player> playerList = null;
        private List<Player> statsPlayerList = null;
        private List<Player> playerCache = null;

        private AdminTool tool = null;
        private PlayerHandlerConfiguration config = null;
        private int lastWave = 0;

        public bool Open()
        {
            config = tool.FileIO.ReadConfig<PlayerHandlerConfiguration>();
            return true;
        }

        public PlayerHandler(AdminTool tool)
        {
            this.tool = tool;
            playerCache = new List<Player>();
            playerList = new List<Player>();
        }

        public void Update()
        {
            List<Player> newPlayerList = tool.Web.GetPlayerList();
            if (newPlayerList == null) return;

            foreach (Player p in newPlayerList)
            {
                if (!IsOnline(p, playerList) && !IsOnline(p, playerCache)) OnNewPlayerJoin(p);
            }

            foreach (Player p in playerList)
            {
                if (!IsOnline(p, newPlayerList)) OnPlayerLeave(p);
            }

            foreach (Player p in playerCache)
            {
                List<Player> newPlayerCache = new List<Player>();
                if ((DateTime.Now - p.CacheTime).TotalSeconds < config.NewPlayerTimeout)
                {
                    newPlayerCache.Add(p);
                }
                playerCache = newPlayerCache;
            }

            playerList = newPlayerList;
        }

        public void UpdateStatus()
        {
            if (tool.Status.CurrentWave < lastWave)
            {
                Logger.Log(LogLevel.Info, "[PLH] New game started.");

                //TODO: list seems to be empty during map load
                /*foreach (Player stat in statsPlayerList)
                {
                    Player p = null;
                    try
                    {
                        p = GetPlayerByNameMatch(stat.PlayerName, true);       
                    }
                    catch (PlayerSearchException e)
                    {
                        Logger.Log("[PLH] Stats: Not tracking player {0} : {1}", LogLevel.Verbose, stat.PlayerName, e.Message);
                        continue;
                    }
                    Logger.Log("[PLH] Stats: Writing stats for {0}", LogLevel.Verbose, stat.PlayerName);
                    p.CopyStats(stat);
                    tool.Database.TrackStats(p);
                }*/
            }
            else
            {
                List<Player> stats = tool.Web.GetPlayerList(true);
                if (stats != null) statsPlayerList = stats;
            }

            lastWave = tool.Status.CurrentWave;
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

        public bool IsOnline(Player player, List<Player> pl)
        {
            if (pl == null) return false;
            foreach (Player p in pl)
            {
                if (p.UqNetId == player.UqNetId)
                {
                    return true;
                }
            }
            return false;
        }

        private void OnPlayerLeave(Player p)
        {
            Logger.Log(LogLevel.Verbose, "[PLH] Player '{0}' left",  p.PlayerName);
            p.CacheTime = DateTime.Now;
            playerCache.Add(p);
        }

        private void OnNewPlayerJoin(Player p)
        {
            tool.Database.AttachDbInfo(p);

            if (p.IsNew)
            {
                Logger.Log(LogLevel.Verbose, "[PLH] Registering player '{0}', Steam-ID: '{1}'", p.PlayerName, p.SteamId);
                tool.Database.InsertPlayer(p);
            }
            else
            {
                Logger.Log(LogLevel.Verbose, "[PLH] Updating player '{0}', Steam-ID: '{1}'", p.PlayerName, p.SteamId);
                tool.Database.UpdatePlayer(p);
            }

            if (playerList != null)
            {
                if (p.IsNew)
                {
                    if (config.WelcomeNewPlayer) tool.Say(config.OnWelcomeNewPlayer, p.PlayerName, p.DatabaseId.ToString());
                }
                else
                {
                    if (config.WelcomeOldPlayer) tool.Say(config.OnWelcomeOldPlayer, p.PlayerName, p.Visits.ToString());
                }

                Logger.Log(LogLevel.Verbose, "[PLH] New player '{0}' joined.", p.PlayerName);
            }
        }

    }
}