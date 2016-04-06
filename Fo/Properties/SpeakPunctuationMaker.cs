using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class SpeakPunctuationMaker : ToBeImplementedProperty.Maker
    {
        private Property m_defaultProp;

        protected SpeakPunctuationMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new SpeakPunctuationMaker( propName );
        }


        public override bool IsInherited()
        {
            return true;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( m_defaultProp == null )
                m_defaultProp = Make( propertyList, "none", propertyList.getParentFObj() );
            return m_defaultProp;
        }
    }
}