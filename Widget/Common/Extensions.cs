using System;

namespace WebPageWidget.Common
{
    public static class Extensions
    {
        public static string ToLowerAlphabetical(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return string.Empty;
            }

            return str.ToLower().ToAlphabetical();
        }

        public static string ToAlphabetical(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return string.Empty;
            }

            var strTokens = str.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            Array.Sort(strTokens);

            return string.Join(" ", strTokens).Trim();
        }

        public static string ReplaceIgnoreCase(this string source, string stringToBeReplaced, string replaceWith)
        {
            var count = 0;
            var position0 = 0;
            var position1 = 0;

            var upperString = source.ToUpper();
            var upperPattern = stringToBeReplaced.ToUpper();
            var inc = (source.Length / stringToBeReplaced.Length) *
                      (replaceWith.Length - stringToBeReplaced.Length);
            var chars = new char[source.Length + Math.Max(0, inc)];

            while ((position1 = upperString.IndexOf(upperPattern, position0, StringComparison.Ordinal)) != -1)
            {
                for (var i = position0; i < position1; ++i)
                {
                    chars[count++] = source[i];
                }

                foreach (var t in replaceWith)
                {
                    chars[count++] = t;
                }

                position0 = position1 + stringToBeReplaced.Length;
            }
            if (position0 == 0) return source;

            for (var i = position0; i < source.Length; ++i)
            {
                chars[count++] = source[i];
            }

            return new string(chars, 0, count);
        }
    }
}
