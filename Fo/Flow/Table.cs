using System.Collections;
using Fonet.DataTypes;
using Fonet.Fo.Properties;
using Fonet.Layout;

namespace Fonet.Fo.Flow
{
    internal class Table : FObj
    {
        private const int Mincolwidth = 10000;

        private AreaContainer _areaContainer;

        private bool _bAutoLayout;

        private int _bodyCount;

        private int _breakAfter;

        private int _breakBefore;

        private readonly ArrayList _columns = new ArrayList();

        private int _contentWidth;

        private int _height;

        private string _id;

        private LengthRange _ipd;

        private int _maxIpd;

        private int _minIpd;

        private bool _omitFooterAtBreak;

        private bool _omitHeaderAtBreak;

        private int _optIpd;

        private int _spaceAfter;

        private int _spaceBefore;

        private TableFooter _tableFooter;

        private TableHeader _tableHeader;

        public Table( FObj parent, PropertyList propertyList )
            : base( parent, propertyList )
        {
            Name = "fo:table";
        }

        public new static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        public override Status Layout( Area area )
        {
            if ( Marker == MarkerBreakAfter )
                return new Status( Status.Ok );

            if ( Marker == MarkerStart )
            {
                AccessibilityProps mAccProps = PropMgr.GetAccessibilityProps();
                AuralProps mAurProps = PropMgr.GetAuralProps();
                BorderAndPadding bap = PropMgr.GetBorderAndPadding();
                BackgroundProps bProps = PropMgr.GetBackgroundProps();
                MarginProps mProps = PropMgr.GetMarginProps();
                RelativePositionProps mRelProps = PropMgr.GetRelativePositionProps();

                _breakBefore = Properties.GetProperty( "break-before" ).GetEnum();
                _breakAfter = Properties.GetProperty( "break-after" ).GetEnum();
                _spaceBefore =
                    Properties.GetProperty( "space-before.optimum" ).GetLength().MValue();
                _spaceAfter =
                    Properties.GetProperty( "space-after.optimum" ).GetLength().MValue();
                _ipd =
                    Properties.GetProperty( "inline-progression-dimension" ).
                        GetLengthRange();
                _height = Properties.GetProperty( "height" ).GetLength().MValue();
                _bAutoLayout = Properties.GetProperty( "table-layout" ).GetEnum() ==
                    TableLayout.Auto;

                _id = Properties.GetProperty( "id" ).GetString();

                _omitHeaderAtBreak =
                    Properties.GetProperty( "table-omit-header-at-break" ).GetEnum()
                        == GenericBoolean.Enums.True;
                _omitFooterAtBreak =
                    Properties.GetProperty( "table-omit-footer-at-break" ).GetEnum()
                        == GenericBoolean.Enums.True;

                if ( area is BlockArea )
                    area.end();
                if ( _areaContainer
                    == null )
                    area.getIDReferences().CreateID( _id );

                Marker = 0;

                if ( _breakBefore == GenericBreak.Enums.Page )
                    return new Status( Status.ForcePageBreak );

                if ( _breakBefore == GenericBreak.Enums.OddPage )
                    return new Status( Status.ForcePageBreakOdd );

                if ( _breakBefore == GenericBreak.Enums.EvenPage )
                    return new Status( Status.ForcePageBreakEven );
            }

            if ( _spaceBefore != 0 && Marker == 0 )
                area.addDisplaySpace( _spaceBefore );

            if ( Marker == 0 && _areaContainer == null )
                area.getIDReferences().ConfigureID( _id, area );

            int spaceLeft = area.spaceLeft();
            _areaContainer =
                new AreaContainer( PropMgr.GetFontState( area.getFontInfo() ), 0, 0,
                    area.getAllocationWidth(), area.spaceLeft(),
                    Position.Static );

            _areaContainer.foCreator = this;
            _areaContainer.setPage( area.getPage() );
            _areaContainer.setParent( area );
            _areaContainer.setBackground( PropMgr.GetBackgroundProps() );
            _areaContainer.setBorderAndPadding( PropMgr.GetBorderAndPadding() );
            _areaContainer.start();

            _areaContainer.setAbsoluteHeight( area.getAbsoluteHeight() );
            _areaContainer.setIDReferences( area.getIDReferences() );

            var addedHeader = false;
            var addedFooter = false;
            int numChildren = Children.Count;

            if ( _columns.Count == 0 )
            {
                FindColumns( _areaContainer );
                if ( _bAutoLayout )
                {
                    FonetDriver.ActiveDriver.FireFonetWarning(
                        "table-layout=auto is not supported, using fixed!" );
                }
                _contentWidth =
                    CalcFixedColumnWidths( _areaContainer.getAllocationWidth() );
            }
            _areaContainer.setAllocationWidth( _contentWidth );
            LayoutColumns( _areaContainer );

            for ( int i = Marker; i < numChildren; i++ )
            {
                var fo = (FoNode)Children[ i ];
                if ( fo is TableHeader )
                {
                    if ( _columns.Count == 0 )
                    {
                        FonetDriver.ActiveDriver.FireFonetWarning(
                            "Current implementation of tables requires a table-column for each column, indicating column-width" );
                        return new Status( Status.Ok );
                    }
                    _tableHeader = (TableHeader)fo;
                    _tableHeader.SetColumns( _columns );
                }
                else if ( fo is TableFooter )
                {
                    if ( _columns.Count == 0 )
                    {
                        FonetDriver.ActiveDriver.FireFonetWarning(
                            "Current implementation of tables requires a table-column for each column, indicating column-width" );
                        return new Status( Status.Ok );
                    }
                    _tableFooter = (TableFooter)fo;
                    _tableFooter.SetColumns( _columns );
                }
                else if ( fo is TableBody )
                {
                    if ( _columns.Count == 0 )
                    {
                        FonetDriver.ActiveDriver.FireFonetWarning(
                            "Current implementation of tables requires a table-column for each column, indicating column-width" );
                        return new Status( Status.Ok );
                    }
                    Status status;
                    if ( _tableHeader != null && !addedHeader )
                    {
                        if ( ( status =
                            _tableHeader.Layout( _areaContainer ) ).IsIncomplete() )
                        {
                            _tableHeader.ResetMarker();
                            return new Status( Status.AreaFullNone );
                        }
                        addedHeader = true;
                        _tableHeader.ResetMarker();
                        area.setMaxHeight( area.getMaxHeight() - spaceLeft
                            + _areaContainer.getMaxHeight() );
                    }
                    if ( _tableFooter != null && !_omitFooterAtBreak
                        && !addedFooter )
                    {
                        if ( ( status =
                            _tableFooter.Layout( _areaContainer ) ).IsIncomplete() )
                            return new Status( Status.AreaFullNone );
                        addedFooter = true;
                        _tableFooter.ResetMarker();
                    }
                    fo.SetWidows( Widows );
                    fo.SetOrphans( Orphans );
                    ( (TableBody)fo ).SetColumns( _columns );

                    if ( ( status = fo.Layout( _areaContainer ) ).IsIncomplete() )
                    {
                        Marker = i;
                        if ( _bodyCount == 0
                            && status.GetCode() == Status.AreaFullNone )
                        {
                            if ( _tableHeader != null )
                                _tableHeader.RemoveLayout( _areaContainer );
                            if ( _tableFooter != null )
                                _tableFooter.RemoveLayout( _areaContainer );
                            ResetMarker();
                        }
                        if ( _areaContainer.getContentHeight() > 0 )
                        {
                            area.addChild( _areaContainer );
                            area.increaseHeight( _areaContainer.GetHeight() );
                            if ( _omitHeaderAtBreak )
                                _tableHeader = null;
                            if ( _tableFooter != null && !_omitFooterAtBreak )
                            {
                                ( (TableBody)fo ).SetYPosition( _tableFooter.GetYPosition() );
                                _tableFooter.SetYPosition( _tableFooter.GetYPosition()
                                    + ( (TableBody)fo ).GetHeight() );
                            }
                            SetupColumnHeights();
                            status = new Status( Status.AreaFullSome );
                        }
                        return status;
                    }
                    _bodyCount++;
                    area.setMaxHeight( area.getMaxHeight() - spaceLeft
                        + _areaContainer.getMaxHeight() );
                    if ( _tableFooter != null && !_omitFooterAtBreak )
                    {
                        ( (TableBody)fo ).SetYPosition( _tableFooter.GetYPosition() );
                        _tableFooter.SetYPosition( _tableFooter.GetYPosition()
                            + ( (TableBody)fo ).GetHeight() );
                    }
                }
            }

            if ( _tableFooter != null && _omitFooterAtBreak )
            {
                if ( _tableFooter.Layout( _areaContainer ).IsIncomplete() )
                {
                    FonetDriver.ActiveDriver.FireFonetWarning(
                        "Footer could not fit on page, moving last body row to next page" );
                    area.addChild( _areaContainer );
                    area.increaseHeight( _areaContainer.GetHeight() );
                    if ( _omitHeaderAtBreak )
                        _tableHeader = null;
                    _tableFooter.RemoveLayout( _areaContainer );
                    _tableFooter.ResetMarker();
                    return new Status( Status.AreaFullSome );
                }
            }

            if ( _height != 0 )
                _areaContainer.SetHeight( _height );

            SetupColumnHeights();

            _areaContainer.end();
            area.addChild( _areaContainer );

            area.increaseHeight( _areaContainer.GetHeight() );

            if ( _spaceAfter != 0 )
                area.addDisplaySpace( _spaceAfter );

            if ( area is BlockArea )
                area.start();

            if ( _breakAfter == GenericBreak.Enums.Page )
            {
                Marker = MarkerBreakAfter;
                return new Status( Status.ForcePageBreak );
            }

            if ( _breakAfter == GenericBreak.Enums.OddPage )
            {
                Marker = MarkerBreakAfter;
                return new Status( Status.ForcePageBreakOdd );
            }

            if ( _breakAfter == GenericBreak.Enums.EvenPage )
            {
                Marker = MarkerBreakAfter;
                return new Status( Status.ForcePageBreakEven );
            }

            return new Status( Status.Ok );
        }

        protected void SetupColumnHeights()
        {
            foreach ( TableColumn c in _columns )
            {
                if ( c != null )
                    c.SetHeight( _areaContainer.getContentHeight() );
            }
        }

        private void FindColumns( Area areaContainer )
        {
            var nextColumnNumber = 1;
            foreach ( FoNode fo in Children )
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
                        if ( currentColumnNumber < _columns.Count )
                        {
                            if ( _columns[ currentColumnNumber - 1 ] != null )
                            {
                                FonetDriver.ActiveDriver.FireFonetWarning(
                                    "More than one column object assigned to column " + currentColumnNumber );
                            }
                        }
                        _columns.Insert( currentColumnNumber - 1, c );
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

            foreach ( TableColumn c in _columns )
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

            SetIpd( dTblUnits > 0.0, maxAllocationWidth );
            if ( dTblUnits > 0.0 )
            {
                var iProportionalWidth = 0;
                if ( _optIpd > iFixedWidth )
                    iProportionalWidth = _optIpd - iFixedWidth;
                else if ( _maxIpd > iFixedWidth )
                    iProportionalWidth = _maxIpd - iFixedWidth;
                else
                    iProportionalWidth = maxAllocationWidth - iFixedWidth;
                if ( iProportionalWidth > 0 )
                    dUnitLength = iProportionalWidth / dTblUnits;
                else
                {
                    FonetDriver.ActiveDriver.FireFonetWarning( string.Format(
                        "Sum of fixed column widths {0} greater than maximum available IPD {1}; no space for {2} propertional units",
                        iFixedWidth, maxAllocationWidth, dTblUnits ) );
                    dUnitLength = Mincolwidth / tuMin;
                }
            }
            else
            {
                int iTableWidth = iFixedWidth;
                if ( _minIpd > iFixedWidth )
                {
                    iTableWidth = _minIpd;
                    dWidthFactor = _minIpd / (double)iFixedWidth;
                }
                else if ( _maxIpd < iFixedWidth )
                {
                    if ( _maxIpd != 0 )
                    {
                        FonetDriver.ActiveDriver.FireFonetWarning(
                            "Sum of fixed column widths " + iFixedWidth +
                                " greater than maximum specified IPD " + _maxIpd );
                    }
                }
                else if ( _optIpd != -1 && iFixedWidth != _optIpd )
                {
                    FonetDriver.ActiveDriver.FireFonetWarning(
                        "Sum of fixed column widths " + iFixedWidth +
                            " differs from specified optimum IPD " + _optIpd );
                }
            }
            var offset = 0;
            foreach ( TableColumn c in _columns )
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

        private void LayoutColumns( Area tableArea )
        {
            foreach ( TableColumn c in _columns )
            {
                if ( c != null )
                    c.Layout( tableArea );
            }
        }

        public int GetAreaHeight()
        {
            return _areaContainer.GetHeight();
        }

        public override int GetContentWidth()
        {
            if ( _areaContainer != null )
                return _areaContainer.getContentWidth();
            return 0;
        }

        private void SetIpd( bool bHasProportionalUnits, int maxAllocIpd )
        {
            bool bMaxIsSpecified = !_ipd.GetMaximum().GetLength().IsAuto();
            if ( bMaxIsSpecified )
                _maxIpd = _ipd.GetMaximum().GetLength().MValue();
            else
                _maxIpd = maxAllocIpd;

            if ( _ipd.GetOptimum().GetLength().IsAuto() )
                _optIpd = -1;
            else
                _optIpd = _ipd.GetMaximum().GetLength().MValue();
            if ( _ipd.GetMinimum().GetLength().IsAuto() )
                _minIpd = -1;
            else
                _minIpd = _ipd.GetMinimum().GetLength().MValue();
            if ( bHasProportionalUnits && _optIpd < 0 )
            {
                if ( _minIpd > 0 )
                {
                    if ( bMaxIsSpecified )
                        _optIpd = ( _minIpd + _maxIpd ) / 2;
                    else
                        _optIpd = _minIpd;
                }
                else if ( bMaxIsSpecified )
                    _optIpd = _maxIpd;
                else
                {
                    FonetDriver.ActiveDriver.FireFonetError(
                        "At least one of minimum, optimum, or maximum " +
                            "IPD must be specified on table." );
                    _optIpd = _maxIpd;
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