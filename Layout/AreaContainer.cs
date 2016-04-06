using Fonet.Render.Pdf;

namespace Fonet.Layout
{
    internal class AreaContainer : Area
    {
        private string _areaName;
        private readonly int _position;
        private int _xPosition;
        private int _yPosition;

        public AreaContainer( FontState fontState, int xPosition, int yPosition,
            int allocationWidth, int maxHeight, int position )
            : base( fontState, allocationWidth, maxHeight )
        {
            _xPosition = xPosition;
            _yPosition = yPosition;
            _position = position;
        }

        public override void Render( PdfRenderer renderer )
        {
            renderer.RenderAreaContainer( this );
        }

        public int GetPosition()
        {
            return _position;
        }

        public int GetXPosition()
        {
            return _xPosition;
        }

        public void SetXPosition( int value )
        {
            _xPosition = value;
        }

        public int GetYPosition()
        {
            return _yPosition;
        }

        public int GetCurrentYPosition()
        {
            return _yPosition;
        }

        public void SetYPosition( int value )
        {
            _yPosition = value;
        }

        public void ShiftYPosition( int value )
        {
            _yPosition += value;
        }

        public string GetAreaName()
        {
            return _areaName;
        }

        public void SetAreaName( string areaName )
        {
            _areaName = areaName;
        }
    }
}