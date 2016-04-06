using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class PageBreakInsideMaker : ToBeImplementedProperty.Maker
    {
        private Property m_defaultProp;

        protected PageBreakInsideMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new PageBreakInsideMaker( propName );
        }


        public override bool IsInherited()
        {
            return true;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( m_defaultProp == null )
                m_defaultProp = Make( propertyList, "auto", propertyList.getParentFObj() );
            return m_defaultProp;
        }
    }
}