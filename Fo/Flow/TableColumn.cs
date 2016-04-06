using Fonet.DataTypes;
using Fonet.Fo.Properties;
using Fonet.Layout;

namespace Fonet.Fo.Flow
{
    internal class TableColumn : FObj
    {
        private AreaContainer areaContainer;
        private int columnOffset;
        private int columnWidth;
        private Length columnWidthPropVal;
        private int iColumnNumber;
        private int numColumnsRepeated;
        private bool setup;

        public TableColumn( FObj parent, PropertyList propertyList )
            : base( parent, propertyList )
        {
            name = "fo:table-column";
        }

        public new static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        public Length GetColumnWidthAsLength()
        {
            return columnWidthPropVal;
        }

        public int GetColumnWidth()
        {
            return columnWidth;
        }

        public void SetColumnWidth( int columnWidth )
        {
            this.columnWidth = columnWidth;
        }

        public int GetColumnNumber()
        {
            return iColumnNumber;
        }

        public int GetNumColumnsRepeated()
        {
            return numColumnsRepeated;
        }

        public void DoSetup( Area area )
        {
            BorderAndPadding bap = propMgr.GetBorderAndPadding();
            BackgroundProps bProps = propMgr.GetBackgroundProps();

            iColumnNumber =
                properties.GetProperty( "column-number" ).GetNumber().IntValue();

            numColumnsRepeated =
                properties.GetProperty( "number-columns-repeated" ).GetNumber().IntValue();

            columnWidthPropVal =
                properties.GetProperty( "column-width" ).GetLength();

            columnWidth = columnWidthPropVal.MValue();

            string id = properties.GetProperty( "id" ).GetString();
            area.getIDReferences().InitializeID( id, area );

            setup = true;
        }

        public override Status Layout( Area area )
        {
            if ( marker == MarkerBreakAfter )
                return new Status( Status.OK );

            if ( marker == MarkerStart )
            {
                if ( !setup )
                    DoSetup( area );
            }
            if ( columnWidth > 0 )
            {
                areaContainer =
                    new AreaContainer( propMgr.GetFontState( area.getFontInfo() ),
                        columnOffset, 0, columnWidth,
                        area.getContentHeight(), Position.RELATIVE );
                areaContainer.foCreator = this;
                areaContainer.setPage( area.getPage() );
                areaContainer.setBorderAndPadding( propMgr.GetBorderAndPadding() );
                areaContainer.setBackground( propMgr.GetBackgroundProps() );
                areaContainer.SetHeight( area.GetHeight() );
                area.addChild( areaContainer );
            }
            return new Status( Status.OK );
        }

        public void SetColumnOffset( int columnOffset )
        {
            this.columnOffset = columnOffset;
        }

        public void SetHeight( int height )
        {
            if ( areaContainer != null )
            {
                areaContainer.setMaxHeight( height );
                areaContainer.SetHeight( height );
            }
        }

        internal new class Maker : FObj.Maker
        {
            public override FObj Make( FObj parent, PropertyList propertyList )
            {
                return new TableColumn( parent, propertyList );
            }
        }
    }
}