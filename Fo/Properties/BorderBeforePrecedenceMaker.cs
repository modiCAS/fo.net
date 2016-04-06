using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class BorderBeforePrecedenceMaker : ToBeImplementedProperty.Maker
    {
        private Property m_defaultProp;

        protected BorderBeforePrecedenceMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new BorderBeforePrecedenceMaker( propName );
        }


        public override bool IsInherited()
        {
            return false;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( m_defaultProp == null )
                m_defaultProp = Make( propertyList, "none", propertyList.getParentFObj() );
            return m_defaultProp;
        }
    }
}