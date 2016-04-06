using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class PitchMaker : ToBeImplementedProperty.Maker
    {
        private Property m_defaultProp;

        protected PitchMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new PitchMaker( propName );
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