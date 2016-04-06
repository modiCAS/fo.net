namespace Fonet.Fo.Expr
{
    internal class PropertyTokenizer
    {
        protected const int TokEof = 0;
        protected const int TokNcname = TokEof + 1;
        protected const int TokMultiply = TokNcname + 1;
        protected const int TokLpar = TokMultiply + 1;
        protected const int TokRpar = TokLpar + 1;
        protected const int TokLiteral = TokRpar + 1;
        protected const int TokNumber = TokLiteral + 1;
        protected const int TokFunctionLpar = TokNumber + 1;
        protected const int TokPlus = TokFunctionLpar + 1;
        protected const int TokMinus = TokPlus + 1;
        protected const int TokMod = TokMinus + 1;
        protected const int TokDiv = TokMod + 1;
        protected const int TokNumeric = TokDiv + 1;
        protected const int TokComma = TokNumeric + 1;
        protected const int TokPercent = TokComma + 1;
        protected const int TokColorspec = TokPercent + 1;
        protected const int TokFloat = TokColorspec + 1;
        protected const int TokInteger = TokFloat + 1;

        private const string NameStartChars = "_abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string NameChars = ".-0123456789";
        private const string Digits = "0123456789";
        private const string Hexchars = Digits + "abcdefABCDEF";

        protected int CurrentToken = TokEof;

        private int _currentTokenStartIndex;
        protected string CurrentTokenValue;
        protected int CurrentUnitLength;
        private readonly string _expr;
        private int _exprIndex;
        private readonly int _exprLength;
        private bool _recognizeOperator;

        protected PropertyTokenizer( string s )
        {
            _expr = s;
            _exprLength = s.Length;
        }

        protected void Next()
        {
            CurrentTokenValue = null;
            _currentTokenStartIndex = _exprIndex;
            bool currentMaybeOperator = _recognizeOperator;
            bool bSawDecimal;
            _recognizeOperator = true;
            for ( ;; )
            {
                if ( _exprIndex >= _exprLength )
                {
                    CurrentToken = TokEof;
                    return;
                }
                char c = _expr[ _exprIndex++ ];
                switch ( c )
                {
                case ' ':
                case '\t':
                case '\r':
                case '\n':
                    _currentTokenStartIndex = _exprIndex;
                    break;
                case ',':
                    _recognizeOperator = false;
                    CurrentToken = TokComma;
                    return;
                case '+':
                    _recognizeOperator = false;
                    CurrentToken = TokPlus;
                    return;
                case '-':
                    _recognizeOperator = false;
                    CurrentToken = TokMinus;
                    return;
                case '(':
                    CurrentToken = TokLpar;
                    _recognizeOperator = false;
                    return;
                case ')':
                    CurrentToken = TokRpar;
                    return;
                case '"':
                case '\'':
                    _exprIndex = _expr.IndexOf( c, _exprIndex );
                    if ( _exprIndex < 0 )
                    {
                        _exprIndex = _currentTokenStartIndex + 1;
                        throw new PropertyException( "missing quote" );
                    }
                    CurrentTokenValue = _expr.Substring(
                        _currentTokenStartIndex + 1,
                        _exprIndex++ - ( _currentTokenStartIndex + 1 ) );
                    CurrentToken = TokLiteral;
                    return;
                case '*':
                    CurrentToken = TokMultiply;
                    return;
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    ScanDigits();
                    if ( _exprIndex < _exprLength && _expr[ _exprIndex ] == '.' )
                    {
                        _exprIndex++;
                        bSawDecimal = true;
                        if ( _exprIndex < _exprLength
                            && IsDigit( _expr[ _exprIndex ] ) )
                        {
                            _exprIndex++;
                            ScanDigits();
                        }
                    }
                    else
                        bSawDecimal = false;
                    if ( _exprIndex < _exprLength && _expr[ _exprIndex ] == '%' )
                    {
                        _exprIndex++;
                        CurrentToken = TokPercent;
                    }
                    else
                    {
                        CurrentUnitLength = _exprIndex;
                        ScanName();
                        CurrentUnitLength = _exprIndex - CurrentUnitLength;
                        CurrentToken = CurrentUnitLength > 0
                            ? TokNumeric
                            : ( bSawDecimal ? TokFloat : TokInteger );
                    }
                    CurrentTokenValue = _expr.Substring( _currentTokenStartIndex,
                        _exprIndex - _currentTokenStartIndex );
                    return;

                case '.':
                    if ( _exprIndex < _exprLength
                        && IsDigit( _expr[ _exprIndex ] ) )
                    {
                        ++_exprIndex;
                        ScanDigits();
                        if ( _exprIndex < _exprLength
                            && _expr[ _exprIndex ] == '%' )
                        {
                            _exprIndex++;
                            CurrentToken = TokPercent;
                        }
                        else
                        {
                            CurrentUnitLength = _exprIndex;
                            ScanName();
                            CurrentUnitLength = _exprIndex - CurrentUnitLength;
                            CurrentToken = CurrentUnitLength > 0
                                ? TokNumeric
                                : TokFloat;
                        }
                        CurrentTokenValue = _expr.Substring( _currentTokenStartIndex,
                            _exprIndex - _currentTokenStartIndex );
                        return;
                    }
                    throw new PropertyException( "illegal character '.'" );

                case '#':
                    if ( _exprIndex < _exprLength && IsHexDigit( _expr[ _exprIndex ] ) )
                    {
                        ++_exprIndex;
                        ScanHexDigits();
                        CurrentToken = TokColorspec;
                        CurrentTokenValue = _expr.Substring( _currentTokenStartIndex,
                            _exprIndex - _currentTokenStartIndex );
                        return;
                    }
                    throw new PropertyException( "illegal character '#'" );

                default:
                    --_exprIndex;
                    ScanName();
                    if ( _exprIndex == _currentTokenStartIndex )
                        throw new PropertyException( "illegal character" );
                    CurrentTokenValue = _expr.Substring(
                        _currentTokenStartIndex, _exprIndex - _currentTokenStartIndex );
                    if ( CurrentTokenValue.Equals( "mod" ) )
                    {
                        CurrentToken = TokMod;
                        return;
                    }
                    if ( CurrentTokenValue.Equals( "div" ) )
                    {
                        CurrentToken = TokDiv;
                        return;
                    }
                    if ( FollowingParen() )
                    {
                        CurrentToken = TokFunctionLpar;
                        _recognizeOperator = false;
                    }
                    else
                    {
                        CurrentToken = TokNcname;
                        _recognizeOperator = false;
                    }
                    return;
                }
            }
        }

        private void ScanName()
        {
            if ( _exprIndex < _exprLength && IsNameStartChar( _expr[ _exprIndex ] ) )
            {
                while ( ++_exprIndex < _exprLength && IsNameChar( _expr[ _exprIndex ] ) )
                    ;
            }
        }

        private void ScanDigits()
        {
            while ( _exprIndex < _exprLength && IsDigit( _expr[ _exprIndex ] ) )
                _exprIndex++;
        }

        private void ScanHexDigits()
        {
            while ( _exprIndex < _exprLength && IsHexDigit( _expr[ _exprIndex ] ) )
                _exprIndex++;
        }

        private bool FollowingParen()
        {
            for ( int i = _exprIndex; i < _exprLength; i++ )
            {
                switch ( _expr[ i ] )
                {
                case '(':
                    _exprIndex = i + 1;
                    return true;
                case ' ':
                case '\r':
                case '\n':
                case '\t':
                    break;
                default:
                    return false;
                }
            }
            return false;
        }

        private static bool IsDigit( char c )
        {
            return Digits.IndexOf( c ) >= 0;
        }

        private static bool IsHexDigit( char c )
        {
            return Hexchars.IndexOf( c ) >= 0;
        }

        private static bool IsSpace( char c )
        {
            switch ( c )
            {
            case ' ':
            case '\r':
            case '\n':
            case '\t':
                return true;
            }
            return false;
        }

        private static bool IsNameStartChar( char c )
        {
            return NameStartChars.IndexOf( c ) >= 0 || c >= 0x80;
        }

        private static bool IsNameChar( char c )
        {
            return NameStartChars.IndexOf( c ) >= 0 || NameChars.IndexOf( c ) >= 0 || c >= 0x80;
        }
    }
}