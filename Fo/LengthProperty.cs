using Fonet.DataTypes;
using Fonet.Fo.Expr;

namespace Fonet.Fo
{
    internal class LengthProperty : Property
    {
        private readonly Length _length;

        public LengthProperty( Length length )
        {
            this._length = length;
        }

        public override Numeric GetNumeric()
        {
            return _length.AsNumeric();
        }

        public override Length GetLength()
        {
            return _length;
        }

        public override object GetObject()
        {
            return _length;
        }

        internal class Maker : PropertyMaker
        {
            public Maker( string name ) : base( name )
            {
            }

            protected virtual bool IsAutoLengthAllowed()
            {
                return false;
            }

            public override Property ConvertProperty(
                Property p, PropertyList propertyList, FObj fo )
            {
                if ( IsAutoLengthAllowed() )
                {
                    string pval = p.GetString();
                    if ( pval != null && pval.Equals( "auto" ) )
                        return new LengthProperty( new AutoLength() );
                }
                if ( p is LengthProperty )
                    return p;
                Length val = p.GetLength();
                if ( val != null )
                    return new LengthProperty( val );
                return ConvertPropertyDatatype( p, propertyList, fo );
            }
        }
    }
}