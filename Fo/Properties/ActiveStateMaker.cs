using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class ActiveStateMaker : ToBeImplementedProperty.Maker
    {
        private Property m_defaultProp;

        protected ActiveStateMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new ActiveStateMaker( propName );
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