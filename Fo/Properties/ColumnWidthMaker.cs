namespace Fonet.Fo.Properties
{
    internal class ColumnWidthMaker : LengthProperty.Maker
    {
        protected ColumnWidthMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new ColumnWidthMaker( propName );
        }


        public override bool IsInherited()
        {
            return false;
        }

        public override Property Make( PropertyList propertyList )
        {
            return Make( propertyList, "proportional-column-width(1)", propertyList.GetParentFObj() );
        }
    }
}