using System;
namespace KF2Admin.Utility
{
    class StringUtil
    {
        public static string TagReplace(string s, params string[] tags)
        {
            for (int i = 0; i < tags.Length; i += 2)
            {
                if (tags.Length < i)
                {
                    Logger.Log(LogLevel.Error, "[UTI] No value for parameter '{0}' specified", tags[i]);
                }
                else
                {
                    s = s.Replace(tags[i], tags[i + 1]);
                }
            }
            return s;
        }

        public static string DateTagReplace(string s)
        {
            DateTime t = DateTime.Now;
            s = TagReplace(s,
                "{%D}",t.Day.ToString(),
                "{%W}", t.DayOfWeek.ToString(),
                "{%h}",t.Hour.ToString(),
                "{%z}", t.Kind.ToString(),
                "{%f}", t.Millisecond.ToString(),
                "{%i}", t.Minute.ToString(),
                "{%m}", t.Month.ToString(),
                "{%s}", t.Second.ToString(),
                "{%Y}", t.Year.ToString());

            return s;
        }

    }
}
