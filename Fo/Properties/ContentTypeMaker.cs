using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class ContentTypeMaker : ToBeImplementedProperty.Maker
    {
        private Property m_defaultProp;

        protected ContentTypeMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new ContentTypeMaker( propName );
        }


        public override bool IsInherited()
        {
            return false;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( m_defaultProp == null )
                m_defaultProp = Make( propertyList, "auto", propertyList.getParentFObj() );
            return m_defaultProp;
        }
    }
}