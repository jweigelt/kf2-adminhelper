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
using KF2Admin.Utility;
namespace KF2Admin.Config
{
    public class CoreConfiguration
    {
        public string WebAdminURL { get; set; } = "http://localhost:8080";
        public string WebAdminUserName { get; set; } = "admin";
        public string WebAdminPassword { get; set; } = "1234";

        public int TickDelay { get; set; } = 5;
        public int PlayerListUpdateDelay { get; set; } = 5000;
        public int ChatUpdateDelay { get; set; } = 1500;
        public int GameUpdateDelay { get; set; } = 15000;

        public string SQLiteFile { get; set; } = "KF2Admin.sqlite";

        public string CommandKey { get; set; } = "!";

        public LogLevel LogMinimumLevel { get; set; } = LogLevel.Info;
        public bool LogToFile { get; set; } = true;
        public string LogFile { get; set; } = "log.txt";

    }
}