using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class FontSizeMaker : LengthProperty.Maker
    {
        private Property m_defaultProp;

        protected FontSizeMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new FontSizeMaker( propName );
        }


        public override bool IsInherited()
        {
            return true;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( m_defaultProp == null )
                m_defaultProp = Make( propertyList, "12pt", propertyList.getParentFObj() );
            return m_defaultProp;
        }

        public override IPercentBase GetPercentBase( FObj fo, PropertyList propertyList )
        {
            return new LengthBase( fo, propertyList, LengthBase.INH_FONTSIZE );
        }
    }
}