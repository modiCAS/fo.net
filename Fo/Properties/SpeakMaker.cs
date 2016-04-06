using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class SpeakMaker : ToBeImplementedProperty.Maker
    {
        private Property m_defaultProp;

        protected SpeakMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new SpeakMaker( propName );
        }


        public override bool IsInherited()
        {
            return true;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( m_defaultProp == null )
                m_defaultProp = Make( propertyList, "normal", propertyList.getParentFObj() );
            return m_defaultProp;
        }
    }
}