using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class ScriptMaker : ToBeImplementedProperty.Maker
    {
        private Property m_defaultProp;

        protected ScriptMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new ScriptMaker( propName );
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