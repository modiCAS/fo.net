using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class SuppressAtLineBreakMaker : ToBeImplementedProperty.Maker
    {
        private Property _mDefaultProp;

        protected SuppressAtLineBreakMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new SuppressAtLineBreakMaker( propName );
        }

        public override bool IsInherited()
        {
            return false;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "auto", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}