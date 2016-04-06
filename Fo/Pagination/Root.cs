using System.Collections;

namespace Fonet.Fo.Pagination
{
    internal class Root : FObj
    {
        private LayoutMasterSet layoutMasterSet;

        private readonly ArrayList pageSequences;

        private int runningPageNumberCounter;

        protected internal Root( FObj parent, PropertyList propertyList )
            : base( parent, propertyList )
        {
            name = "fo:root";
            pageSequences = new ArrayList();
            if ( parent != null )
                throw new FonetException( "root must be root element" );
        }

        public new static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        protected internal int getRunningPageNumberCounter()
        {
            return runningPageNumberCounter;
        }

        protected internal void setRunningPageNumberCounter( int count )
        {
            runningPageNumberCounter = count;
        }

        public int getPageSequenceCount()
        {
            return pageSequences.Count;
        }

        public PageSequence getSucceedingPageSequence( PageSequence current )
        {
            int currentIndex = pageSequences.IndexOf( current );
            if ( currentIndex == -1 )
                return null;
            if ( currentIndex < pageSequences.Count - 1 )
                return (PageSequence)pageSequences[ currentIndex + 1 ];
            return null;
        }

        public LayoutMasterSet getLayoutMasterSet()
        {
            return layoutMasterSet;
        }

        public void setLayoutMasterSet( LayoutMasterSet layoutMasterSet )
        {
            this.layoutMasterSet = layoutMasterSet;
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