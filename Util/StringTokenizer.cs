using System;
using System.Collections;

namespace Fonet.Util
{
    internal class StringTokenizer : IEnumerator
    {
        private int _currentPosition;
        private readonly string _delimiters;

        /// <summary>
        ///     maxDelimChar stores the value of the delimiter character with
        ///     the highest value. It is used to optimize the detection of
        ///     delimiter characters.
        /// </summary>
        private char _maxDelimChar;

        private readonly int _maxPosition;
        private int _newPosition;
        private readonly bool _retDelims;
        private readonly string _str;

        /// <summary>
        ///     Constructs a string tokenizer for the specified string. All
        ///     characters in the <i>delim</i> argument are the delimiters
        ///     for separating tokens.<br />
        ///     If the <i>returnDelims</i> flag is <i>true</i>, then
        ///     the delimiter characters are also returned as tokens. Each delimiter
        ///     is returned as a string of length one. If the flag is
        ///     <i>false</i>, the delimiter characters are skipped and only
        ///     serve as separators between tokens.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="delim"></param>
        /// <param name="returnDelims"></param>
        public StringTokenizer( string str, string delim, bool returnDelims )
        {
            _currentPosition = 0;
            _newPosition = -1;
            this._str = str;
            _maxPosition = str.Length;
            _delimiters = delim;
            _retDelims = returnDelims;
            SetMaxDelimChar();
        }

        /// <summary>
        ///     Constructs a string tokenizer for the specified string. The
        ///     characters in the <code>delim</code> argument are the delimiters
        ///     for separating tokens. Delimiter characters themselves will not
        ///     be treated as tokens.
        /// </summary>
        /// <param name="str">a string to be parsed.</param>
        /// <param name="delim">the delimiters.</param>
        public StringTokenizer( string str, string delim ) : this( str, delim, false )
        {
        }

        /// <summary>
        ///     Constructs a string tokenizer for the specified string. The
        ///     tokenizer uses the default delimiter set, which is the space
        ///     character, the tab character, the newline character, the
        ///     carriage-return character, and the form-feed character.
        ///     Delimiter characters themselves will not be treated as tokens.
        /// </summary>
        /// <param name="str">a string to be parsed</param>
        public StringTokenizer( string str ) : this( str, " \t\n\r\f", false )
        {
        }

        /// <summary>
        ///     Returns the same value as the <code>hasMoreTokens</code> method.
        ///     It exists so that this class can implement the
        ///     <i>Enumeration</i> interface.
        /// </summary>
        /// <returns>
        ///     <i>true</i> if there are more tokens; <i>false</i>
        ///     otherwise.
        /// </returns>
        public bool MoveNext()
        {
            /*
             * Temporary store this position and use it in the following
             * nextToken() method only if the delimiters have'nt been changed in
             * that nextToken() invocation.
             */
            _newPosition = SkipDelimiters( _currentPosition );
            return _newPosition < _maxPosition;
        }

        /// <summary>
        ///     Returns the same value as the <code>nextToken</code> method, except
        ///     that its declared return value is <code>Object</code> rather than
        ///     <code>String</code>. It exists so that this class can implement the
        ///     <code>Enumeration</code> interface.
        /// </summary>
        public object Current
        {
            get { return NextToken(); }
        }

        public void Reset()
        {
        }

        /// <summary>
        ///     Set maxDelimChar to the highest char in the delimiter set.
        /// </summary>
        private void SetMaxDelimChar()
        {
            if ( _delimiters == null )
            {
                _maxDelimChar = (char)0;
                return;
            }

            var m = (char)0;
            for ( var i = 0; i < _delimiters.Length; i++ )
            {
                char c = _delimiters[ i ];
                if ( m < c )
                    m = c;
            }
            _maxDelimChar = m;
        }

        /// <summary>
        ///     Skips delimiters starting from the specified position. If
        ///     retDelims is false, returns the index of the first non-delimiter
        ///     character at or after startPos. If retDelims is true, startPos
        ///     is returned.
        /// </summary>
        /// <param name="startPos"></param>
        /// <returns></returns>
        private int SkipDelimiters( int startPos )
        {
            if ( _delimiters == null )
                throw new NullReferenceException();

            int position = startPos;
            while ( !_retDelims && position < _maxPosition )
            {
                char c = _str[ position ];
                if ( c > _maxDelimChar || _delimiters.IndexOf( c ) < 0 )
                    break;
                position++;
            }
            return position;
        }

        /// <summary>
        ///     Skips ahead from startPos and returns the index of the next
        ///     delimiter character encountered, or maxPosition if no such
        ///     delimiter is found.
        /// </summary>
        /// <param name="startPos"></param>
        /// <returns></returns>
        private int ScanToken( int startPos )
        {
            int position = startPos;
            while ( position < _maxPosition )
            {
                char c = _str[ position ];
                if ( c <= _maxDelimChar && _delimiters.IndexOf( c ) >= 0 )
                    break;
                position++;
            }
            if ( _retDelims && startPos == position )
            {
                char c = _str[ position ];
                if ( c <= _maxDelimChar && _delimiters.IndexOf( c ) >= 0 )
                    position++;
            }
            return position;
        }

        /// <summary>
        ///     Returns the next token from this string tokenizer.
        /// </summary>
        /// <returns>the next token from this string tokenizer.</returns>
        public virtual string NextToken()
        {
            /*
             * If next position already computed in hasMoreElements() and
             * delimiters have changed between the computation and this invocation,
             * then use the computed value.
             */
            _currentPosition = _newPosition >= 0
                ? _newPosition
                : SkipDelimiters( _currentPosition );

            _newPosition = -1;

            if ( _currentPosition >= _maxPosition )
                throw new InvalidOperationException();
            int start = _currentPosition;
            _currentPosition = ScanToken( _currentPosition );
            return _str.Substring( start, _currentPosition - start );
        }

        /// <summary>
        ///     Tests if there are more tokens available from this tokenizer's
        ///     string.  If this method returns <tt>true</tt>, then a subsequent
        ///     call to <tt>nextToken</tt> with no argument will successfully
        ///     return a token.
        /// </summary>
        /// <returns>
        ///     <code>true</code> if and only if there is at least one token in
        ///     the string after the current position; <code>false</code> otherwise.
        /// </returns>
        public bool HasMoreTokens()
        {
            /*
             * Temporary store this position and use it in the following
             * NextToken() method only if the delimiters have'nt been changed in
             * that NextToken() invocation.
             */
            _newPosition = SkipDelimiters( _currentPosition );
            return _newPosition < _maxPosition;
        }

        /// <summary>
        ///     Calculates the number of times that this tokenizer's
        ///     <code>nextToken</code> method can be called before it generates an
        ///     exception. The current position is not advanced.
        /// </summary>
        /// <returns>
        ///     the number of tokens remaining in the string using the current
        ///     delimiter set.
        /// </returns>
        public virtual int CountTokens()
        {
            var count = 0;
            int currpos = _currentPosition;
            while ( currpos < _maxPosition )
            {
                currpos = SkipDelimiters( currpos );
                if ( currpos >= _maxPosition )
                    break;
                currpos = ScanToken( currpos );
                count++;
            }
            return count;
        }
    }
}