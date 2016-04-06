using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class ShowDestinationMaker : ToBeImplementedProperty.Maker
    {
        private Property m_defaultProp;

        protected ShowDestinationMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new ShowDestinationMaker( propName );
        }


        public override bool IsInherited()
        {
            return false;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( m_defaultProp == null )
                m_defaultProp = Make( propertyList, "replace", propertyList.getParentFObj() );
            return m_defaultProp;
        }
    }
}