using Fonet.Fo.Properties;
using Fonet.Render.Pdf;

namespace Fonet.Layout
{
    internal class ColumnArea : AreaContainer
    {
        private int _columnIndex;
        private int _maxColumns;

        public ColumnArea( FontState fontState, int xPosition, int yPosition,
            int allocationWidth, int maxHeight, int columnCount )
            : base( fontState, xPosition, yPosition,
                allocationWidth, maxHeight, Position.Absolute )
        {
            _maxColumns = columnCount;
            SetAreaName( "normal-flow-ref.-area" );
        }

        public override void Render( PdfRenderer renderer )
        {
            renderer.RenderAreaContainer( this );
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

        public int GetColumnIndex()
        {
            return _columnIndex;
        }

        public void SetColumnIndex( int columnIndex )
        {
            this._columnIndex = columnIndex;
        }

        public void IncrementSpanIndex()
        {
            var span = (SpanArea)Parent;
            span.SetCurrentColumn( span.GetCurrentColumn() + 1 );
        }
    }
}