using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class MinHeightMaker : ToBeImplementedProperty.Maker
    {
        private Property m_defaultProp;

        protected MinHeightMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new MinHeightMaker( propName );
        }


        public override bool IsInherited()
        {
            return false;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( m_defaultProp == null )
                m_defaultProp = Make( propertyList, "0pt", propertyList.getParentFObj() );
            return m_defaultProp;
        }
    }
}