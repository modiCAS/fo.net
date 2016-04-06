using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class VoiceFamilyMaker : ToBeImplementedProperty.Maker
    {
        private Property m_defaultProp;

        protected VoiceFamilyMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new VoiceFamilyMaker( propName );
        }

        public override bool IsInherited()
        {
            return true;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( m_defaultProp == null )
                m_defaultProp = Make( propertyList, "", propertyList.getParentFObj() );
            return m_defaultProp;
        }
    }
}