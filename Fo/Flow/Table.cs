using System.Collections;
using Fonet.DataTypes;
using Fonet.Fo.Properties;
using Fonet.Layout;

namespace Fonet.Fo.Flow
{
    internal class Table : FObj
    {
        private const int MINCOLWIDTH = 10000;

        private AreaContainer areaContainer;

        private bool bAutoLayout;

        private int bodyCount;

        private int breakAfter;

        private int breakBefore;

        private readonly ArrayList columns = new ArrayList();

        private int contentWidth;

        private int height;

        private string id;

        private LengthRange ipd;

        private int maxIPD;

        private int minIPD;

        private bool omitFooterAtBreak;

        private bool omitHeaderAtBreak;

        private int optIPD;

        private int spaceAfter;

        private int spaceBefore;

        private TableFooter tableFooter;

        private TableHeader tableHeader;

        public Table( FObj parent, PropertyList propertyList )
            : base( parent, propertyList )
        {
            name = "fo:table";
        }

        public new static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        public override Status Layout( Area area )
        {
            if ( marker == MarkerBreakAfter )
                return new Status( Status.OK );

            if ( marker == MarkerStart )
            {
                AccessibilityProps mAccProps = propMgr.GetAccessibilityProps();
                AuralProps mAurProps = propMgr.GetAuralProps();
                BorderAndPadding bap = propMgr.GetBorderAndPadding();
                BackgroundProps bProps = propMgr.GetBackgroundProps();
                MarginProps mProps = propMgr.GetMarginProps();
                RelativePositionProps mRelProps = propMgr.GetRelativePositionProps();

                breakBefore = properties.GetProperty( "break-before" ).GetEnum();
                breakAfter = properties.GetProperty( "break-after" ).GetEnum();
                spaceBefore =
                    properties.GetProperty( "space-before.optimum" ).GetLength().MValue();
                spaceAfter =
                    properties.GetProperty( "space-after.optimum" ).GetLength().MValue();
                ipd =
                    properties.GetProperty( "inline-progression-dimension" ).
                        GetLengthRange();
                height = properties.GetProperty( "height" ).GetLength().MValue();
                bAutoLayout = properties.GetProperty( "table-layout" ).GetEnum() ==
                    TableLayout.AUTO;

                id = properties.GetProperty( "id" ).GetString();

                omitHeaderAtBreak =
                    properties.GetProperty( "table-omit-header-at-break" ).GetEnum()
                        == GenericBoolean.Enums.TRUE;
                omitFooterAtBreak =
                    properties.GetProperty( "table-omit-footer-at-break" ).GetEnum()
                        == GenericBoolean.Enums.TRUE;

                if ( area is BlockArea )
                    area.end();
                if ( areaContainer
                    == null )
                    area.getIDReferences().CreateID( id );

                marker = 0;

                if ( breakBefore == GenericBreak.Enums.PAGE )
                    return new Status( Status.FORCE_PAGE_BREAK );

                if ( breakBefore == GenericBreak.Enums.ODD_PAGE )
                    return new Status( Status.FORCE_PAGE_BREAK_ODD );

                if ( breakBefore == GenericBreak.Enums.EVEN_PAGE )
                    return new Status( Status.FORCE_PAGE_BREAK_EVEN );
            }

            if ( spaceBefore != 0 && marker == 0 )
                area.addDisplaySpace( spaceBefore );

            if ( marker == 0 && areaContainer == null )
                area.getIDReferences().ConfigureID( id, area );

            int spaceLeft = area.spaceLeft();
            areaContainer =
                new AreaContainer( propMgr.GetFontState( area.getFontInfo() ), 0, 0,
                    area.getAllocationWidth(), area.spaceLeft(),
                    Position.STATIC );

            areaContainer.foCreator = this;
            areaContainer.setPage( area.getPage() );
            areaContainer.setParent( area );
            areaContainer.setBackground( propMgr.GetBackgroundProps() );
            areaContainer.setBorderAndPadding( propMgr.GetBorderAndPadding() );
            areaContainer.start();

            areaContainer.setAbsoluteHeight( area.getAbsoluteHeight() );
            areaContainer.setIDReferences( area.getIDReferences() );

            var addedHeader = false;
            var addedFooter = false;
            int numChildren = children.Count;

            if ( columns.Count == 0 )
            {
                FindColumns( areaContainer );
                if ( bAutoLayout )
                {
                    FonetDriver.ActiveDriver.FireFonetWarning(
                        "table-layout=auto is not supported, using fixed!" );
                }
                contentWidth =
                    CalcFixedColumnWidths( areaContainer.getAllocationWidth() );
            }
            areaContainer.setAllocationWidth( contentWidth );
            layoutColumns( areaContainer );

            for ( int i = marker; i < numChildren; i++ )
            {
                var fo = (FONode)children[ i ];
                if ( fo is TableHeader )
                {
                    if ( columns.Count == 0 )
                    {
                        FonetDriver.ActiveDriver.FireFonetWarning(
                            "Current implementation of tables requires a table-column for each column, indicating column-width" );
                        return new Status( Status.OK );
                    }
                    tableHeader = (TableHeader)fo;
                    tableHeader.SetColumns( columns );
                }
                else if ( fo is TableFooter )
                {
                    if ( columns.Count == 0 )
                    {
                        FonetDriver.ActiveDriver.FireFonetWarning(
                            "Current implementation of tables requires a table-column for each column, indicating column-width" );
                        return new Status( Status.OK );
                    }
                    tableFooter = (TableFooter)fo;
                    tableFooter.SetColumns( columns );
                }
                else if ( fo is TableBody )
                {
                    if ( columns.Count == 0 )
                    {
                        FonetDriver.ActiveDriver.FireFonetWarning(
                            "Current implementation of tables requires a table-column for each column, indicating column-width" );
                        return new Status( Status.OK );
                    }
                    Status status;
                    if ( tableHeader != null && !addedHeader )
                    {
                        if ( ( status =
                            tableHeader.Layout( areaContainer ) ).isIncomplete() )
                        {
                            tableHeader.ResetMarker();
                            return new Status( Status.AREA_FULL_NONE );
                        }
                        addedHeader = true;
                        tableHeader.ResetMarker();
                        area.setMaxHeight( area.getMaxHeight() - spaceLeft
                            + areaContainer.getMaxHeight() );
                    }
                    if ( tableFooter != null && !omitFooterAtBreak
                        && !addedFooter )
                    {
                        if ( ( status =
                            tableFooter.Layout( areaContainer ) ).isIncomplete() )
                            return new Status( Status.AREA_FULL_NONE );
                        addedFooter = true;
                        tableFooter.ResetMarker();
                    }
                    fo.SetWidows( widows );
                    fo.SetOrphans( orphans );
                    ( (TableBody)fo ).SetColumns( columns );

                    if ( ( status = fo.Layout( areaContainer ) ).isIncomplete() )
                    {
                        marker = i;
                        if ( bodyCount == 0
                            && status.getCode() == Status.AREA_FULL_NONE )
                        {
                            if ( tableHeader != null )
                                tableHeader.RemoveLayout( areaContainer );
                            if ( tableFooter != null )
                                tableFooter.RemoveLayout( areaContainer );
                            ResetMarker();
                        }
                        if ( areaContainer.getContentHeight() > 0 )
                        {
                            area.addChild( areaContainer );
                            area.increaseHeight( areaContainer.GetHeight() );
                            if ( omitHeaderAtBreak )
                                tableHeader = null;
                            if ( tableFooter != null && !omitFooterAtBreak )
                            {
                                ( (TableBody)fo ).SetYPosition( tableFooter.GetYPosition() );
                                tableFooter.SetYPosition( tableFooter.GetYPosition()
                                    + ( (TableBody)fo ).GetHeight() );
                            }
                            SetupColumnHeights();
                            status = new Status( Status.AREA_FULL_SOME );
                        }
                        return status;
                    }
                    bodyCount++;
                    area.setMaxHeight( area.getMaxHeight() - spaceLeft
                        + areaContainer.getMaxHeight() );
                    if ( tableFooter != null && !omitFooterAtBreak )
                    {
                        ( (TableBody)fo ).SetYPosition( tableFooter.GetYPosition() );
                        tableFooter.SetYPosition( tableFooter.GetYPosition()
                            + ( (TableBody)fo ).GetHeight() );
                    }
                }
            }

            if ( tableFooter != null && omitFooterAtBreak )
            {
                if ( tableFooter.Layout( areaContainer ).isIncomplete() )
                {
                    FonetDriver.ActiveDriver.FireFonetWarning(
                        "Footer could not fit on page, moving last body row to next page" );
                    area.addChild( areaContainer );
                    area.increaseHeight( areaContainer.GetHeight() );
                    if ( omitHeaderAtBreak )
                        tableHeader = null;
                    tableFooter.RemoveLayout( areaContainer );
                    tableFooter.ResetMarker();
                    return new Status( Status.AREA_FULL_SOME );
                }
            }

            if ( height != 0 )
                areaContainer.SetHeight( height );

            SetupColumnHeights();

            areaContainer.end();
            area.addChild( areaContainer );

            area.increaseHeight( areaContainer.GetHeight() );

            if ( spaceAfter != 0 )
                area.addDisplaySpace( spaceAfter );

            if ( area is BlockArea )
                area.start();

            if ( breakAfter == GenericBreak.Enums.PAGE )
            {
                marker = MarkerBreakAfter;
                return new Status( Status.FORCE_PAGE_BREAK );
            }

            if ( breakAfter == GenericBreak.Enums.ODD_PAGE )
            {
                marker = MarkerBreakAfter;
                return new Status( Status.FORCE_PAGE_BREAK_ODD );
            }

            if ( breakAfter == GenericBreak.Enums.EVEN_PAGE )
            {
                marker = MarkerBreakAfter;
                return new Status( Status.FORCE_PAGE_BREAK_EVEN );
            }

            return new Status( Status.OK );
        }

        protected void SetupColumnHeights()
        {
            foreach ( TableColumn c in columns )
            {
                if ( c != null )
                    c.SetHeight( areaContainer.getContentHeight() );
            }
        }

        private void FindColumns( Area areaContainer )
        {
            var nextColumnNumber = 1;
            foreach ( FONode fo in children )
            {
                if ( fo is TableColumn )
                {
                    var c = (TableColumn)fo;
                    c.DoSetup( areaContainer );
                    int numColumnsRepeated = c.GetNumColumnsRepeated();
                    int currentColumnNumber = c.GetColumnNumber();
                    if ( currentColumnNumber == 0 )
                        currentColumnNumber = nextColumnNumber;
                    for ( var j = 0; j < numColumnsRepeated; j++ )
                    {
                        if ( currentColumnNumber < columns.Count )
                        {
                            if ( columns[ currentColumnNumber - 1 ] != null )
                            {
                                FonetDriver.ActiveDriver.FireFonetWarning(
                                    "More than one column object assigned to column " + currentColumnNumber );
                            }
                        }
                        columns.Insert( currentColumnNumber - 1, c );
                        currentColumnNumber++;
                    }
                    nextColumnNumber = currentColumnNumber;
                }
            }
        }

        private int CalcFixedColumnWidths( int maxAllocationWidth )
        {
            var nextColumnNumber = 1;
            var iEmptyCols = 0;
            var dTblUnits = 0.0;
            var iFixedWidth = 0;
            var dWidthFactor = 0.0;
            var dUnitLength = 0.0;
            var tuMin = 100000.0;

            foreach ( TableColumn c in columns )
            {
                if ( c == null )
                {
                    FonetDriver.ActiveDriver.FireFonetWarning(
                        "No table-column specification for column " +
                            nextColumnNumber );
                    iEmptyCols++;
                }
                else
                {
                    Length colLength = c.GetColumnWidthAsLength();
                    double tu = colLength.GetTableUnits();
                    if ( tu > 0 && tu < tuMin && colLength.MValue() == 0 )
                        tuMin = tu;
                    dTblUnits += tu;
                    iFixedWidth += colLength.MValue();
                }
                nextColumnNumber++;
            }

            SetIPD( dTblUnits > 0.0, maxAllocationWidth );
            if ( dTblUnits > 0.0 )
            {
                var iProportionalWidth = 0;
                if ( optIPD > iFixedWidth )
                    iProportionalWidth = optIPD - iFixedWidth;
                else if ( maxIPD > iFixedWidth )
                    iProportionalWidth = maxIPD - iFixedWidth;
                else
                    iProportionalWidth = maxAllocationWidth - iFixedWidth;
                if ( iProportionalWidth > 0 )
                    dUnitLength = iProportionalWidth / dTblUnits;
                else
                {
                    FonetDriver.ActiveDriver.FireFonetWarning( string.Format(
                        "Sum of fixed column widths {0} greater than maximum available IPD {1}; no space for {2} propertional units",
                        iFixedWidth, maxAllocationWidth, dTblUnits ) );
                    dUnitLength = MINCOLWIDTH / tuMin;
                }
            }
            else
            {
                int iTableWidth = iFixedWidth;
                if ( minIPD > iFixedWidth )
                {
                    iTableWidth = minIPD;
                    dWidthFactor = minIPD / (double)iFixedWidth;
                }
                else if ( maxIPD < iFixedWidth )
                {
                    if ( maxIPD != 0 )
                    {
                        FonetDriver.ActiveDriver.FireFonetWarning(
                            "Sum of fixed column widths " + iFixedWidth +
                                " greater than maximum specified IPD " + maxIPD );
                    }
                }
                else if ( optIPD != -1 && iFixedWidth != optIPD )
                {
                    FonetDriver.ActiveDriver.FireFonetWarning(
                        "Sum of fixed column widths " + iFixedWidth +
                            " differs from specified optimum IPD " + optIPD );
                }
            }
            var offset = 0;
            foreach ( TableColumn c in columns )
            {
                if ( c != null )
                {
                    c.SetColumnOffset( offset );
                    Length l = c.GetColumnWidthAsLength();
                    if ( dUnitLength > 0 )
                        l.ResolveTableUnit( dUnitLength );
                    int colWidth = l.MValue();
                    if ( colWidth <= 0 )
                    {
                        FonetDriver.ActiveDriver.FireFonetWarning(
                            "Zero-width table column!" );
                    }
                    if ( dWidthFactor > 0.0 )
                        colWidth = (int)( colWidth * dWidthFactor );
                    c.SetColumnWidth( colWidth );
                    offset += colWidth;
                }
            }
            return offset;
        }

        private void layoutColumns( Area tableArea )
        {
            foreach ( TableColumn c in columns )
            {
                if ( c != null )
                    c.Layout( tableArea );
            }
        }

        public int GetAreaHeight()
        {
            return areaContainer.GetHeight();
        }

        public override int GetContentWidth()
        {
            if ( areaContainer != null )
                return areaContainer.getContentWidth();
            return 0;
        }

        private void SetIPD( bool bHasProportionalUnits, int maxAllocIPD )
        {
            bool bMaxIsSpecified = !ipd.GetMaximum().GetLength().IsAuto();
            if ( bMaxIsSpecified )
                maxIPD = ipd.GetMaximum().GetLength().MValue();
            else
                maxIPD = maxAllocIPD;

            if ( ipd.GetOptimum().GetLength().IsAuto() )
                optIPD = -1;
            else
                optIPD = ipd.GetMaximum().GetLength().MValue();
            if ( ipd.GetMinimum().GetLength().IsAuto() )
                minIPD = -1;
            else
                minIPD = ipd.GetMinimum().GetLength().MValue();
            if ( bHasProportionalUnits && optIPD < 0 )
            {
                if ( minIPD > 0 )
                {
                    if ( bMaxIsSpecified )
                        optIPD = ( minIPD + maxIPD ) / 2;
                    else
                        optIPD = minIPD;
                }
                else if ( bMaxIsSpecified )
                    optIPD = maxIPD;
                else
                {
                    FonetDriver.ActiveDriver.FireFonetError(
                        "At least one of minimum, optimum, or maximum " +
                            "IPD must be specified on table." );
                    optIPD = maxIPD;
                }
            }
        }

        internal new class Maker : FObj.Maker
        {
            public override FObj Make( FObj parent, PropertyList propertyList )
            {
                return new Table( parent, propertyList );
            }
        }
    }
}