using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class MinHeightMaker : ToBeImplementedProperty.Maker
    {
        private Property _mDefaultProp;

        protected MinHeightMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new MinHeightMaker( propName );
        }


        public override bool IsInherited()
        {
            return false;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "0pt", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}