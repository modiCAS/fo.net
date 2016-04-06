using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class FontSizeMaker : LengthProperty.Maker
    {
        private Property _mDefaultProp;

        protected FontSizeMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new FontSizeMaker( propName );
        }


        public override bool IsInherited()
        {
            return true;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "12pt", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }

        public override IPercentBase GetPercentBase( FObj fo, PropertyList propertyList )
        {
            return new LengthBase( fo, propertyList, LengthBase.InhFontsize );
        }
    }
}