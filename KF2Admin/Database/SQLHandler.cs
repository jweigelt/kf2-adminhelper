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
using System.Data;
using System.Data.SQLite;

using System.Data.Common;
using System.Collections.Generic;

using MySql.Data;
using MySql.Data.MySqlClient;

using KF2Admin.Utility;
using KF2Admin.Admin;


namespace KF2Admin.Database
{
    public class SQLHandler
    {
        public enum DbType { SQLite, MySQL }

        public DbType SQLType { get; set; } = DbType.SQLite;
        public string SQLiteFileName { get; set; } = "KF2Admin.sqlite";
        public string MySQLHostName { get; set; } = "localhost:3306";
        public string MySQLDbName { get; set; } = "kf2admin";
        public string MySQLUserName { get; set; } = "root";
        public string MySQLPassword { get; set; } = "";

        private DbConnection connection = null;

        public bool Open()
        {
            if (connection != null)
            {
                Logger.Log(LogLevel.Warning, "[SQL] Database connection already open");
                return true;
            }

            Logger.Log(LogLevel.Verbose, "[SQL] Opening database connection...");

            if (SQLType == DbType.SQLite)
                connection = new SQLiteConnection(string.Format("Data Source={0};", SQLiteFileName));
            else
                connection = new MySqlConnection(string.Format("SERVER={0};DATABASE={1};UID={2};PASSWORD={3}", MySQLHostName, MySQLDbName, MySQLUserName, MySQLPassword));

            try
            {
                connection.Open();
                Logger.Log(LogLevel.Info, "[SQL] Database OK.");
                return true;
            }
            catch (Exception e)
            {
                Logger.Log(LogLevel.Error, "[SQL] Couldn't open database : {0}", e.Message);
                return false;
            }

        }

        public void Close()
        {
            if (connection != null)
            {
                Logger.Log(LogLevel.Info, "[SQL] Closing Database...");
                connection.Close();
                connection = null;
            }
        }

        DbDataReader Query(string query, params string[] parameters)
        {
            Logger.Log(LogLevel.VerboseSQL, "[SQL] Query: {0}", query);
            DbDataReader reader = null;
            try
            {
                if (connection.State != ConnectionState.Open) connection.Open();
                reader = BuildCommand(query, parameters).ExecuteReader();
                return reader;
            }
            catch (Exception e)
            {
                if (reader != null)
                {
                    if (!reader.IsClosed) reader.Close();
                }

                Logger.Log(LogLevel.Error, "[SQL] Query failed : {0}", e.Message);
                return null;
            }
        }

        void NonQuery(string query, params string[] parameters)
        {
            Logger.Log(LogLevel.VerboseSQL, "[SQL] NonQuery: {0}", query);
            try
            {
                if (connection.State != ConnectionState.Open) connection.Open();
                BuildCommand(query, parameters).ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Logger.Log(LogLevel.Error, "[SQL] Query failed : {0}", e.Message);
            }
        }

        private DbCommand BuildCommand(string query, params string[] parameters)
        {
            DbCommand command;
            if (SQLType == DbType.SQLite)
                command = new SQLiteCommand(query, (SQLiteConnection)connection);
            else
                command = new MySqlCommand(query, (MySqlConnection)connection);

            for (int i = 0; i < parameters.Length; i += 2)
            {
                if (parameters.Length < i)
                {
                    Logger.Log(LogLevel.Error, "[SQL] No value for parameter '{0}' specified", parameters[i]);
                }
                else
                {
                    DbParameter p = command.CreateParameter();
                    p.ParameterName = parameters[i];
                    p.Value = parameters[i + 1];
                    command.Parameters.Add(p);
                }
            }
            command.Prepare();
            return command;
        }

        private bool HasRows(DbDataReader reader)
        {
            if (reader == null) return false;
            return reader.HasRows;
        }

        public void AttachDbInfo(Player player)
        {
            string sql =
             "SELECT " +
             "id, player_visits, player_last_visit, player_kills " +
             "FROM " +
             "kf2_players " +
             "WHERE kf2_players.player_steam_id = @steam_id";

            DbDataReader reader = Query(sql, "@steam_id", player.SteamId);
            if (!HasRows(reader))
            {
                player.IsNew = true;
            }
            else
            {
                reader.Read();
                player.DatabaseId = (long)reader["id"];
                player.Visits = (long)reader["player_visits"];
                player.LastVisit = (long)reader["player_last_visit"];
                player.Kills = (long)reader["player_kills"];
            }
        }

        public bool PlayerExists(Player player)
        {
            string sql =
                "SELECT " +
                "kf2_players.id " +
                "FROM " +
                "kf2_players " +
                "WHERE kf2_players.player_steam_id = @steam_id";

            return (HasRows(Query(sql, "@steam_id", player.SteamId)));
        }

        public bool HasPermission(Player player, string permission)
        {
            string sql =
                "SELECT " +
                "kf2_players.id " +
                "FROM " +
                "kf2_group_permissions ," +
                "kf2_players " +
                "INNER JOIN kf2_player_groups ON kf2_group_permissions.group_id = kf2_player_groups.group_id AND kf2_player_groups.player_id = kf2_players.id " +
                "WHERE kf2_players.player_steam_id = @steam_id AND kf2_group_permissions.permission_name = @permission";

            return (HasRows(Query(sql, "@steam_id", player.SteamId, "@permission", permission)));
        }

        public void InsertPlayer(Player player)
        {
            string sql =
                "INSERT INTO kf2_players " +
                "(player_name, player_steam_id, player_unique_net_id, player_ip_address, player_last_visit, player_visits, player_kills) VALUES " +
                "(@name, @steam_id, @unique_net_id, @ip_address, @timestamp, 1, 0); " +
                "SELECT last_insert_rowid();";

            DbDataReader reader = Query(sql, "@name", player.PlayerName, "@steam_id", player.SteamId, "@unique_net_id", player.UqNetId, "@ip_address", player.IpAddress.ToString(), "@timestamp", GetTimestamp().ToString());
            if (reader != null)
            {
                reader.Read();
                player.DatabaseId = (long)reader[0];
                reader.Close();
            }
        }

        public void UpdatePlayer(Player player)
        {
            string sql =
                "UPDATE kf2_players SET " +
                "player_name = @name, " +
                "player_ip_address = @ip_address, " +
                "player_last_visit = @timestamp, " +
                 "player_visits = (player_visits + 1) " +
                "WHERE player_steam_id = @steam_id";

            NonQuery(sql, "@name", player.PlayerName, "@ip_address", player.IpAddress.ToString(), "@timestamp", GetTimestamp().ToString(), "@steam_id", player.SteamId);
        }

        public List<PlayerGroup> GetPlayerGroups(Player player)
        {
            List<PlayerGroup> groups = new List<PlayerGroup>();
            string sql =
                "SELECT " +
                "kf2_groups.group_name, " +
                "kf2_groups.id " +
                "FROM " +
                "kf2_players " +
                "INNER JOIN kf2_player_groups ON kf2_player_groups.player_id = kf2_players.id " +
                "INNER JOIN kf2_groups ON kf2_player_groups.group_id = kf2_groups.id " +
                "WHERE kf2_players.player_steam_id = @steam_id";

            DbDataReader reader = Query(sql, "@steam_id", player.SteamId);
            if (reader != null)
            {
                while (reader.Read())
                {
                    groups.Add(new PlayerGroup((long)reader["id"], (string)reader["group_name"]));
                }
                reader.Close();
            }

            return groups;
        }

        public List<PlayerGroup> GetAllPlayerGroups()
        {
            List<PlayerGroup> groups = new List<PlayerGroup>();
            string sql =
                "SELECT " +
                "id, " +
                "group_name " +
                "FROM " +
                "kf2_groups";

            DbDataReader reader = Query(sql);
            if (reader != null)
            {
                while (reader.Read())
                {
                    groups.Add(new PlayerGroup((long)reader["id"], (string)reader["group_name"]));
                }
                reader.Close();
            }

            return groups;
        }

        public long GetPlayerId(Player player)
        {
            long playerId = -1;
            string sql =
               "SELECT " +
               "id " +
               "FROM " +
               "kf2_players " +
               "WHERE kf2_players.player_steam_id = @steam_id";

            DbDataReader reader = Query(sql, "@steam_id", player.SteamId);
            if (reader != null)
            {
                reader.Read();
                playerId = (long)reader["id"];
                reader.Close();
            }
            return playerId;
        }

        public void SetPlayerGroup(Player player, PlayerGroup group, bool delete = false)
        {
            string sql = null;
            long playerId = GetPlayerId(player);

            if (delete)
            {
                sql =
                    "DELETE FROM kf2_player_groups " +
                    "WHERE group_id = @group_id AND player_id = @player_id";
            }
            else
            {
                sql =
                    "INSERT INTO kf2_player_groups " +
                    "(group_id, player_id) VALUES " +
                    "(@group_id, @player_id)";
            }


            NonQuery(sql, "@group_id", group.Id.ToString(), "@player_id", playerId.ToString());
        }

        public void TrackStats(Player player)
        {
            string sql =
              "UPDATE kf2_players SET " +
              "player_kills = (player_kills + @kills) " +
              "WHERE player_steam_id = @steam_id";

            NonQuery(sql, "@kills", player.Kills.ToString(), "@steam_id", player.SteamId);
        }

        public bool GroupListEmpty()
        {
            string sql =
                "SELECT " +
                "id " +
                "FROM " +
                "kf2_player_groups";

            return !HasRows(Query(sql));
        }

        private int GetTimestamp()
        {
            return (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }

    }
}