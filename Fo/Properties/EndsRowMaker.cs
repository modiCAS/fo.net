using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class EndsRowMaker : ToBeImplementedProperty.Maker
    {
        private Property _mDefaultProp;

        protected EndsRowMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new EndsRowMaker( propName );
        }


        public override bool IsInherited()
        {
            return false;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "false", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}