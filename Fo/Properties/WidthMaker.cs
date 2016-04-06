using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class WidthMaker : LengthProperty.Maker
    {
        private Property _mDefaultProp;

        protected WidthMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new WidthMaker( propName );
        }

        public override bool IsInherited()
        {
            return false;
        }

        protected override bool IsAutoLengthAllowed()
        {
            return true;
        }

        public override IPercentBase GetPercentBase( FObj fo, PropertyList propertyList )
        {
            return new LengthBase( fo, propertyList, LengthBase.ContainingBox );
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "auto", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}