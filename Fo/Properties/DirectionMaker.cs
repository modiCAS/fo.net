using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class DirectionMaker : ToBeImplementedProperty.Maker
    {
        private Property _mDefaultProp;

        protected DirectionMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new DirectionMaker( propName );
        }


        public override bool IsInherited()
        {
            return true;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "ltr", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}