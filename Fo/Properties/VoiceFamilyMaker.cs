using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class VoiceFamilyMaker : ToBeImplementedProperty.Maker
    {
        private Property _mDefaultProp;

        protected VoiceFamilyMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new VoiceFamilyMaker( propName );
        }

        public override bool IsInherited()
        {
            return true;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}