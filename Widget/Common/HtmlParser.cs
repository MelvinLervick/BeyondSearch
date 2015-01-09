﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebPageWidget.Common
{
    public class HtmlParser : TextParser
    {
        public HtmlParser()
        {
        }

        public HtmlParser(string html)
            : base(html)
        {
        }

        /// <summary>
        /// Parses the next tag that matches the specified tag name
        /// </summary>
        /// <param name="name">Name of the tags to parse ("*" = parse all tags)</param>
        /// <param name="tag">Returns information on the next occurrence of the
        /// specified tag or null if none found</param>
        /// <returns>True if a tag was parsed or false if the end of the document was reached</returns>
        public bool ParseNext(string name, out HtmlTag tag)
        {
            // Must always set out parameter
            tag = null;

            // Nothing to do if no tag specified
            if (String.IsNullOrEmpty(name))
            {
                return false;
            }

            // Loop until match is found or no more tags
            MoveTo('<');
            while (!EndOfText)
            {
                // Skip over opening '<'
                MoveAhead();

                // Examine first tag character
                var c = Peek();
                if (c == '!' && Peek(1) == '-' && Peek(2) == '-')
                {
                    // Skip over comments
                    const string endComment = "-->";
                    MoveTo(endComment);
                    MoveAhead(endComment.Length);
                }
                else if (c == '/')
                {
                    // Skip over closing tags
                    MoveTo('>');
                    MoveAhead();
                }
                else
                {
                    bool inScript;

                    // Parse tag
                    bool result = ParseTag(name, ref tag, out inScript);

                    // Because scripts may contain tag characters, we have special
                    // handling to skip over script contents
                    if (inScript)
                    {
                        MovePastScript();
                    }
                    // Return true if requested tag was found
                    if (result)
                    {
                        return true;
                    }
                }
                // Find next tag
                MoveTo('<');
            }
            // No more matching tags found
            return false;
        }

        /// <summary>
        /// Parses the contents of an HTML tag. The current position should be at the first
        /// character following the tag's opening less-than character.
        /// 
        /// Note: We parse to the end of the tag even if this tag was not requested by the
        /// caller. This ensures subsequent parsing takes place after this tag
        /// </summary>
        /// <param name="reqName">Name of the tag the caller is requesting, or "*" if caller
        /// is requesting all tags</param>
        /// <param name="tag">Returns information on this tag if it's one the caller is
        /// requesting</param>
        /// <param name="inScript">Returns true if tag began, and did not end, and script
        /// block</param>
        /// <returns>True if data is being returned for a tag requested by the caller
        /// or false otherwise</returns>
        protected bool ParseTag(string reqName, ref HtmlTag tag, out bool inScript)
        {
            var doctype = false;
            inScript = false;
            var requested = false;

            // Get name of this tag
            var name = ParseTagName();

            // Special handling
            if (System.String.Compare(name, "!DOCTYPE", System.StringComparison.OrdinalIgnoreCase) == 0)
            {
                doctype = true;
            }
            else
            {
                if (System.String.Compare(name, "script", System.StringComparison.OrdinalIgnoreCase) == 0)
                {
                    inScript = true;
                }
            }

            // Is this a tag requested by caller?
            if (reqName == "*" || System.String.Compare(name, reqName, System.StringComparison.OrdinalIgnoreCase) == 0)
            {
                // Yes
                requested = true;
                // Create new tag object
                tag = new HtmlTag
                {
                    Name = name,
                    Attributes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                };
            }

            // Parse attributes
            MovePastWhitespace();
            while (Peek() != '>' && Peek() != NullChar)
            {
                if (Peek() == '/')
                {
                    // Handle trailing forward slash
                    if (requested)
                    {
                        tag.TrailingSlash = true;
                    }
                    MoveAhead();
                    MovePastWhitespace();
                    // If this is a script tag, it was closed
                    inScript = false;
                }
                else
                {
                    // Parse attribute name
                    name = (!doctype) ? ParseAttributeName() : ParseAttributeValue();
                    MovePastWhitespace();
                    // Parse attribute value
                    var value = String.Empty;
                    if (Peek() == '=')
                    {
                        MoveAhead();
                        MovePastWhitespace();
                        value = ParseAttributeValue();
                        MovePastWhitespace();
                    }
                    // Add attribute to collection if requested tag
                    if (requested)
                    {
                        // This tag replaces existing tags with same name
                        if (tag.Attributes.ContainsKey(name))
                        {
                            tag.Attributes.Remove(name);
                        }
                        tag.Attributes.Add(name, value);
                    }
                }
            }
            // Skip over closing '>'
            MoveAhead();

            return requested;
        }

        /// <summary>
        /// Parses a tag name. The current position should be the first character of the name
        /// </summary>
        /// <returns>Returns the parsed name string</returns>
        protected string ParseTagName()
        {
            var start = Position;
            while (!EndOfText && !Char.IsWhiteSpace(Peek()) && Peek() != '>')
            {
                MoveAhead();
            }
            return Substring(start, Position);
        }

        /// <summary>
        /// Parses an attribute name. The current position should be the first character
        /// of the name
        /// </summary>
        /// <returns>Returns the parsed name string</returns>
        protected string ParseAttributeName()
        {
            var start = Position;
            while (!EndOfText && !Char.IsWhiteSpace(Peek()) && Peek() != '>' && Peek() != '=')
            {
                MoveAhead();
            }
            return Substring(start, Position);
        }

        /// <summary>
        /// Parses an attribute value. The current position should be the first non-whitespace
        /// character following the equal sign.
        /// 
        /// Note: We terminate the name or value if we encounter a new line. This seems to
        /// be the best way of handling errors such as values missing closing quotes, etc.
        /// </summary>
        /// <returns>Returns the parsed value string</returns>
        protected string ParseAttributeValue()
        {
            int start;
            int end;
            var c = Peek();
            if (c == '"' || c == '\'')
            {
                // Move past opening quote
                MoveAhead();
                // Parse quoted value
                start = Position;
                MoveTo(new char[] { c, '\r', '\n' });
                end = Position;
                // Move past closing quote
                if (Peek() == c)
                {
                    MoveAhead();
                }
            }
            else
            {
                // Parse unquoted value
                start = Position;
                while (!EndOfText && !Char.IsWhiteSpace(c) && c != '>')
                {
                    MoveAhead();
                    c = Peek();
                }
                end = Position;
            }
            return Substring(start, end);
        }

        /// <summary>
        /// Locates the end of the current script and moves past the closing tag
        /// </summary>
        protected void MovePastScript()
        {
            const string endScript = "</script";

            while (!EndOfText)
            {
                MoveTo(endScript, true);
                MoveAhead(endScript.Length);
                if (Peek() == '>' || Char.IsWhiteSpace(Peek()))
                {
                    MoveTo('>');
                    MoveAhead();
                    break;
                }
            }
        }
    }
}