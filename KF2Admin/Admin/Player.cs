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
using System;
using System.Net;
namespace KF2Admin.Admin
{
    public class Player
    {
        public Player(string playerName, string uqNetId, string steamId, string playerKey, IPAddress ipAddress, ushort ping, uint playerId, bool isAdmin, bool isSpectator)
        {
            PlayerName = playerName;
            UqNetId = uqNetId;
            SteamId = steamId;
            PlayerKey = playerKey;
            IpAddress = ipAddress;
            Ping = ping;
            PlayerId = playerId;
            IsAdmin = isAdmin;
            IsSpectator = isSpectator;
        }
        public Player(string playerName, string perk, int dosh, int health, long kills, ushort ping, bool isAdmin)
        {
            PlayerName = playerName;
            Perk = perk;
            Dosh = dosh;
            Health = health;
            Kills = kills;
            Ping = ping;
            IsAdmin = isAdmin;
        }

        public string PlayerName { get; set; } = string.Empty;
        public string UqNetId { get; set; } = string.Empty;
        public string SteamId { get; set; } = string.Empty;
        public string PlayerKey { get; set; } = string.Empty;
        public IPAddress IpAddress { get; set; } = null;

        public ushort Ping { get; set; } = 0;
        public uint PlayerId { get; set; } = 0;

        public bool IsAdmin { get; set; } = false;
        public bool IsSpectator { get; set; } = false;

        public bool IsNew { get; set; } = false;
        public long DatabaseId { get; set; } = 0;
        public long Visits { get; set; } = 0;
        public long LastVisit { get; set; }

        public long Kills { get; set; } = 0;
        public int Health { get; set; } = 0;
        public int Dosh { get; set; } = 0;
        public string Perk { get; set; } = string.Empty;
         
        public void CopyStats(Player p)
        {
            Kills = p.Kills;
            Health = p.Health;
            Perk = p.Perk;
            Dosh = p.Dosh;
        }

    }
}