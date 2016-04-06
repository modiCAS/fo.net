using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class PitchMaker : ToBeImplementedProperty.Maker
    {
        private Property _mDefaultProp;

        protected PitchMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new PitchMaker( propName );
        }


        public override bool IsInherited()
        {
            return true;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "medium", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}