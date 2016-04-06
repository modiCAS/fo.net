using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class AlignmentBaselineMaker : ToBeImplementedProperty.Maker
    {
        private Property _mDefaultProp;

        protected AlignmentBaselineMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new AlignmentBaselineMaker( propName );
        }


        public override bool IsInherited()
        {
            return false;
        }

        public override Property Make( PropertyList propertyList )
        {
            return _mDefaultProp ?? ( _mDefaultProp = Make( propertyList, "auto", propertyList.GetParentFObj() ) );
        }
    }
}