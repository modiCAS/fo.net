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
            Property p = null;
            ListProperty listprop;

            if ( p == null )
            {
                listprop = (ListProperty)propertyList.GetExplicitProperty( "border-width" );
                if ( listprop != null )
                {
                    IShorthandParser shparser = new BoxPropShorthandParser( listprop );
                    p = shparser.GetValueForProperty( PropName, this, propertyList );
                }
            }

            return p;
        }

        private static void InitKeywords()
        {
            _sHtKeywords = new Hashtable( 3 );

            _sHtKeywords.Add( "thin", "0.5pt" );

            _sHtKeywords.Add( "medium", "1pt" );

            _sHtKeywords.Add( "thick", "2pt" );
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

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "0pt", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}