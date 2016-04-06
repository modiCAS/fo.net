using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class CaptionSideMaker : ToBeImplementedProperty.Maker
    {
        private Property m_defaultProp;

        protected CaptionSideMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new CaptionSideMaker( propName );
        }


        public override bool IsInherited()
        {
            return true;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( m_defaultProp == null )
                m_defaultProp = Make( propertyList, "before", propertyList.getParentFObj() );
            return m_defaultProp;
        }
    }
}