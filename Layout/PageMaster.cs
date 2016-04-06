namespace Fonet.Layout
{
    internal class PageMaster
    {
        private RegionArea _after;
        private RegionArea _before;
        private BodyRegionArea _body;
        private RegionArea _end;
        private readonly int _height;
        private RegionArea _start;
        private readonly int _width;

        public PageMaster( int pageWidth, int pageHeight )
        {
            _width = pageWidth;
            _height = pageHeight;
        }

        public void AddAfter( RegionArea region )
        {
            _after = region;
        }

        public void AddBefore( RegionArea region )
        {
            _before = region;
        }

        public void AddBody( BodyRegionArea region )
        {
            _body = region;
        }

        public void AddEnd( RegionArea region )
        {
            _end = region;
        }

        public void AddStart( RegionArea region )
        {
            _start = region;
        }

        public int GetHeight()
        {
            return _height;
        }

        public int GetWidth()
        {
            return _width;
        }

        public Page MakePage( AreaTree areaTree )
        {
            var p = new Page( areaTree, _height, _width );
            if ( _body != null )
                p.AddBody( _body.MakeBodyAreaContainer() );
            if ( _before != null )
                p.AddBefore( _before.MakeAreaContainer() );
            if ( _after != null )
                p.AddAfter( _after.MakeAreaContainer() );
            if ( _start != null )
                p.AddStart( _start.MakeAreaContainer() );
            if ( _end != null )
                p.AddEnd( _end.MakeAreaContainer() );

            return p;
        }
    }
}