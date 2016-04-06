using Fonet.Fo;

namespace Fonet.DataTypes
{
    internal class ToBeImplementedProperty : Property
    {
        public ToBeImplementedProperty( string propName )
        {
            FonetDriver.ActiveDriver.FireFonetWarning(
                "property - \"" + propName + "\" is not implemented yet." );
        }

        internal class Maker : PropertyMaker
        {
            public Maker( string propName ) : base( propName )
            {
            }

            public override Property ConvertProperty(
                Property p, PropertyList propertyList, FObj fo )
            {
                if ( p is ToBeImplementedProperty )
                    return p;
                var val = new ToBeImplementedProperty( PropName );
                return val;
            }
        }
    }
}