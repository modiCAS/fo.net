using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class RichnessMaker : ToBeImplementedProperty.Maker
    {
        private Property m_defaultProp;

        protected RichnessMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new RichnessMaker( propName );
        }


        public override bool IsInherited()
        {
            return true;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( m_defaultProp == null )
                m_defaultProp = Make( propertyList, "50", propertyList.getParentFObj() );
            return m_defaultProp;
        }
    }
}