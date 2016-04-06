using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class ReferenceOrientationMaker : ToBeImplementedProperty.Maker
    {
        private Property m_defaultProp;

        protected ReferenceOrientationMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new ReferenceOrientationMaker( propName );
        }


        public override bool IsInherited()
        {
            return true;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( m_defaultProp == null )
                m_defaultProp = Make( propertyList, "0", propertyList.getParentFObj() );
            return m_defaultProp;
        }
    }
}