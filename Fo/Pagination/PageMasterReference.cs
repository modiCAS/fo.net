namespace Fonet.Fo.Pagination
{
    internal abstract class PageMasterReference : FObj, ISubSequenceSpecifier
    {
        public PageMasterReference( FObj parent, PropertyList propertyList )
            : base( parent, propertyList )
        {
            Name = GetElementName();
            if ( GetProperty( "master-reference" ) != null )
                SetMasterName( GetProperty( "master-reference" ).GetString() );
            ValidateParent( parent );
        }

        public string MasterName { get; private set; }

        protected PageSequenceMaster PageSequenceMaster { get; set; }

        public abstract string GetNextPageMaster( int currentPageNumber,
            bool thisIsFirstPage,
            bool isEmptyPage );

        public abstract void Reset();

        protected void SetMasterName( string masterName )
        {
            MasterName = masterName;
        }

        protected abstract string GetElementName();

        protected void ValidateParent( FObj parent )
        {
            if ( parent.GetName().Equals( "fo:page-sequence-master" ) )
            {
                PageSequenceMaster = (PageSequenceMaster)parent;

                if ( MasterName == null )
                {
                    FonetDriver.ActiveDriver.FireFonetWarning(
                        GetElementName() + " does not have a master-reference and so is being ignored" );
                }
                else
                    PageSequenceMaster.AddSubsequenceSpecifier( this );
            }
            else
            {
                throw new FonetException( GetElementName() + " must be"
                    + "child of fo:page-sequence-master, not "
                    + parent.GetName() );
            }
        }
    }
}