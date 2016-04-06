using System.Collections;

namespace Fonet.Fo.Pagination
{
    internal class PageSequenceMaster : FObj
    {
        private readonly LayoutMasterSet layoutMasterSet;

        private readonly ArrayList subSequenceSpecifiers;

        protected PageSequenceMaster( FObj parent, PropertyList propertyList )
            : base( parent, propertyList )
        {
            name = "fo:page-sequence-master";

            subSequenceSpecifiers = new ArrayList();

            if ( parent.GetName().Equals( "fo:layout-master-set" ) )
            {
                layoutMasterSet = (LayoutMasterSet)parent;
                string pm = properties.GetProperty( "master-name" ).GetString();
                if ( pm == null )
                {
                    FonetDriver.ActiveDriver.FireFonetWarning(
                        "page-sequence-master does not have a page-master-name and so is being ignored" );
                }
                else
                    layoutMasterSet.addPageSequenceMaster( pm, this );
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

        protected internal void AddSubsequenceSpecifier( SubSequenceSpecifier pageMasterReference )
        {
            subSequenceSpecifiers.Add( pageMasterReference );
        }

        protected internal SubSequenceSpecifier getSubSequenceSpecifier( int sequenceNumber )
        {
            if ( sequenceNumber >= 0
                && sequenceNumber < GetSubSequenceSpecifierCount() )
                return (SubSequenceSpecifier)subSequenceSpecifiers[ sequenceNumber ];
            return null;
        }

        protected internal int GetSubSequenceSpecifierCount()
        {
            return subSequenceSpecifiers.Count;
        }

        public void Reset()
        {
            foreach ( SubSequenceSpecifier s in subSequenceSpecifiers )
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