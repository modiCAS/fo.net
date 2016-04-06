namespace Fonet.Fo.Flow
{
    internal class TableBody : AbstractTableBody
    {
        public TableBody( FObj parent, PropertyList propertyList )
            : base( parent, propertyList )
        {
            Name = "fo:table-body";
        }

        public new static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        internal new class Maker : FObj.Maker
        {
            public override FObj Make( FObj parent, PropertyList propertyList )
            {
                return new TableBody( parent, propertyList );
            }
        }
    }
}