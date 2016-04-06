using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class SpeakNumeralMaker : ToBeImplementedProperty.Maker
    {
        private Property m_defaultProp;

        protected SpeakNumeralMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new SpeakNumeralMaker( propName );
        }


        public override bool IsInherited()
        {
            return true;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( m_defaultProp == null )
                m_defaultProp = Make( propertyList, "continuous", propertyList.getParentFObj() );
            return m_defaultProp;
        }
    }
}