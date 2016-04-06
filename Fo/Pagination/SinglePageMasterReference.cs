namespace Fonet.Fo.Pagination
{
    internal class SinglePageMasterReference : PageMasterReference, ISubSequenceSpecifier
    {
        private const int First = 0;

        private const int Done = 1;

        private int _state;

        public SinglePageMasterReference(
            FObj parent, PropertyList propertyList )
            : base( parent, propertyList )
        {
            _state = First;
        }

        public override string GetNextPageMaster( int currentPageNumber,
            bool thisIsFirstPage,
            bool isEmptyPage )
        {
            if ( _state == First )
            {
                _state = Done;
                return MasterName;
            }
            return null;
        }

        public override void Reset()
        {
            _state = First;
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