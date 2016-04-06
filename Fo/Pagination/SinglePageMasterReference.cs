namespace Fonet.Fo.Pagination
{
    internal class SinglePageMasterReference : PageMasterReference, SubSequenceSpecifier
    {
        private const int FIRST = 0;

        private const int DONE = 1;

        private int state;

        public SinglePageMasterReference(
            FObj parent, PropertyList propertyList )
            : base( parent, propertyList )
        {
            state = FIRST;
        }

        public override string GetNextPageMaster( int currentPageNumber,
            bool thisIsFirstPage,
            bool isEmptyPage )
        {
            if ( state == FIRST )
            {
                state = DONE;
                return MasterName;
            }
            return null;
        }

        public override void Reset()
        {
            state = FIRST;
        }

        public new static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        protected override string GetElementName()
        {
            return "fo:single-page-master-reference";
        }

        internal new class Maker : FObj.Maker
        {
            public override FObj Make( FObj parent, PropertyList propertyList )
            {
                return new SinglePageMasterReference( parent, propertyList );
            }
        }
    }
}