using System.Collections;
using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class LineHeightMaker : LengthProperty.Maker
    {
        private static Hashtable _sHtKeywords;

        protected LineHeightMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new LineHeightMaker( propName );
        }


        public override bool IsInherited()
        {
            return true;
        }

        public override bool InheritsSpecified()
        {
            return true;
        }

        public override Property Make( PropertyList propertyList )
        {
            return Make( propertyList, "normal", propertyList.GetParentFObj() );
        }

        private static void InitKeywords()
        {
            _sHtKeywords = new Hashtable( 1 );

            _sHtKeywords.Add( "normal", "1.2em" );
        }

        protected override string CheckValueKeywords( string keyword )
        {
            if ( _sHtKeywords == null )
                InitKeywords();
            var value = (string)_sHtKeywords[ keyword ];
            if ( value == null )
                return base.CheckValueKeywords( keyword );
            return value;
        }

        protected override Property ConvertPropertyDatatype( Property p, PropertyList propertyList, FObj fo )
        {
            {
                Number numval =
                    p.GetNumber();
                if ( numval != null )
                {
                    return new LengthProperty(
                        new PercentLength( numval.DoubleValue(),
                            GetPercentBase( fo, propertyList ) ) );
                }
            }

            return base.ConvertPropertyDatatype( p, propertyList, fo );
        }

        public override IPercentBase GetPercentBase( FObj fo, PropertyList propertyList )
        {
            return new LengthBase( fo, propertyList, LengthBase.Fontsize );
        }
    }
}