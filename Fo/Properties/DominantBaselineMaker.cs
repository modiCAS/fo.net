using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class DominantBaselineMaker : ToBeImplementedProperty.Maker
    {
        private Property _mDefaultProp;

        protected DominantBaselineMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new DominantBaselineMaker( propName );
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