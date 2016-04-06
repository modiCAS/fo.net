using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class PauseBeforeMaker : ToBeImplementedProperty.Maker
    {
        private Property _mDefaultProp;

        protected PauseBeforeMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new PauseBeforeMaker( propName );
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