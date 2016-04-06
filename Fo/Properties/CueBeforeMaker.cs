using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class CueBeforeMaker : ToBeImplementedProperty.Maker
    {
        private Property m_defaultProp;

        protected CueBeforeMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new CueBeforeMaker( propName );
        }


        public override bool IsInherited()
        {
            return false;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( m_defaultProp == null )
                m_defaultProp = Make( propertyList, "none", propertyList.getParentFObj() );
            return m_defaultProp;
        }
    }
}