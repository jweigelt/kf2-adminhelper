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
namespace KF2Admin.Utility
{
    class Log
    {
        public const string FILE_PREFIX = "[IO ] ";
        public const string FILE_DIRECTORY_CREATE = FILE_PREFIX + "Created directory '{0}'"; //directory path
        public const string FILE_DIRECTORY_CREATE_ERROR = FILE_PREFIX + " Failed to create directory '{0}' ({1})"; //directory path, error string
        public const string FILE_XML_PARSE = FILE_PREFIX + "Parsed XML-file '{0}', type: {1}"; //file path, class name
        public const string FILE_XML_PARSE_ERROR = FILE_PREFIX + " Error reading XML-file '{0}', type: {1} ({2})"; //file path, class name, error message
        public const string FILE_XML_CREATE = FILE_PREFIX + "Wrote XML-file '{0}', type: {1}"; //file path, class name, error message
        public const string FILE_XML_CREATE_ERROR = FILE_PREFIX + "Error writing XML-file '{0}', type: {1} ({2})"; //file path, class name, error message
        public const string FILE_DEFAULT_UNPACK = FILE_PREFIX + "Unpacked default file '{0}'"; //file path
        public const string FILE_DEFAULT_UNPACK_ERROR = FILE_PREFIX + "Failed to unpack default file '{0}' (1)"; //file path, error message
        public const string FILE_DEFAULT_USE_AUTO = FILE_PREFIX + "Generating default XML-file for class '{0}'"; //config class
        public const string FILE_DEFAULT_USE_AUTO_ERROR = FILE_PREFIX + "XML-generation failed. class: '{0}' ({1})"; //config classm error message
       
    }
}
