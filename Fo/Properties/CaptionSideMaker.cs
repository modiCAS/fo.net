using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class CaptionSideMaker : ToBeImplementedProperty.Maker
    {
        private Property _mDefaultProp;

        protected CaptionSideMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new CaptionSideMaker( propName );
        }


        public override bool IsInherited()
        {
            return true;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "before", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}