using System;
using System.Collections;
using Fonet.DataTypes;
using Fonet.Fo;
using Fonet.Fo.Flow;
using Fonet.Fo.Properties;
using Fonet.Render.Pdf;

namespace Fonet.Layout
{
    internal class BodyAreaContainer : Area
    {
        private bool _isNewSpanArea;
        private readonly AreaContainer _beforeFloatReferenceArea;
        private readonly int _columnCount;
        private readonly int _columnGap;
        private readonly AreaContainer _footnoteReferenceArea;
        private int _footnoteState;
        private int _footnoteYPosition;
        private readonly AreaContainer _mainReferenceArea;
        private readonly int _mainYPosition = 0;
        private readonly int _position;
        private int _xPosition;
        private int _yPosition;

        public BodyAreaContainer( FontState fontState, int xPosition,
            int yPosition, int allocationWidth,
            int maxHeight, int position, int columnCount,
            int columnGap )
            : base( fontState, allocationWidth, maxHeight )
        {
            this._xPosition = xPosition;
            this._yPosition = yPosition;
            this._position = position;
            this._columnCount = columnCount;
            this._columnGap = columnGap;

            var beforeFloatRefAreaHeight = 0;
            var footnoteRefAreaHeight = 0;
            int mainRefAreaHeight = maxHeight - beforeFloatRefAreaHeight
                - footnoteRefAreaHeight;
            _beforeFloatReferenceArea = new AreaContainer( fontState, xPosition,
                yPosition, allocationWidth, beforeFloatRefAreaHeight,
                Position.Absolute );
            _beforeFloatReferenceArea.SetAreaName( "before-float-reference-area" );
            AddChild( _beforeFloatReferenceArea );
            _mainReferenceArea = new AreaContainer( fontState, xPosition,
                yPosition, allocationWidth,
                mainRefAreaHeight,
                Position.Absolute );
            _mainReferenceArea.SetAreaName( "main-reference-area" );
            AddChild( _mainReferenceArea );
            int footnoteRefAreaYPosition = yPosition - mainRefAreaHeight;
            _footnoteReferenceArea = new AreaContainer( fontState, xPosition,
                footnoteRefAreaYPosition,
                allocationWidth,
                footnoteRefAreaHeight,
                Position.Absolute );
            _footnoteReferenceArea.SetAreaName( "footnote-reference-area" );
            AddChild( _footnoteReferenceArea );
        }

        public override void Render( PdfRenderer renderer )
        {
            renderer.RenderBodyAreaContainer( this );
        }

        public int GetPosition()
        {
            return _position;
        }

        public int GetXPosition()
        {
            return _xPosition + GetPaddingLeft() + GetBorderLeftWidth();
        }

        public void SetXPosition( int value )
        {
            _xPosition = value;
        }

        public int GetYPosition()
        {
            return _yPosition + GetPaddingTop() + GetBorderTopWidth();
        }

        public void SetYPosition( int value )
        {
            _yPosition = value;
        }

        public AreaContainer GetMainReferenceArea()
        {
            return _mainReferenceArea;
        }

        public AreaContainer GetBeforeFloatReferenceArea()
        {
            return _beforeFloatReferenceArea;
        }

        public AreaContainer GetFootnoteReferenceArea()
        {
            return _footnoteReferenceArea;
        }

        public override void SetIDReferences( IDReferences idReferences )
        {
            _mainReferenceArea.SetIDReferences( idReferences );
        }

        public override IDReferences GetIDReferences()
        {
            return _mainReferenceArea.GetIDReferences();
        }

        public AreaContainer GetNextArea( FObj fo )
        {
            _isNewSpanArea = false;

            int span = Span.None;
            if ( fo is Block )
                span = ( (Block)fo ).GetSpan();
            else if ( fo is BlockContainer )
                span = ( (BlockContainer)fo ).GetSpan();

            if ( _mainReferenceArea.GetChildren().Count == 0 )
            {
                if ( span == Span.All )
                    return AddSpanArea( 1 );
                return AddSpanArea( _columnCount );
            }

            ArrayList spanAreas = _mainReferenceArea.GetChildren();
            var spanArea = (SpanArea)spanAreas[ spanAreas.Count - 1 ];

            if ( span == Span.All && spanArea.GetColumnCount() == 1 )
                return spanArea.GetCurrentColumnArea();
            if ( span == Span.None
                && spanArea.GetColumnCount() == _columnCount )
                return spanArea.GetCurrentColumnArea();
            if ( span == Span.All )
                return AddSpanArea( 1 );
            if ( span == Span.None )
                return AddSpanArea( _columnCount );
            throw new FonetException( "BodyAreaContainer::getNextArea(): Span attribute messed up" );
        }

        private AreaContainer AddSpanArea( int numColumns )
        {
            ResetHeights();
            int spanAreaYPosition = GetYPosition()
                - _mainReferenceArea.GetContentHeight();

            var spanArea = new SpanArea( FontState, GetXPosition(),
                spanAreaYPosition, AllocationWidth,
                GetRemainingHeight(), numColumns,
                _columnGap );
            _mainReferenceArea.AddChild( spanArea );
            spanArea.SetPage( GetPage() );
            _isNewSpanArea = true;
            return spanArea.GetCurrentColumnArea();
        }

        public bool IsBalancingRequired( FObj fo )
        {
            if ( _mainReferenceArea.GetChildren().Count == 0 )
                return false;

            ArrayList spanAreas = _mainReferenceArea.GetChildren();
            var spanArea = (SpanArea)spanAreas[ spanAreas.Count - 1 ];

            if ( spanArea.IsBalanced() )
                return false;

            int span = Span.None;
            if ( fo is Block )
                span = ( (Block)fo ).GetSpan();
            else if ( fo is BlockContainer )
                span = ( (BlockContainer)fo ).GetSpan();

            if ( span == Span.All && spanArea.GetColumnCount() == 1 )
                return false;
            if ( span == Span.None
                && spanArea.GetColumnCount() == _columnCount )
                return false;
            if ( span == Span.All )
                return true;
            if ( span == Span.None )
                return false;
            return false;
        }

        public void ResetSpanArea()
        {
            ArrayList spanAreas = _mainReferenceArea.GetChildren();
            var spanArea = (SpanArea)spanAreas[ spanAreas.Count - 1 ];

            if ( !spanArea.IsBalanced() )
            {
                int newHeight = spanArea.GetTotalContentHeight()
                    / spanArea.GetColumnCount();
                newHeight += 2 * 15600;

                _mainReferenceArea.RemoveChild( spanArea );
                ResetHeights();
                var newSpanArea = new SpanArea( FontState, GetXPosition(),
                    spanArea.GetYPosition(),
                    AllocationWidth, newHeight,
                    spanArea.GetColumnCount(),
                    _columnGap );
                _mainReferenceArea.AddChild( newSpanArea );
                newSpanArea.SetPage( GetPage() );
                newSpanArea.SetIsBalanced();
                _isNewSpanArea = true;
            }
            else
                throw new Exception( "Trying to balance balanced area" );
        }

        public int GetRemainingHeight()
        {
            return _mainReferenceArea.GetMaxHeight()
                - _mainReferenceArea.GetContentHeight();
        }

        private void ResetHeights()
        {
            var totalHeight = 0;
            foreach ( SpanArea spanArea in _mainReferenceArea.GetChildren() )
            {
                int spanContentHeight = spanArea.GetMaxContentHeight();
                int spanMaxHeight = spanArea.GetMaxHeight();

                totalHeight += spanContentHeight < spanMaxHeight
                    ? spanContentHeight
                    : spanMaxHeight;
            }
            _mainReferenceArea.SetHeight( totalHeight );
        }

        public bool IsLastColumn()
        {
            ArrayList spanAreas = _mainReferenceArea.GetChildren();
            var spanArea = (SpanArea)spanAreas[ spanAreas.Count - 1 ];
            return spanArea.IsLastColumn();
        }

        public bool IsNewSpanArea()
        {
            return _isNewSpanArea;
        }

        public AreaContainer GetCurrentColumnArea()
        {
            ArrayList spanAreas = _mainReferenceArea.GetChildren();
            var spanArea = (SpanArea)spanAreas[ spanAreas.Count - 1 ];
            return spanArea.GetCurrentColumnArea();
        }

        public int GetFootnoteState()
        {
            return _footnoteState;
        }

        public bool NeedsFootnoteAdjusting()
        {
            _footnoteYPosition = _footnoteReferenceArea.GetYPosition();
            switch ( _footnoteState )
            {
            case 0:
                ResetHeights();
                if ( _footnoteReferenceArea.GetHeight() > 0
                    && _mainYPosition + _mainReferenceArea.GetHeight()
                        > _footnoteYPosition )
                    return true;
                break;
            case 1:
                break;
            }
            return false;
        }

        public void AdjustFootnoteArea()
        {
            _footnoteState++;
            if ( _footnoteState == 1 )
            {
                _mainReferenceArea.SetMaxHeight( _footnoteReferenceArea.GetYPosition()
                    - _mainYPosition );
                _footnoteYPosition = _footnoteReferenceArea.GetYPosition();
                _footnoteReferenceArea.SetMaxHeight( _footnoteReferenceArea.GetHeight() );

                foreach ( object obj in _footnoteReferenceArea.GetChildren() )
                {
                    if ( obj is Area )
                    {
                        var childArea = (Area)obj;
                        _footnoteReferenceArea.RemoveChild( childArea );
                    }
                }

                GetPage().SetPendingFootnotes( null );
            }
        }

        protected static void ResetMaxHeight( Area ar, int change )
        {
            ar.SetMaxHeight( change );
            foreach ( object obj in ar.GetChildren() )
            {
                if ( obj is Area )
                {
                    var childArea = (Area)obj;
                    ResetMaxHeight( childArea, change );
                }
            }
        }
    }
}