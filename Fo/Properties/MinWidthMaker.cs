using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class MinWidthMaker : ToBeImplementedProperty.Maker
    {
        private Property m_defaultProp;

        protected MinWidthMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new MinWidthMaker( propName );
        }


        public override bool IsInherited()
        {
            return false;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( m_defaultProp == null )
                m_defaultProp = Make( propertyList, "", propertyList.getParentFObj() );
            return m_defaultProp;
        }
    }
}