using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class PageBreakAfterMaker : ToBeImplementedProperty.Maker
    {
        private Property _mDefaultProp;

        protected PageBreakAfterMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new PageBreakAfterMaker( propName );
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