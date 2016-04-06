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
        private readonly int beforeFloatRefAreaHeight;
        private readonly AreaContainer beforeFloatReferenceArea;
        private readonly int columnCount;
        private readonly int columnGap;
        private readonly int footnoteRefAreaHeight;
        private readonly AreaContainer footnoteReferenceArea;
        private int footnoteState;
        private int footnoteYPosition;
        private readonly int mainRefAreaHeight;
        private readonly AreaContainer mainReferenceArea;
        private readonly int mainYPosition = 0;
        private readonly int position;
        private int xPosition;
        private int yPosition;

        public BodyAreaContainer( FontState fontState, int xPosition,
            int yPosition, int allocationWidth,
            int maxHeight, int position, int columnCount,
            int columnGap )
            : base( fontState, allocationWidth, maxHeight )
        {
            this.xPosition = xPosition;
            this.yPosition = yPosition;
            this.position = position;
            this.columnCount = columnCount;
            this.columnGap = columnGap;

            beforeFloatRefAreaHeight = 0;
            footnoteRefAreaHeight = 0;
            mainRefAreaHeight = maxHeight - beforeFloatRefAreaHeight
                - footnoteRefAreaHeight;
            beforeFloatReferenceArea = new AreaContainer( fontState, xPosition,
                yPosition, allocationWidth, beforeFloatRefAreaHeight,
                Position.Absolute );
            beforeFloatReferenceArea.setAreaName( "before-float-reference-area" );
            addChild( beforeFloatReferenceArea );
            mainReferenceArea = new AreaContainer( fontState, xPosition,
                yPosition, allocationWidth,
                mainRefAreaHeight,
                Position.Absolute );
            mainReferenceArea.setAreaName( "main-reference-area" );
            addChild( mainReferenceArea );
            int footnoteRefAreaYPosition = yPosition - mainRefAreaHeight;
            footnoteReferenceArea = new AreaContainer( fontState, xPosition,
                footnoteRefAreaYPosition,
                allocationWidth,
                footnoteRefAreaHeight,
                Position.Absolute );
            footnoteReferenceArea.setAreaName( "footnote-reference-area" );
            addChild( footnoteReferenceArea );
        }

        public override void render( PdfRenderer renderer )
        {
            renderer.RenderBodyAreaContainer( this );
        }

        public int getPosition()
        {
            return position;
        }

        public int getXPosition()
        {
            return xPosition + getPaddingLeft() + getBorderLeftWidth();
        }

        public void setXPosition( int value )
        {
            xPosition = value;
        }

        public int GetYPosition()
        {
            return yPosition + getPaddingTop() + getBorderTopWidth();
        }

        public void setYPosition( int value )
        {
            yPosition = value;
        }

        public AreaContainer getMainReferenceArea()
        {
            return mainReferenceArea;
        }

        public AreaContainer getBeforeFloatReferenceArea()
        {
            return beforeFloatReferenceArea;
        }

        public AreaContainer getFootnoteReferenceArea()
        {
            return footnoteReferenceArea;
        }

        public override void setIDReferences( IDReferences idReferences )
        {
            mainReferenceArea.setIDReferences( idReferences );
        }

        public override IDReferences getIDReferences()
        {
            return mainReferenceArea.getIDReferences();
        }

        public AreaContainer getNextArea( FObj fo )
        {
            _isNewSpanArea = false;

            int span = Span.None;
            if ( fo is Block )
                span = ( (Block)fo ).GetSpan();
            else if ( fo is BlockContainer )
                span = ( (BlockContainer)fo ).GetSpan();

            if ( mainReferenceArea.getChildren().Count == 0 )
            {
                if ( span == Span.All )
                    return addSpanArea( 1 );
                return addSpanArea( columnCount );
            }

            ArrayList spanAreas = mainReferenceArea.getChildren();
            var spanArea = (SpanArea)spanAreas[ spanAreas.Count - 1 ];

            if ( span == Span.All && spanArea.getColumnCount() == 1 )
                return spanArea.getCurrentColumnArea();
            if ( span == Span.None
                && spanArea.getColumnCount() == columnCount )
                return spanArea.getCurrentColumnArea();
            if ( span == Span.All )
                return addSpanArea( 1 );
            if ( span == Span.None )
                return addSpanArea( columnCount );
            throw new FonetException( "BodyAreaContainer::getNextArea(): Span attribute messed up" );
        }

        private AreaContainer addSpanArea( int numColumns )
        {
            resetHeights();
            int spanAreaYPosition = GetYPosition()
                - mainReferenceArea.getContentHeight();

            var spanArea = new SpanArea( fontState, getXPosition(),
                spanAreaYPosition, allocationWidth,
                GetRemainingHeight(), numColumns,
                columnGap );
            mainReferenceArea.addChild( spanArea );
            spanArea.setPage( getPage() );
            _isNewSpanArea = true;
            return spanArea.getCurrentColumnArea();
        }

        public bool isBalancingRequired( FObj fo )
        {
            if ( mainReferenceArea.getChildren().Count == 0 )
                return false;

            ArrayList spanAreas = mainReferenceArea.getChildren();
            var spanArea = (SpanArea)spanAreas[ spanAreas.Count - 1 ];

            if ( spanArea.isBalanced() )
                return false;

            int span = Span.None;
            if ( fo is Block )
                span = ( (Block)fo ).GetSpan();
            else if ( fo is BlockContainer )
                span = ( (BlockContainer)fo ).GetSpan();

            if ( span == Span.All && spanArea.getColumnCount() == 1 )
                return false;
            if ( span == Span.None
                && spanArea.getColumnCount() == columnCount )
                return false;
            if ( span == Span.All )
                return true;
            if ( span == Span.None )
                return false;
            return false;
        }

        public void resetSpanArea()
        {
            ArrayList spanAreas = mainReferenceArea.getChildren();
            var spanArea = (SpanArea)spanAreas[ spanAreas.Count - 1 ];

            if ( !spanArea.isBalanced() )
            {
                int newHeight = spanArea.getTotalContentHeight()
                    / spanArea.getColumnCount();
                newHeight += 2 * 15600;

                mainReferenceArea.removeChild( spanArea );
                resetHeights();
                var newSpanArea = new SpanArea( fontState, getXPosition(),
                    spanArea.GetYPosition(),
                    allocationWidth, newHeight,
                    spanArea.getColumnCount(),
                    columnGap );
                mainReferenceArea.addChild( newSpanArea );
                newSpanArea.setPage( getPage() );
                newSpanArea.setIsBalanced();
                _isNewSpanArea = true;
            }
            else
                throw new Exception( "Trying to balance balanced area" );
        }

        public int GetRemainingHeight()
        {
            return mainReferenceArea.getMaxHeight()
                - mainReferenceArea.getContentHeight();
        }

        private void resetHeights()
        {
            var totalHeight = 0;
            foreach ( SpanArea spanArea in mainReferenceArea.getChildren() )
            {
                int spanContentHeight = spanArea.getMaxContentHeight();
                int spanMaxHeight = spanArea.getMaxHeight();

                totalHeight += spanContentHeight < spanMaxHeight
                    ? spanContentHeight
                    : spanMaxHeight;
            }
            mainReferenceArea.SetHeight( totalHeight );
        }

        public bool isLastColumn()
        {
            ArrayList spanAreas = mainReferenceArea.getChildren();
            var spanArea = (SpanArea)spanAreas[ spanAreas.Count - 1 ];
            return spanArea.isLastColumn();
        }

        public bool isNewSpanArea()
        {
            return _isNewSpanArea;
        }

        public AreaContainer getCurrentColumnArea()
        {
            ArrayList spanAreas = mainReferenceArea.getChildren();
            var spanArea = (SpanArea)spanAreas[ spanAreas.Count - 1 ];
            return spanArea.getCurrentColumnArea();
        }

        public int getFootnoteState()
        {
            return footnoteState;
        }

        public bool needsFootnoteAdjusting()
        {
            footnoteYPosition = footnoteReferenceArea.GetYPosition();
            switch ( footnoteState )
            {
            case 0:
                resetHeights();
                if ( footnoteReferenceArea.GetHeight() > 0
                    && mainYPosition + mainReferenceArea.GetHeight()
                        > footnoteYPosition )
                    return true;
                break;
            case 1:
                break;
            }
            return false;
        }

        public void adjustFootnoteArea()
        {
            footnoteState++;
            if ( footnoteState == 1 )
            {
                mainReferenceArea.setMaxHeight( footnoteReferenceArea.GetYPosition()
                    - mainYPosition );
                footnoteYPosition = footnoteReferenceArea.GetYPosition();
                footnoteReferenceArea.setMaxHeight( footnoteReferenceArea.GetHeight() );

                foreach ( object obj in footnoteReferenceArea.getChildren() )
                {
                    if ( obj is Area )
                    {
                        var childArea = (Area)obj;
                        footnoteReferenceArea.removeChild( childArea );
                    }
                }

                getPage().setPendingFootnotes( null );
            }
        }

        protected static void resetMaxHeight( Area ar, int change )
        {
            ar.setMaxHeight( change );
            foreach ( object obj in ar.getChildren() )
            {
                if ( obj is Area )
                {
                    var childArea = (Area)obj;
                    resetMaxHeight( childArea, change );
                }
            }
        }
    }
}