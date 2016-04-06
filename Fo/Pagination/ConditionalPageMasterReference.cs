using Fonet.Fo.Properties;

namespace Fonet.Fo.Pagination
{
    internal class ConditionalPageMasterReference : FObj
    {
        private int blankOrNotBlank;

        private string masterName;
        private int oddOrEven;

        private int pagePosition;

        private RepeatablePageMasterAlternatives repeatablePageMasterAlternatives;

        public ConditionalPageMasterReference( FObj parent, PropertyList propertyList )
            : base( parent, propertyList )
        {
            name = GetElementName();
            if ( GetProperty( "master-reference" ) != null )
                SetMasterName( GetProperty( "master-reference" ).GetString() );

            validateParent( parent );

            setPagePosition( properties.GetProperty( "page-position" ).GetEnum() );
            setOddOrEven( properties.GetProperty( "odd-or-even" ).GetEnum() );
            setBlankOrNotBlank( properties.GetProperty( "blank-or-not-blank" ).GetEnum() );
        }

        public new static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        protected internal void SetMasterName( string masterName )
        {
            this.masterName = masterName;
        }

        public string GetMasterName()
        {
            return masterName;
        }

        protected internal bool isValid( int currentPageNumber, bool thisIsFirstPage,
            bool isEmptyPage )
        {
            var okOnPagePosition = true;
            switch ( getPagePosition() )
            {
            case PagePosition.FIRST:
                if ( !thisIsFirstPage )
                    okOnPagePosition = false;
                break;
            case PagePosition.LAST:
                FonetDriver.ActiveDriver.FireFonetInfo( "Last page position not known" );
                okOnPagePosition = true;
                break;
            case PagePosition.REST:
                if ( thisIsFirstPage )
                    okOnPagePosition = false;
                break;
            case PagePosition.ANY:
                okOnPagePosition = true;
                break;
            }

            var okOnOddOrEven = true;
            int ooe = getOddOrEven();
            bool isOddPage = currentPageNumber % 2 == 1 ? true : false;
            if ( OddOrEven.ODD == ooe && !isOddPage )
                okOnOddOrEven = false;
            if ( OddOrEven.EVEN == ooe && isOddPage )
                okOnOddOrEven = false;

            var okOnBlankOrNotBlank = true;

            int bnb = getBlankOrNotBlank();

            if ( BlankOrNotBlank.BLANK == bnb && !isEmptyPage )
                okOnBlankOrNotBlank = false;
            else if ( BlankOrNotBlank.NOT_BLANK == bnb && isEmptyPage )
                okOnBlankOrNotBlank = false;

            return okOnOddOrEven && okOnPagePosition && okOnBlankOrNotBlank;
        }

        protected internal void setPagePosition( int pagePosition )
        {
            this.pagePosition = pagePosition;
        }

        protected internal int getPagePosition()
        {
            return pagePosition;
        }

        protected internal void setOddOrEven( int oddOrEven )
        {
            this.oddOrEven = oddOrEven;
        }

        protected internal int getOddOrEven()
        {
            return oddOrEven;
        }

        protected internal void setBlankOrNotBlank( int blankOrNotBlank )
        {
            this.blankOrNotBlank = blankOrNotBlank;
        }

        protected internal int getBlankOrNotBlank()
        {
            return blankOrNotBlank;
        }

        protected internal string GetElementName()
        {
            return "fo:conditional-page-master-reference";
        }

        protected internal void validateParent( FObj parent )
        {
            if ( parent.GetName().Equals( "fo:repeatable-page-master-alternatives" ) )
            {
                repeatablePageMasterAlternatives =
                    (RepeatablePageMasterAlternatives)parent;

                if ( GetMasterName() == null )
                {
                    FonetDriver.ActiveDriver.FireFonetWarning(
                        "single-page-master-reference"
                            + "does not have a master-reference and so is being ignored" );
                }
                else
                    repeatablePageMasterAlternatives.addConditionalPageMasterReference( this );
            }
            else
            {
                throw new FonetException( "fo:conditional-page-master-reference must be child "
                    + "of fo:repeatable-page-master-alternatives, not "
                    + parent.GetName() );
            }
        }

        internal new class Maker : FObj.Maker
        {
            public override FObj Make( FObj parent, PropertyList propertyList )
            {
                return new ConditionalPageMasterReference( parent, propertyList );
            }
        }
    }
}