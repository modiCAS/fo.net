using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class TargetStylesheetMaker : ToBeImplementedProperty.Maker
    {
        private Property _mDefaultProp;

        protected TargetStylesheetMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new TargetStylesheetMaker( propName );
        }

        public override bool IsInherited()
        {
            return false;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "use-normal-stylesheet", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}