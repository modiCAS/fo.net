using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class BackgroundColorMaker : GenericColor
    {
        private Property _mDefaultProp;

        protected BackgroundColorMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new BackgroundColorMaker( propName );
        }

        public override bool IsInherited()
        {
            return false;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "transparent", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }

        protected override Property ConvertPropertyDatatype(
            Property p, PropertyList propertyList, FObj fo )
        {
            string nameval = p.GetNCname();
            if ( nameval != null )
                return new ColorTypeProperty( new ColorType( nameval ) );
            return base.ConvertPropertyDatatype( p, propertyList, fo );
        }
    }
}