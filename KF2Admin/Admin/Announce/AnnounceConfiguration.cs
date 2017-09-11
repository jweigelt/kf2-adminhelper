﻿/* 
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

namespace KF2Admin.Admin.Announce
{
    public class AnnounceConfiguration
    {
        public bool Enable { get; set; } = false;
        public int AnnounceDelay { get; set; } = 60000 * 10;
        public List<Announce> AnnounceList { get; set; } = new List<Announce>();
    }
}
