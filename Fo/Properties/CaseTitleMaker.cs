using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class CaseTitleMaker : ToBeImplementedProperty.Maker
    {
        private Property _mDefaultProp;

        protected CaseTitleMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new CaseTitleMaker( propName );
        }


        public override bool IsInherited()
        {
            return false;
        }

        public override Property Make( PropertyList propertyList )
        {
            return _mDefaultProp ?? ( _mDefaultProp = Make( propertyList, "", propertyList.GetParentFObj() ) );
        }
    }
}