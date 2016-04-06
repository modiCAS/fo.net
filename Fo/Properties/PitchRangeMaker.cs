using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class PitchRangeMaker : ToBeImplementedProperty.Maker
    {
        private Property m_defaultProp;

        protected PitchRangeMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new PitchRangeMaker( propName );
        }


        public override bool IsInherited()
        {
            return true;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( m_defaultProp == null )
                m_defaultProp = Make( propertyList, "50", propertyList.getParentFObj() );
            return m_defaultProp;
        }
    }
}