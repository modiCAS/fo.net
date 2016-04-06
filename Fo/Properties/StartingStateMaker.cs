using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class StartingStateMaker : ToBeImplementedProperty.Maker
    {
        private Property m_defaultProp;

        protected StartingStateMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new StartingStateMaker( propName );
        }

        public override bool IsInherited()
        {
            return false;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( m_defaultProp == null )
                m_defaultProp = Make( propertyList, "show", propertyList.getParentFObj() );
            return m_defaultProp;
        }
    }
}