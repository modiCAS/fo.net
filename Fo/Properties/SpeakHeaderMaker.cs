using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class SpeakHeaderMaker : ToBeImplementedProperty.Maker
    {
        private Property m_defaultProp;

        protected SpeakHeaderMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new SpeakHeaderMaker( propName );
        }


        public override bool IsInherited()
        {
            return true;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( m_defaultProp == null )
                m_defaultProp = Make( propertyList, "once", propertyList.getParentFObj() );
            return m_defaultProp;
        }
    }
}