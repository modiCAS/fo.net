namespace Fonet.Fo.Flow
{
    internal class TableFooter : AbstractTableBody
    {
        public TableFooter( FObj parent, PropertyList propertyList )
            : base( parent, propertyList )
        {
            name = "fo:table-footer";
        }

        public override int GetYPosition()
        {
            return areaContainer.GetCurrentYPosition() - spaceBefore;
        }

        public override void SetYPosition( int value )
        {
            areaContainer.setYPosition( value + 2 * spaceBefore );
        }

        public new static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        internal new class Maker : FObj.Maker
        {
            public override FObj Make( FObj parent, PropertyList propertyList )
            {
                return new TableFooter( parent, propertyList );
            }
        }
    }
}