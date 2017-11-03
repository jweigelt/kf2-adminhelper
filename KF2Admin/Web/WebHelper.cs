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
using System.Net;
using System.Collections.Generic;
using KF2Admin.Utility;
using KF2Admin.Admin;

namespace KF2Admin.Web
{
    public enum PlayerAction
    {
        Kick = 0,
        BanSession = 1,
        BanIPAddress = 2,
        BanUqId = 3,
        MuteVoice = 4,
        UnmuteVoice = 5
    }

    public enum ChangeCurrentAction
    {
        Map = 0,
        GameMode = 1,
        MapGameMode = 2,
    }

    public class WebHelper
    {
        const string LOGIN_POSTDATA = "token={0}&password_hash={1}&username={2}&password={3}&remember={4}";
        const string LOGIN_DURATION_INF = "-1";

        const string LOCATION_LOGIN = "/ServerAdmin/";
        const string LOCATION_LOGOUT = "/ServerAdmin/logout";
        const string LOCATION_DASHBOARD = "/ServerAdmin/current/info";
        const string LOCATION_PLAYERS = "/ServerAdmin/current/players";
        const string LOCATION_CHAT = "/ServerAdmin/current/chat+frame+data";
        const string LOCATION_GAME_STATUS = "/ServerAdmin/current+gamesummary";
        const string LOCATION_PLAYERS_ACTION = "/ServerAdmin/current/players+data";
        const string LOCATION_SAY = "/ServerAdmin/current/chat+frame";
        const string LOCATION_MAP_LIST = "/ServerAdmin/settings/maplist";

        const string LOCATION_MAP_CHANGE = "/ServerAdmin/current/change";
        const string LOCATION_MODE_LIST = "/ServerAdmin/current/change";
        const string LOCATION_SETTINGS_GENERAL = "/ServerAdmin/settings/general";

        const string EXP_NOPLAYERS = "There are no players";

        public string WebAdminUrl { get; set; } = "http://blinky.janelo.net:8080";
        public string UserAgent { get; set; } = "KF2Admin/1.0";
        public string UserName { get; set; } = "admin";
        public string Password { get; set; } = "m0ngo";

        private static readonly string[] PLAYER_ACTIONS = { "kick", "sessionban", "banip", "banid", "mutevoice", "unmutevoice" };

        private CookieContainer cookieContainer = null;

        public WebHelper()
        {
            ResetSession();
        }
        public void ResetSession()
        {
            cookieContainer = new CookieContainer();
        }

        public bool Authenticate()
        {
            WebRequest initialRequest = null;
            WebRequest loginRequest = null;
            HtmlElement tokenField = null;
            string token = null;
            string authString = null;

            Logger.Log("[WEB] Logging in...", LogLevel.Verbose);

            try
            {
                Logger.Log("[WEB] Requesting session id and login token...", LogLevel.Verbose);
                initialRequest = new WebRequest(WebAdminUrl + LOCATION_LOGIN, "GET", UserAgent, cookieContainer);
                initialRequest.PerformRequest();

                tokenField = HtmlElement.FindElementByAttributeMatch(initialRequest.ResponseContentData, "input", true, "name", "token");
                token = tokenField.GetAttributeValue("value");
                Logger.Log("[WEB] Received our token for this login : '{0}'", LogLevel.Verbose, token);

                authString = string.Format(LOGIN_POSTDATA, token, string.Empty, UserName, Password, LOGIN_DURATION_INF);
                loginRequest = new WebRequest(WebAdminUrl + LOCATION_LOGIN, "POST", UserAgent, cookieContainer, authString, "application/x-www-form-urlencoded");
                loginRequest.PerformRequest();

                if (loginRequest.ResponseUri.LocalPath != LOCATION_DASHBOARD)
                {
                    throw new Exception("Wrong username/password");
                }
            }
            catch (Exception e)
            {
                Logger.Log("[WEB] Authentication failed : {0}", LogLevel.Error, e.Message);
                return false;
            }

            Logger.Log("[WEB] Login OK.", LogLevel.Info);
            return true;
        }

        public bool Logout()
        {
            Logger.Log("[WEB] Logging out...", LogLevel.Info);
            WebRequest logoutRequest = new WebRequest(WebAdminUrl + LOCATION_LOGOUT, "GET", UserAgent, cookieContainer);
            try
            {
                logoutRequest.PerformRequest();
            }
            catch (Exception e)
            {
                Logger.Log("[WEB] Logout failed : {0}", LogLevel.Error, e.Message);
                return false;
            }
            Logger.Log("[WEB] Logout OK.", LogLevel.Info);
            return true;
        }

        public List<Player> GetPlayerList(bool stats = false)
        {
            WebRequest playerRequest = null;
            HtmlElement playerTable = null;
            List<HtmlElement> tableBody = null;
            List<HtmlElement> playerRows = null;
            List<Player> playerList = new List<Player>();

            try
            {
                playerRequest = new WebRequest(WebAdminUrl + (stats ? LOCATION_DASHBOARD : LOCATION_PLAYERS), "GET", UserAgent, cookieContainer);
                playerRequest.PerformRequest();

                playerTable = HtmlElement.FindElementByAttributeMatch(playerRequest.ResponseContentData, "table", false, "id", "players");
                tableBody = HtmlElement.FindElementsByTagMatch(playerTable.InnerHTML, "tbody", false);
                if (tableBody.Count == 0) throw new ParseException("Can't find playertable tbody");
                playerRows = HtmlElement.FindElementsByTagMatch(tableBody[0].InnerHTML, "tr", false);
                if (playerRows.Count == 0) throw new ParseException("Playerlist not found");
            }
            catch (Exception e)
            {
                Logger.Log("[WEB] Playerlist retrieval failed : {0}", LogLevel.Error, e.Message);
                return null;
            }

            foreach (HtmlElement tr in playerRows)
            {
                if (tr.InnerHTML.Contains(EXP_NOPLAYERS))
                {
                    return playerList;
                }
                else
                {
                    try
                    {
                        playerList.Add(GetPlayer(tr, stats));
                    }
                    catch (Exception e)
                    {
                        Logger.Log("[WEB] Error parsing player: {0}", LogLevel.Warning, e.Message);
                    }
                }
            }
            return playerList;
        }


        private Player GetPlayer(HtmlElement tr, bool stats = false)
        {
            List<HtmlElement> cells = HtmlElement.FindElementsByTagMatch(tr.InnerHTML, "td", false);
            Player player = null;

            if (stats)
            {
                // string perk, int dosh, int health, long kills, ushort ping, bool isAdmin
                player = new Player(
                    cells[1].InnerHTML, //Name
                    cells[2].InnerHTML, //Perk
                    int.Parse(cells[3].InnerHTML),    //Dosh
                    (cells[4].InnerHTML.Length > 0 ? int.Parse(cells[4].InnerHTML) : 0),    //Health
                    long.Parse(cells[5].InnerHTML),    //Kills
                    ushort.Parse(cells[6].InnerHTML), //Ping
                    (cells[5].InnerHTML == "Yes"));   //Admin
            }
            else
            {
                HtmlElement playerId = HtmlElement.FindElementByAttributeMatch(cells[9].InnerHTML, "input", true, "name", "playerid");
                HtmlElement playerkey = HtmlElement.FindElementByAttributeMatch(cells[9].InnerHTML, "input", true, "name", "playerkey");

                player = new Player(
                    cells[1].InnerHTML, //Name
                    cells[4].InnerHTML, //Unique Net Id
                    cells[5].InnerHTML, //Steam Id
                    playerkey.GetAttributeValue("value"),    //Player Key
                    IPAddress.Parse(cells[3].InnerHTML),     //IP Address
                    ushort.Parse(cells[2].InnerHTML),        //Ping
                    uint.Parse(playerId.GetAttributeValue("value")),  //Player Id
                    (cells[7].InnerHTML == "Yes"),  //Admin
                    (cells[8].InnerHTML == "Yes")); //Spectator
            }

            return player;
        }

        public ChatMessage GetChat()
        {
            WebRequest chatRequest = new WebRequest(WebAdminUrl + LOCATION_CHAT, "POST", UserAgent, cookieContainer, "ajax=1", "application/x-www-form-urlencoded");
            try
            {
                chatRequest.PerformRequest();

                if (chatRequest.ResponseContentData.Length > 0)
                {

                    string playerName = HtmlElement.FindElementByAttributeMatch(chatRequest.ResponseContentData, "span", false, "class", "username ").InnerHTML;
                    string message = HtmlElement.FindElementByAttributeMatch(chatRequest.ResponseContentData, "span", false, "class", "message").InnerHTML;

                    Logger.Log("[WEB] #{0}: {1}", LogLevel.Info, playerName, message);
                    return new ChatMessage(playerName, message);
                }

            }
            catch (Exception e)
            {
                Logger.Log("Failed to retrieve chat update : {0}", LogLevel.Warning, e.Message);
            }

            return null;
        }

        public bool PlayerAction(Player player, PlayerAction action)
        {
            string postData = string.Format("ajax=1&action={0}&playerkey={1}", PLAYER_ACTIONS[(int)action], player.PlayerKey);
            WebRequest actionRequest = new WebRequest(WebAdminUrl + LOCATION_PLAYERS_ACTION, "POST", UserAgent, cookieContainer, postData, "application/x-www-form-urlencoded");
            try
            {
                actionRequest.PerformRequest();
                //TODO: evtl. Antwort validieren
                return true;
            }
            catch (Exception e)
            {
                Logger.Log("Failed to perform action on player '{0}' : {1}", LogLevel.Warning, player.PlayerName, e.Message);
                return false;
            }
        }

        public bool Say(string message)
        {
            Logger.Log("[WEB] Saying '{0}'", LogLevel.Verbose, message);
            try
            {
                WebRequest sayRequest = new WebRequest(WebAdminUrl + LOCATION_SAY, "POST", UserAgent, cookieContainer, "message=" + message, "application/x-www-form-urlencoded");
                sayRequest.PerformRequest();
                return true;
            }
            catch (Exception e)
            {
                Logger.Log("Failed to say '{0}' : {1}", LogLevel.Warning, message, e.Message);
                return false;
            }
        }

        public List<string> GetInstalledMaps()
        {
            WebRequest mapRequest = new WebRequest(WebAdminUrl + LOCATION_MAP_LIST, "GET", UserAgent, cookieContainer);
            try
            {
                mapRequest.PerformRequest();
                HtmlElement mapContainer = HtmlElement.FindElementByAttributeMatch(mapRequest.ResponseContentData, "textarea", false, "id", "allmaps");
                string[] ms = mapContainer.InnerHTML.Split('\n');
                return new List<string>(ms);
            }
            catch (Exception e)
            {
                Logger.Log("Failed to retrieve map list {0}", LogLevel.Warning, e.Message);
                return null;
            }
        }

        public List<string> GetInstalledGamemodes()
        {
            List<string> modes = new List<string>();

            WebRequest modeRequest = new WebRequest(WebAdminUrl + LOCATION_MODE_LIST, "GET", UserAgent, cookieContainer);
            try
            {
                modeRequest.PerformRequest();
                HtmlElement mapContainer = HtmlElement.FindElementByAttributeMatch(modeRequest.ResponseContentData, "select", false, "id", "gametype");
                List<HtmlElement> options = HtmlElement.FindElementsByTagMatch(mapContainer.InnerHTML, "option", false);

                foreach (HtmlElement e in options)
                {
                    modes.Add(e.GetAttributeValue("value"));
                }

            }
            catch (Exception e)
            {
                Logger.Log("Failed to retrieve gamemode list {0}", LogLevel.Warning, e.Message);
                return null;
            }
            return modes;
        }

        public bool ChangeCurrent(ChangeCurrentAction action, string s1, string s2 = "")
        {
            string postData = "action=change";

            switch (action)
            {
                case ChangeCurrentAction.Map:
                    postData += "&map=" + s1;
                    break;
                case ChangeCurrentAction.GameMode:
                    postData += "&gametype=" + s1;
                    break;
                case ChangeCurrentAction.MapGameMode:
                    postData += "&map=" + s1 + "&gametype=" + s2;
                    break;
            }
            WebRequest mapRequest = new WebRequest(WebAdminUrl + LOCATION_MAP_CHANGE, "POST", UserAgent, cookieContainer, postData, "application/x-www-form-urlencoded");
            try
            {
                mapRequest.PerformRequest();
                return true;
            }
            catch (Exception e)
            {
                Logger.Log("Failed to change map {0}", LogLevel.Warning, e.Message);
                return false;
            }
        }

        public bool ChangeGeneral(params string[] postParams)
        {
            string postData = "action=save&liveAdjust=1";
            for (int i = 0; i < postParams.Length; i += 2)
            {
                if (postParams.Length <= (i + 1))
                {
                    Logger.Log("Failed to change settings. Invalid post params.", LogLevel.Warning);
                    return false; ;
                }
                postData += "&";
                postData += postParams[i] + "=" + postParams[i + 1];
            }

            WebRequest generalRequest = new WebRequest(WebAdminUrl + LOCATION_SETTINGS_GENERAL, "POST", UserAgent, cookieContainer, postData, "application/x-www-form-urlencoded");
            try
            {
                generalRequest.PerformRequest();

                return true;
            }
            catch (Exception e)
            {
                Logger.Log("Failed to change settings {0}", LogLevel.Warning, e.Message);
                return false;
            }
        }

        public bool ChangeDifficulty(Difficulty d)
        {
            return ChangeGeneral("settings_GameDifficulty", d.Value, "settings_GameDifficulty_raw", d.Value);
        }

        public bool UpdateServerStatus(ServerStatus status)
        {
            WebRequest statusRequest = new WebRequest(WebAdminUrl + LOCATION_GAME_STATUS, "POST", UserAgent, cookieContainer, "ajax=1", "application/x-www-form-urlencoded");
            try
            {
                statusRequest.PerformRequest();
                HtmlElement map = HtmlElement.FindElementsByTagMatch(statusRequest.ResponseContentData, "img", true)[0];
                HtmlElement wave = HtmlElement.FindElementByAttributeMatch(statusRequest.ResponseContentData, "dt", false, "class", "gs_wave");

                status.CurrentMap = map.GetAttributeValue("alt");
                status.CurrentWave = int.Parse(wave.InnerHTML.Split(' ')[1]);

            }
            catch (Exception e)
            {
                Logger.Log("Failed to retrieve game status update : {0}", LogLevel.Warning, e.Message);
                return false;
            }
            return true;
        }

    }
}