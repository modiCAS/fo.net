using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class SpeechRateMaker : ToBeImplementedProperty.Maker
    {
        private Property m_defaultProp;

        protected SpeechRateMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new SpeechRateMaker( propName );
        }


        public override bool IsInherited()
        {
            return true;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( m_defaultProp == null )
                m_defaultProp = Make( propertyList, "medium", propertyList.getParentFObj() );
            return m_defaultProp;
        }
    }
}