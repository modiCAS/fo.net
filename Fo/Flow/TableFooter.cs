namespace Fonet.Fo.Flow
{
    internal class TableFooter : AbstractTableBody
    {
        public TableFooter( FObj parent, PropertyList propertyList )
            : base( parent, propertyList )
        {
            Name = "fo:table-footer";
        }

        public override int GetYPosition()
        {
            return AreaContainer.GetCurrentYPosition() - SpaceBefore;
        }

        public override void SetYPosition( int value )
        {
            AreaContainer.SetYPosition( value + 2 * SpaceBefore );
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