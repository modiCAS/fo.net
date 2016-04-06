using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class TargetPresentationContextMaker : ToBeImplementedProperty.Maker
    {
        private Property _mDefaultProp;

        protected TargetPresentationContextMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new TargetPresentationContextMaker( propName );
        }

        public override bool IsInherited()
        {
            return false;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "use-target-processing-context", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}