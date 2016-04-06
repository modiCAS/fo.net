using Fonet.DataTypes;

namespace Fonet.Fo
{
    internal class ColorTypeProperty : Property
    {
        private readonly ColorType _colorType;

        public ColorTypeProperty( ColorType colorType )
        {
            this._colorType = colorType;
        }

        public override ColorType GetColorType()
        {
            return _colorType;
        }

        public override object GetObject()
        {
            return _colorType;
        }

        internal class Maker : PropertyMaker
        {
            public Maker( string propName ) : base( propName )
            {
            }

            public override Property ConvertProperty(
                Property p, PropertyList propertyList, FObj fo )
            {
                if ( p is ColorTypeProperty )
                    return p;
                ColorType val = p.GetColorType();
                if ( val != null )
                    return new ColorTypeProperty( val );
                return ConvertPropertyDatatype( p, propertyList, fo );
            }
        }
    }
}