using Fonet.Fo.Properties;
using Fonet.Render.Pdf;

namespace Fonet.Layout
{
    internal class SpanArea : AreaContainer
    {
        private bool _isBalanced;
        private readonly int _columnCount;
        private int _columnGap;
        private int _currentColumn = 1;

        public SpanArea( FontState fontState, int xPosition, int yPosition,
            int allocationWidth, int maxHeight, int columnCount,
            int columnGap ) :
                base( fontState, xPosition, yPosition, allocationWidth, maxHeight,
                    Position.Absolute )
        {
            ContentRectangleWidth = allocationWidth;
            this._columnCount = columnCount;
            this._columnGap = columnGap;

            int columnWidth = ( allocationWidth - columnGap * ( columnCount - 1 ) )
                / columnCount;
            for ( var columnIndex = 0; columnIndex < columnCount; columnIndex++ )
            {
                int colXPosition = xPosition
                    + columnIndex * ( columnWidth + columnGap );
                int colYPosition = yPosition;
                var colArea = new ColumnArea( fontState, colXPosition,
                    colYPosition, columnWidth,
                    maxHeight, columnCount );
                AddChild( colArea );
                colArea.SetColumnIndex( columnIndex + 1 );
            }
        }

        public override void Render( PdfRenderer renderer )
        {
            renderer.RenderSpanArea( this );
        }

        public override void End()
        {
        }

        public override void Start()
        {
        }

        public override int SpaceLeft()
        {
            return MaxHeight - CurrentHeight;
        }

        public int GetColumnCount()
        {
            return _columnCount;
        }

        public int GetCurrentColumn()
        {
            return _currentColumn;
        }

        public void SetCurrentColumn( int currentColumn )
        {
            if ( currentColumn <= _columnCount )
                this._currentColumn = currentColumn;
            else
                this._currentColumn = _columnCount;
        }

        public AreaContainer GetCurrentColumnArea()
        {
            return (AreaContainer)GetChildren()[ _currentColumn - 1 ];
        }

        public bool IsBalanced()
        {
            return _isBalanced;
        }

        public void SetIsBalanced()
        {
            _isBalanced = true;
        }

        public int GetTotalContentHeight()
        {
            var totalContentHeight = 0;
            foreach ( AreaContainer ac in GetChildren() )
                totalContentHeight += ac.GetContentHeight();
            return totalContentHeight;
        }

        public int GetMaxContentHeight()
        {
            var maxContentHeight = 0;
            foreach ( AreaContainer nextElm in GetChildren() )
            {
                if ( nextElm.GetContentHeight() > maxContentHeight )
                    maxContentHeight = nextElm.GetContentHeight();
            }
            return maxContentHeight;
        }

        public override void SetPage( Page page )
        {
            this.Page = page;
            foreach ( AreaContainer ac in GetChildren() )
                ac.SetPage( page );
        }

        public bool IsLastColumn()
        {
            return _currentColumn == _columnCount;
        }
    }
}