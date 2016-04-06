using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class PauseMaker : ToBeImplementedProperty.Maker
    {
        private Property _mDefaultProp;

        protected PauseMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new PauseMaker( propName );
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