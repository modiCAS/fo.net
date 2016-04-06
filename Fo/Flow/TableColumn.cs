using Fonet.DataTypes;
using Fonet.Fo.Properties;
using Fonet.Layout;

namespace Fonet.Fo.Flow
{
    internal class TableColumn : FObj
    {
        private AreaContainer _areaContainer;
        private int _columnOffset;
        private int _columnWidth;
        private Length _columnWidthPropVal;
        private int _iColumnNumber;
        private int _numColumnsRepeated;
        private bool _setup;

        public TableColumn( FObj parent, PropertyList propertyList )
            : base( parent, propertyList )
        {
            Name = "fo:table-column";
        }

        public new static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        public Length GetColumnWidthAsLength()
        {
            return _columnWidthPropVal;
        }

        public int GetColumnWidth()
        {
            return _columnWidth;
        }

        public void SetColumnWidth( int columnWidth )
        {
            this._columnWidth = columnWidth;
        }

        public int GetColumnNumber()
        {
            return _iColumnNumber;
        }

        public int GetNumColumnsRepeated()
        {
            return _numColumnsRepeated;
        }

        public void DoSetup( Area area )
        {
            BorderAndPadding bap = PropMgr.GetBorderAndPadding();
            BackgroundProps bProps = PropMgr.GetBackgroundProps();

            _iColumnNumber =
                Properties.GetProperty( "column-number" ).GetNumber().IntValue();

            _numColumnsRepeated =
                Properties.GetProperty( "number-columns-repeated" ).GetNumber().IntValue();

            _columnWidthPropVal =
                Properties.GetProperty( "column-width" ).GetLength();

            _columnWidth = _columnWidthPropVal.MValue();

            string id = Properties.GetProperty( "id" ).GetString();
            area.getIDReferences().InitializeID( id, area );

            _setup = true;
        }

        public override Status Layout( Area area )
        {
            if ( Marker == MarkerBreakAfter )
                return new Status( Status.Ok );

            if ( Marker == MarkerStart )
            {
                if ( !_setup )
                    DoSetup( area );
            }
            if ( _columnWidth > 0 )
            {
                _areaContainer =
                    new AreaContainer( PropMgr.GetFontState( area.getFontInfo() ),
                        _columnOffset, 0, _columnWidth,
                        area.getContentHeight(), Position.Relative );
                _areaContainer.foCreator = this;
                _areaContainer.setPage( area.getPage() );
                _areaContainer.setBorderAndPadding( PropMgr.GetBorderAndPadding() );
                _areaContainer.setBackground( PropMgr.GetBackgroundProps() );
                _areaContainer.SetHeight( area.GetHeight() );
                area.addChild( _areaContainer );
            }
            return new Status( Status.Ok );
        }

        public void SetColumnOffset( int columnOffset )
        {
            this._columnOffset = columnOffset;
        }

        public void SetHeight( int height )
        {
            if ( _areaContainer != null )
            {
                _areaContainer.setMaxHeight( height );
                _areaContainer.SetHeight( height );
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