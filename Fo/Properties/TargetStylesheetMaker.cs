using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class TargetStylesheetMaker : ToBeImplementedProperty.Maker
    {
        private Property m_defaultProp;

        protected TargetStylesheetMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new TargetStylesheetMaker( propName );
        }

        public override bool IsInherited()
        {
            return false;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( m_defaultProp == null )
                m_defaultProp = Make( propertyList, "use-normal-stylesheet", propertyList.getParentFObj() );
            return m_defaultProp;
        }
    }
}