using Fonet.Fo.Properties;

namespace Fonet.Fo.Pagination
{
    internal class ConditionalPageMasterReference : FObj
    {
        private int _blankOrNotBlank;

        private string _masterName;
        private int _oddOrEven;

        private int _pagePosition;

        private RepeatablePageMasterAlternatives _repeatablePageMasterAlternatives;

        public ConditionalPageMasterReference( FObj parent, PropertyList propertyList )
            : base( parent, propertyList )
        {
            Name = GetElementName();
            if ( GetProperty( "master-reference" ) != null )
                SetMasterName( GetProperty( "master-reference" ).GetString() );

            ValidateParent( parent );

            SetPagePosition( Properties.GetProperty( "page-position" ).GetEnum() );
            SetOddOrEven( Properties.GetProperty( "odd-or-even" ).GetEnum() );
            SetBlankOrNotBlank( Properties.GetProperty( "blank-or-not-blank" ).GetEnum() );
        }

        public new static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        protected internal void SetMasterName( string masterName )
        {
            this._masterName = masterName;
        }

        public string GetMasterName()
        {
            return _masterName;
        }

        protected internal bool IsValid( int currentPageNumber, bool thisIsFirstPage,
            bool isEmptyPage )
        {
            var okOnPagePosition = true;
            switch ( GetPagePosition() )
            {
            case PagePosition.First:
                if ( !thisIsFirstPage )
                    okOnPagePosition = false;
                break;
            case PagePosition.Last:
                FonetDriver.ActiveDriver.FireFonetInfo( "Last page position not known" );
                okOnPagePosition = true;
                break;
            case PagePosition.Rest:
                if ( thisIsFirstPage )
                    okOnPagePosition = false;
                break;
            case PagePosition.Any:
                okOnPagePosition = true;
                break;
            }

            var okOnOddOrEven = true;
            int ooe = GetOddOrEven();
            bool isOddPage = currentPageNumber % 2 == 1 ? true : false;
            if ( OddOrEven.Odd == ooe && !isOddPage )
                okOnOddOrEven = false;
            if ( OddOrEven.Even == ooe && isOddPage )
                okOnOddOrEven = false;

            var okOnBlankOrNotBlank = true;

            int bnb = GetBlankOrNotBlank();

            if ( BlankOrNotBlank.Blank == bnb && !isEmptyPage )
                okOnBlankOrNotBlank = false;
            else if ( BlankOrNotBlank.NotBlank == bnb && isEmptyPage )
                okOnBlankOrNotBlank = false;

            return okOnOddOrEven && okOnPagePosition && okOnBlankOrNotBlank;
        }

        protected internal void SetPagePosition( int pagePosition )
        {
            this._pagePosition = pagePosition;
        }

        protected internal int GetPagePosition()
        {
            return _pagePosition;
        }

        protected internal void SetOddOrEven( int oddOrEven )
        {
            this._oddOrEven = oddOrEven;
        }

        protected internal int GetOddOrEven()
        {
            return _oddOrEven;
        }

        protected internal void SetBlankOrNotBlank( int blankOrNotBlank )
        {
            this._blankOrNotBlank = blankOrNotBlank;
        }

        protected internal int GetBlankOrNotBlank()
        {
            return _blankOrNotBlank;
        }

        protected internal string GetElementName()
        {
            return "fo:conditional-page-master-reference";
        }

        protected internal void ValidateParent( FObj parent )
        {
            if ( parent.GetName().Equals( "fo:repeatable-page-master-alternatives" ) )
            {
                _repeatablePageMasterAlternatives =
                    (RepeatablePageMasterAlternatives)parent;

                if ( GetMasterName() == null )
                {
                    FonetDriver.ActiveDriver.FireFonetWarning(
                        "single-page-master-reference"
                            + "does not have a master-reference and so is being ignored" );
                }
                else
                    _repeatablePageMasterAlternatives.AddConditionalPageMasterReference( this );
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