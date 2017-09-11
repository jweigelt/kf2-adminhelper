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
namespace KF2Admin.Web
{
    public class HtmlElement
    {
        public string InnerHTML { get; }
        public string Header { get; }

        public HtmlElement(string header, string innerHTML = "")
        {
            Header = header;
            InnerHTML = innerHTML;
        }

        public string GetAttributeValue(string attr)
        {
            int expp = Header.IndexOf(attr);
            if (expp < 0) throw new ParseException("Can't find attribute '{0}'", attr);

            int vs = Header.IndexOf("\"", expp);
            if (expp < 0) throw new ParseException("Missing opening \" on attribute  '{0}'", attr);
            vs++;

            int ve = Header.IndexOf("\"", vs);
            if (expp < 0) throw new ParseException("Missing closing \" on attribute  '{0}'", attr);

            return Header.Substring(vs, ve - vs);
        }

        public static HtmlElement FindElementByAttributeMatch(string src, string tag, bool inline, string attr, string val)
        {
            //NOTE: schließende " werden nicht mitgesucht um mehrere class-Zuweisungen zu unterstützen
            int expp = src.IndexOf(attr + "=\"" + val);
            if (expp < 0) throw new ParseException("No element matching '<{0} {1}=\"{2}\" ...' could be found.", tag, attr, val);

            int hs = src.LastIndexOf("<" + tag, expp);
            int he = src.IndexOf(">", expp);
            if (hs < 0 || he < 0) throw new ParseException("Declaration of '{0}' is missing closing/opening <>", tag);
            he++;

            string header = src.Substring(hs, he - hs);
            string content = string.Empty;

            if (!inline)
            {
                int ce = src.IndexOf("</" + tag + ">", he);
                if (ce < 0 || ce < he) throw new ParseException("Declaration of '<{0}>' is missing closing tag", tag);
                content = src.Substring(he, ce - he);
            }

            return new HtmlElement(header, content);
        }

        public static List<HtmlElement> FindElementsByTagMatch(string src, string tag, bool inline)
        {
            List<HtmlElement> elements = new List<HtmlElement>();

            int hs = 0;

            while ((hs = src.IndexOf("<" + tag, hs)) > 0)
            {
                int he = src.IndexOf(">", hs);
                if (he < 0) throw new ParseException("Declaration of '{0}' is missing closing >", tag);
                he++;

                string header = src.Substring(hs, he - hs);
                string content = string.Empty;

                if (!inline)
                {
                    string et = "</" + tag + ">";
                    int ce = src.IndexOf(et, he);
                    if (ce < 0 || ce < he) throw new ParseException("Declaration of '<{0}>' is missing closing tag", tag);
                    content = src.Substring(he, ce - he);
                    hs = ce + et.Length;
                }
                else
                {
                    hs = he;
                }
                elements.Add(new HtmlElement(header, content));
            }
            return elements;
        }
    }
}