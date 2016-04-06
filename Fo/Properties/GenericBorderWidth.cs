using System.Collections;

namespace Fonet.Fo.Properties
{
    internal class GenericBorderWidth : LengthProperty.Maker
    {
        private static Hashtable s_htKeywords;

        private Property m_defaultProp;

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

        private static void initKeywords()
        {
            s_htKeywords = new Hashtable( 3 );

            s_htKeywords.Add( "thin", "0.5pt" );

            s_htKeywords.Add( "medium", "1pt" );

            s_htKeywords.Add( "thick", "2pt" );
        }

        protected override string CheckValueKeywords( string keyword )
        {
            if ( s_htKeywords == null )
                initKeywords();
            var value = (string)s_htKeywords[ keyword ];
            if ( value == null )
                return base.CheckValueKeywords( keyword );
            return value;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( m_defaultProp == null )
                m_defaultProp = Make( propertyList, "0pt", propertyList.getParentFObj() );
            return m_defaultProp;
        }
    }
}