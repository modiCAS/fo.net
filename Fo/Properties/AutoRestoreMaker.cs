using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class AutoRestoreMaker : ToBeImplementedProperty.Maker
    {
        private Property _mDefaultProp;

        protected AutoRestoreMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new AutoRestoreMaker( propName );
        }


        public override bool IsInherited()
        {
            return true;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "false", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}