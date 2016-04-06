using System;

namespace Fonet.Fo
{
    internal class StringProperty : Property
    {
        private readonly string _str;

        public StringProperty( string str )
        {
            this._str = str;
        }

        public override object GetObject()
        {
            return _str;
        }

        public override string GetString()
        {
            return _str;
        }

        internal class Maker : PropertyMaker
        {
            public Maker( string propName ) : base( propName )
            {
            }

            public override Property Make(
                PropertyList propertyList, string value, FObj fo )
            {
                int vlen = value.Length - 1;
                if ( vlen > 0 )
                {
                    char q1 = value[ 0 ];
                    if ( q1 == '"' || q1 == '\'' )
                    {
                        if ( value[ vlen ] == q1 )
                            return new StringProperty( value.Substring( 1, vlen - 2 ) );
                        Console.WriteLine( "Warning String-valued property starts with quote"
                            + " but doesn't end with quote: "
                            + value );
                    }
                }
                return new StringProperty( value );
            }
        }
    }
}