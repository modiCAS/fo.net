using Fonet.Fo.Properties;
using Fonet.Layout;

namespace Fonet.Fo.Flow
{
    internal class BlockContainer : FObj
    {
        private AreaContainer _areaContainer;
        private int _bottom;
        private int _height;
        private int _left;
        private int _position;
        private int _right;
        private int _span;
        private int _top;
        private int _width;

        protected BlockContainer( FObj parent, PropertyList propertyList )
            : base( parent, propertyList )
        {
            Name = "fo:block-container";
            _span = Properties.GetProperty( "span" ).GetEnum();
        }

        public new static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        public override Status Layout( Area area )
        {
            if ( Marker == MarkerStart )
            {
                AbsolutePositionProps mAbsProps = PropMgr.GetAbsolutePositionProps();
                BorderAndPadding bap = PropMgr.GetBorderAndPadding();
                BackgroundProps bProps = PropMgr.GetBackgroundProps();
                MarginProps mProps = PropMgr.GetMarginProps();

                Marker = 0;
                _position = Properties.GetProperty( "position" ).GetEnum();
                _top = Properties.GetProperty( "top" ).GetLength().MValue();
                _bottom = Properties.GetProperty( "bottom" ).GetLength().MValue();
                _left = Properties.GetProperty( "left" ).GetLength().MValue();
                _right = Properties.GetProperty( "right" ).GetLength().MValue();
                _width = Properties.GetProperty( "width" ).GetLength().MValue();
                _height = Properties.GetProperty( "height" ).GetLength().MValue();
                _span = Properties.GetProperty( "span" ).GetEnum();

                string id = Properties.GetProperty( "id" ).GetString();
                area.getIDReferences().InitializeID( id, area );
            }

            var container = (AreaContainer)area;
            if ( _width == 0 && _height == 0 )
            {
                _width = _right - _left;
                _height = _bottom - _top;
            }

            _areaContainer =
                new AreaContainer( PropMgr.GetFontState( container.getFontInfo() ),
                    container.getXPosition() + _left,
                    container.GetYPosition() - _top, _width, _height,
                    _position );

            _areaContainer.setPage( area.getPage() );
            _areaContainer.setBackground( PropMgr.GetBackgroundProps() );
            _areaContainer.setBorderAndPadding( PropMgr.GetBorderAndPadding() );
            _areaContainer.start();

            _areaContainer.setAbsoluteHeight( 0 );
            _areaContainer.setIDReferences( area.getIDReferences() );

            int numChildren = Children.Count;
            for ( int i = Marker; i < numChildren; i++ )
            {
                var fo = (FObj)Children[ i ];
                Status status = fo.Layout( _areaContainer );
            }

            _areaContainer.end();
            if ( _position == Position.Absolute )
                _areaContainer.SetHeight( _height );
            area.addChild( _areaContainer );

            return new Status( Status.Ok );
        }

        public override int GetContentWidth()
        {
            if ( _areaContainer != null )
                return _areaContainer.getContentWidth();
            return 0;
        }

        public override bool GeneratesReferenceAreas()
        {
            return true;
        }

        public int GetSpan()
        {
            return _span;
        }

        internal new class Maker : FObj.Maker
        {
            public override FObj Make( FObj parent, PropertyList propertyList )
            {
                return new BlockContainer( parent, propertyList );
            }
        }
    }
}