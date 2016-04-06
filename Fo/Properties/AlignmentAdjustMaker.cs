using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class AlignmentAdjustMaker : ToBeImplementedProperty.Maker
    {
        private Property _mDefaultProp;

        protected AlignmentAdjustMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new AlignmentAdjustMaker( propName );
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