using System.Collections;

namespace Fonet.Fo
{
    internal class GenericShorthandParser : IShorthandParser
    {
        protected ArrayList List;

        public GenericShorthandParser( ListProperty listprop )
        {
            List = listprop.GetList();
        }

        public Property GetValueForProperty( string propName,
            PropertyMaker maker,
            PropertyList propertyList )
        {
            if ( Count() == 1 )
            {
                string sval = ( (Property)List[ 0 ] ).GetString();
                if ( sval != null && sval.Equals( "inherit" ) )
                    return propertyList.GetFromParentProperty( propName );
            }
            return ConvertValueForProperty( propName, maker, propertyList );
        }

        protected Property GetElement( int index )
        {
            if ( List.Count > index )
                return (Property)List[ index ];
            return null;
        }

        protected int Count()
        {
            return List.Count;
        }

        protected virtual Property ConvertValueForProperty(
            string propName, PropertyMaker maker, PropertyList propertyList )
        {
            foreach ( Property p in List )
            {
                Property prop = maker.ConvertShorthandProperty( propertyList, p, null );
                if ( prop != null )
                    return prop;
            }
            return null;
        }
    }
}