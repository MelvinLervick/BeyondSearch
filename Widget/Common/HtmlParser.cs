using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebPageWidget.Common
{
    public class HtmlParser
    {
        protected string Html;
        protected int Pos;
        protected bool ScriptBegin;

        public HtmlParser(string html)
        {
            Reset(html);
        }

        /// <summary>
        /// Resets the current position to the start of the current document
        /// </summary>
        public void Reset()
        {
            Pos = 0;
        }

        /// <summary>
        /// Sets the current document and resets the current position to the
        /// start of it
        /// </summary>
        /// <param name="html"></param>
        public void Reset(string html)
        {
            Html = html;
            Pos = 0;
        }

        /// <summary>
        /// Indicates if the current position is at the end of the current
        /// document
        /// </summary>
        public bool Eof
        {
            get { return (Pos >= Html.Length); }
        }

        /// <summary>
        /// Parses the next tag that matches the specified tag name
        /// </summary>
        /// <param name="name">Name of the tags to parse ("*" = parse all
        /// tags)</param>
        /// <param name="tag">Returns information on the next occurrence
        /// of the specified tag or null if none found</param>
        /// <returns>True if a tag was parsed or false if the end of the
        /// document was reached</returns>
        public bool ParseNext(string name, out HtmlTag tag)
        {
            tag = null;

            // Nothing to do if no tag specified
            if (String.IsNullOrEmpty(name))
                return false;

            // Loop until match is found or there are no more tags
            while (MoveToNextTag())
            {
                // Skip opening '<'
                Move();

                // Examine first tag character
                var c = Peek();
                if (c == '!' && Peek(1) == '-' && Peek(2) == '-')
                {
                    // Skip over comments
                    const string endComment = "-->";
                    Pos = Html.IndexOf(endComment, Pos, System.StringComparison.Ordinal);
                    NormalizePosition();
                    Move(endComment.Length);
                }
                else if (c == '/')
                {
                    // Skip over closing tags
                    Pos = Html.IndexOf('>', Pos);
                    NormalizePosition();
                    Move();
                }
                else
                {
                    // Parse tag
                    bool result = ParseTag(name, ref tag);

                    // Because scripts may contain tag characters,
                    // we need special handling to skip over
                    // script contents
                    if (ScriptBegin)
                    {
                        const string endScript = "</script";
                        Pos = Html.IndexOf(endScript, Pos,
                          StringComparison.OrdinalIgnoreCase);
                        NormalizePosition();
                        Move(endScript.Length);
                        SkipWhitespace();
                        if (Peek() == '>')
                            Move();
                    }

                    // Return true if requested tag was found
                    if (result)
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Parses the contents of an HTML tag. The current position should
        /// be at the first character following the tag's opening less-than
        /// character.
        /// 
        /// Note: We parse to the end of the tag even if this tag was not
        /// requested by the caller. This ensures subsequent parsing takes
        /// place after this tag
        /// </summary>
        /// <param name="name">Name of the tag the caller is requesting,
        /// or "*" if caller is requesting all tags</param>
        /// <param name="tag">Returns information on this tag if it's one
        /// the caller is requesting</param>
        /// <returns>True if data is being returned for a tag requested by
        /// the caller or false otherwise</returns>

        protected bool ParseTag(string name, ref HtmlTag tag)
        {
            // Get name of this tag
            var s = ParseTagName();

            // Special handling
            var doctype = false;
            var ScriptBegin = false;

            if (System.String.Compare(s, "!DOCTYPE", System.StringComparison.OrdinalIgnoreCase) == 0)
            {
                doctype = true;
            }
            else {
                if (System.String.Compare(s, "script", System.StringComparison.OrdinalIgnoreCase) == 0)
                {
                    ScriptBegin = true;
                }
            }

            // Is this a tag requested by caller?
            var requested = false;
            if (name == "*" || System.String.Compare(s, name, System.StringComparison.OrdinalIgnoreCase) == 0)
            {
                // Yes, create new tag object
                tag = new HtmlTag();
                tag.Name = s;
                tag.Attributes = new Dictionary<string, string>();
                requested = true;
            }

            // Parse attributes
            SkipWhitespace();
            while (Peek() != '>')
            {
                if (Peek() == '/')
                {
                    // Handle trailing forward slash
                    if (requested)
                        tag.TrailingSlash = true;
                    Move();
                    SkipWhitespace();
                    // If this is a script tag, it was closed
                    ScriptBegin = false;
                }
                else
                {
                    // Parse attribute name
                    s = (!doctype) ? ParseAttributeName() : ParseAttributeValue();
                    SkipWhitespace();
                    // Parse attribute value
                    var value = String.Empty;
                    if (Peek() == '=')
                    {
                        Move();
                        SkipWhitespace();
                        value = ParseAttributeValue();
                        SkipWhitespace();
                    }
                    // Add attribute to collection if requested tag
                    if (requested)
                    {
                        // This tag replaces existing tags with same name
                        if (tag.Attributes.Keys.Contains(s))
                            tag.Attributes.Remove(s);
                        tag.Attributes.Add(s, value);
                    }
                }
            }
            // Skip over closing '>'
            Move();

            return requested;
        }

        /// <summary>
        /// Parses a tag name. The current position should be the first
        /// character of the name
        /// </summary>
        /// <returns>Returns the parsed name string</returns>
        protected string ParseTagName()
        {
            var start = Pos;
            while (!Eof && !Char.IsWhiteSpace(Peek()) && Peek() != '>')
                Move();
            return Html.Substring(start, Pos - start);
        }

        /// <summary>
        /// Parses an attribute name. The current position should be the
        /// first character of the name
        /// </summary>
        /// <returns>Returns the parsed name string</returns>
        protected string ParseAttributeName()
        {
            var start = Pos;
            while (!Eof && !Char.IsWhiteSpace(Peek()) && Peek() != '>'
              && Peek() != '=')
                Move();
            return Html.Substring(start, Pos - start);
        }

        /// <summary>
        /// Parses an attribute value. The current position should be the
        /// first non-whitespace character following the equal sign.
        /// 
        /// Note: We terminate the name or value if we encounter a new line.
        /// This seems to be the best way of handling errors such as values
        /// missing closing quotes, etc.
        /// </summary>
        /// <returns>Returns the parsed value string</returns>
        protected string ParseAttributeValue()
        {
            int start, end;
            var c = Peek();
            if (c == '"' || c == '\'')
            {
                // Move past opening quote
                Move();
                // Parse quoted value
                start = Pos;
                Pos = Html.IndexOfAny(new char[] { c, '\r', '\n' }, start);
                NormalizePosition();
                end = Pos;
                // Move past closing quote
                if (Peek() == c)
                    Move();
            }
            else
            {
                // Parse unquoted value
                start = Pos;
                while (!Eof && !Char.IsWhiteSpace(c) && c != '>')
                {
                    Move();
                    c = Peek();
                }
                end = Pos;
            }
            return Html.Substring(start, end - start);
        }

        /// <summary>
        /// Moves to the start of the next tag
        /// </summary>
        /// <returns>True if another tag was found, false otherwise</returns>

        protected bool MoveToNextTag()
        {
            Pos = Html.IndexOf('<', Pos);
            NormalizePosition();
            return !Eof;
        }

        /// <summary>
        /// Returns the character at the current position, or a null
        /// character if we're at the end of the document
        /// </summary>
        /// <returns>The character at the current position</returns>
        public char Peek()
        {
            return Peek(0);
        }

        /// <summary>
        /// Returns the character at the specified number of characters
        /// beyond the current position, or a null character if the
        /// specified position is at the end of the document
        /// </summary>
        /// <param name="ahead">The number of characters beyond the
        /// current position</param>
        /// <returns>The character at the specified position</returns>
        public char Peek(int ahead)
        {
            var pos = (Pos + ahead);
            if (pos < Html.Length)
                return Html[pos];
            return (char)0;
        }

        /// <summary>
        /// Moves the current position ahead one character
        /// </summary>
        protected void Move()
        {
            Move(1);
        }

        /// <summary>
        /// Moves the current position ahead the specified number of characters
        /// </summary>
        /// <param name="ahead">The number of characters to move ahead</param>
        protected void Move(int ahead)
        {
            Pos = Math.Min(Pos + ahead, Html.Length);
        }

        /// <summary>
        /// Moves the current position to the next character that is
        // not whitespace
        /// </summary>
        protected void SkipWhitespace()
        {
            while (!Eof && Char.IsWhiteSpace(Peek()))
                Move();
        }

        /// <summary>
        /// Normalizes the current position. This is primarily for handling
        /// conditions where IndexOf(), etc. return negative values when
        /// the item being sought was not found
        /// </summary>
        protected void NormalizePosition()
        {
            if (Pos < 0)
                Pos = Html.Length;
        }
    }
}