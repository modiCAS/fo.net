using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class CueAfterMaker : ToBeImplementedProperty.Maker
    {
        private Property _mDefaultProp;

        protected CueAfterMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new CueAfterMaker( propName );
        }


        public override bool IsInherited()
        {
            return false;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "none", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}