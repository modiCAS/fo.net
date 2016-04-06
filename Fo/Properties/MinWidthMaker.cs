using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class MinWidthMaker : ToBeImplementedProperty.Maker
    {
        private Property _mDefaultProp;

        protected MinWidthMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new MinWidthMaker( propName );
        }


        public override bool IsInherited()
        {
            return false;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}