namespace Fonet.Layout
{
    internal class PageMaster
    {
        private RegionArea after;
        private RegionArea before;
        private BodyRegionArea body;
        private RegionArea end;
        private readonly int height;
        private RegionArea start;
        private readonly int width;

        public PageMaster( int pageWidth, int pageHeight )
        {
            width = pageWidth;
            height = pageHeight;
        }

        public void addAfter( RegionArea region )
        {
            after = region;
        }

        public void addBefore( RegionArea region )
        {
            before = region;
        }

        public void addBody( BodyRegionArea region )
        {
            body = region;
        }

        public void addEnd( RegionArea region )
        {
            end = region;
        }

        public void addStart( RegionArea region )
        {
            start = region;
        }

        public int GetHeight()
        {
            return height;
        }

        public int getWidth()
        {
            return width;
        }

        public Page makePage( AreaTree areaTree )
        {
            var p = new Page( areaTree, height, width );
            if ( body != null )
                p.addBody( body.makeBodyAreaContainer() );
            if ( before != null )
                p.addBefore( before.makeAreaContainer() );
            if ( after != null )
                p.addAfter( after.makeAreaContainer() );
            if ( start != null )
                p.addStart( start.makeAreaContainer() );
            if ( end != null )
                p.addEnd( end.makeAreaContainer() );

            return p;
        }
    }
}