using System.Collections;

namespace Fonet.Fo.Pagination
{
    internal class Root : FObj
    {
        private LayoutMasterSet _layoutMasterSet;

        private readonly ArrayList _pageSequences;

        private int _runningPageNumberCounter;

        protected internal Root( FObj parent, PropertyList propertyList )
            : base( parent, propertyList )
        {
            Name = "fo:root";
            _pageSequences = new ArrayList();
            if ( parent != null )
                throw new FonetException( "root must be root element" );
        }

        public new static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        protected internal int GetRunningPageNumberCounter()
        {
            return _runningPageNumberCounter;
        }

        protected internal void SetRunningPageNumberCounter( int count )
        {
            _runningPageNumberCounter = count;
        }

        public int GetPageSequenceCount()
        {
            return _pageSequences.Count;
        }

        public PageSequence GetSucceedingPageSequence( PageSequence current )
        {
            int currentIndex = _pageSequences.IndexOf( current );
            if ( currentIndex == -1 )
                return null;
            if ( currentIndex < _pageSequences.Count - 1 )
                return (PageSequence)_pageSequences[ currentIndex + 1 ];
            return null;
        }

        public LayoutMasterSet GetLayoutMasterSet()
        {
            return _layoutMasterSet;
        }

        public void SetLayoutMasterSet( LayoutMasterSet layoutMasterSet )
        {
            this._layoutMasterSet = layoutMasterSet;
        }

        internal new class Maker : FObj.Maker
        {
            public override FObj Make( FObj parent, PropertyList propertyList )
            {
                return new Root( parent, propertyList );
            }
        }
    }
}