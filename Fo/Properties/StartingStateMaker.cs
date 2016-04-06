using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class StartingStateMaker : ToBeImplementedProperty.Maker
    {
        private Property _mDefaultProp;

        protected StartingStateMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new StartingStateMaker( propName );
        }

        public override bool IsInherited()
        {
            return false;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "show", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}