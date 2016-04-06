using System.Collections;

namespace Fonet.Fo.Properties
{
    internal class GenericBorderWidth : LengthProperty.Maker
    {
        private static Hashtable _sHtKeywords;

        private Property _mDefaultProp;

        protected GenericBorderWidth( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new GenericBorderWidth( propName );
        }


        public override bool IsInherited()
        {
            return false;
        }

        public override Property GetShorthand( PropertyList propertyList )
        {
            var listprop = (ListProperty)propertyList.GetExplicitProperty( "border-width" );
            if ( listprop == null ) return null;
            IShorthandParser shparser = new BoxPropShorthandParser( listprop );
            return shparser.GetValueForProperty( PropName, this, propertyList );
        }

        private static void InitKeywords()
        {
            _sHtKeywords = new Hashtable( 3 ) { { "thin", "0.5pt" }, { "medium", "1pt" }, { "thick", "2pt" } };
        }

        protected override string CheckValueKeywords( string keyword )
        {
            if ( _sHtKeywords == null )
                InitKeywords();
            var value = (string)_sHtKeywords[ keyword ];
            return value ?? base.CheckValueKeywords( keyword );
        }

        public override Property Make( PropertyList propertyList )
        {
            return _mDefaultProp ?? ( _mDefaultProp = Make( propertyList, "0pt", propertyList.GetParentFObj() ) );
        }
    }
}