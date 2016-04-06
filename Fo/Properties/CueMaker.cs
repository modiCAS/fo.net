using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class CueMaker : ToBeImplementedProperty.Maker
    {
        private Property _mDefaultProp;

        protected CueMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new CueMaker( propName );
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