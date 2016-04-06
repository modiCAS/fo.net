using System.Collections;

namespace Fonet.Fo.Pagination
{
    internal class PageSequenceMaster : FObj
    {
        private readonly ArrayList _subSequenceSpecifiers;

        protected PageSequenceMaster( FObj parent, PropertyList propertyList )
            : base( parent, propertyList )
        {
            Name = "fo:page-sequence-master";

            _subSequenceSpecifiers = new ArrayList();

            if ( parent.GetName().Equals( "fo:layout-master-set" ) )
            {
                var layoutMasterSet = (LayoutMasterSet)parent;
                string pm = Properties.GetProperty( "master-name" ).GetString();
                if ( pm == null )
                {
                    FonetDriver.ActiveDriver.FireFonetWarning(
                        "page-sequence-master does not have a page-master-name and so is being ignored" );
                }
                else
                    layoutMasterSet.AddPageSequenceMaster( pm, this );
            }
            else
            {
                throw new FonetException( "fo:page-sequence-master must be child "
                    + "of fo:layout-master-set, not "
                    + parent.GetName() );
            }
        }

        public new static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        protected internal void AddSubsequenceSpecifier( ISubSequenceSpecifier pageMasterReference )
        {
            _subSequenceSpecifiers.Add( pageMasterReference );
        }

        protected internal ISubSequenceSpecifier GetSubSequenceSpecifier( int sequenceNumber )
        {
            if ( sequenceNumber >= 0
                && sequenceNumber < GetSubSequenceSpecifierCount() )
                return (ISubSequenceSpecifier)_subSequenceSpecifiers[ sequenceNumber ];
            return null;
        }

        protected internal int GetSubSequenceSpecifierCount()
        {
            return _subSequenceSpecifiers.Count;
        }

        public void Reset()
        {
            foreach ( ISubSequenceSpecifier s in _subSequenceSpecifiers )
                s.Reset();
        }

        internal new class Maker : FObj.Maker
        {
            public override FObj Make( FObj parent, PropertyList propertyList )
            {
                return new PageSequenceMaster( parent, propertyList );
            }
        }
    }
}