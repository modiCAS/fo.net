using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class BorderAfterPrecedenceMaker : ToBeImplementedProperty.Maker
    {
        private Property m_defaultProp;

        protected BorderAfterPrecedenceMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new BorderAfterPrecedenceMaker( propName );
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