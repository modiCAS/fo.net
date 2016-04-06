using Fonet.Render.Pdf;

namespace Fonet.Layout
{
    internal class AreaContainer : Area
    {
        private string areaName;
        private readonly int position;
        private int xPosition;
        private int yPosition;

        public AreaContainer( FontState fontState, int xPosition, int yPosition,
            int allocationWidth, int maxHeight, int position )
            : base( fontState, allocationWidth, maxHeight )
        {
            this.xPosition = xPosition;
            this.yPosition = yPosition;
            this.position = position;
        }

        public override void render( PdfRenderer renderer )
        {
            renderer.RenderAreaContainer( this );
        }

        public int getPosition()
        {
            return position;
        }

        public int getXPosition()
        {
            return xPosition;
        }

        public void setXPosition( int value )
        {
            xPosition = value;
        }

        public int GetYPosition()
        {
            return yPosition;
        }

        public int GetCurrentYPosition()
        {
            return yPosition;
        }

        public void setYPosition( int value )
        {
            yPosition = value;
        }

        public void shiftYPosition( int value )
        {
            yPosition += value;
        }

        public string getAreaName()
        {
            return areaName;
        }

        public void setAreaName( string areaName )
        {
            this.areaName = areaName;
        }
    }
}