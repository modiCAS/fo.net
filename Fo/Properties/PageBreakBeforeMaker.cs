using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class PageBreakBeforeMaker : ToBeImplementedProperty.Maker
    {
        private Property _mDefaultProp;

        protected PageBreakBeforeMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new PageBreakBeforeMaker( propName );
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