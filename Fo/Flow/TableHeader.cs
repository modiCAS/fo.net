namespace Fonet.Fo.Flow
{
    internal class TableHeader : AbstractTableBody
    {
        public TableHeader( FObj parent, PropertyList propertyList )
            : base( parent, propertyList )
        {
            name = "fo:table-header";
        }

        public new static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        internal new class Maker : FObj.Maker
        {
            public override FObj Make( FObj parent, PropertyList propertyList )
            {
                return new TableHeader( parent, propertyList );
            }
        }
    }
}