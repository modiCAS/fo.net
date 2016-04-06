using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class AutoRestoreMaker : ToBeImplementedProperty.Maker
    {
        private Property m_defaultProp;

        protected AutoRestoreMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new AutoRestoreMaker( propName );
        }


        public override bool IsInherited()
        {
            return true;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( m_defaultProp == null )
                m_defaultProp = Make( propertyList, "false", propertyList.getParentFObj() );
            return m_defaultProp;
        }
    }
}