using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class RelativePositionMaker : ToBeImplementedProperty.Maker
    {
        private Property m_defaultProp;

        protected RelativePositionMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new RelativePositionMaker( propName );
        }


        public override bool IsInherited()
        {
            return false;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( m_defaultProp == null )
                m_defaultProp = Make( propertyList, "static", propertyList.getParentFObj() );
            return m_defaultProp;
        }
    }
}