using System;
using System.Collections;

namespace Fonet.Fo.Pagination
{
    internal class RepeatablePageMasterAlternatives : FObj, ISubSequenceSpecifier
    {
        private const int Infinite = -1;

        private readonly ArrayList _conditionalPageMasterRefs;

        private int _maximumRepeats;

        private int _numberConsumed;

        private readonly PageSequenceMaster _pageSequenceMaster;

        public RepeatablePageMasterAlternatives( FObj parent, PropertyList propertyList )
            : base( parent, propertyList )
        {
            Name = "fo:repeatable-page-master-alternatives";

            _conditionalPageMasterRefs = new ArrayList();

            if ( parent.GetName().Equals( "fo:page-sequence-master" ) )
            {
                _pageSequenceMaster = (PageSequenceMaster)parent;
                _pageSequenceMaster.AddSubsequenceSpecifier( this );
            }
            else
            {
                throw new FonetException( "fo:repeatable-page-master-alternatives"
                    + "must be child of fo:page-sequence-master, not "
                    + parent.GetName() );
            }

            string mr = GetProperty( "maximum-repeats" ).GetString();
            if ( mr.Equals( "no-limit" ) )
                SetMaximumRepeats( Infinite );
            else
            {
                try
                {
                    SetMaximumRepeats( int.Parse( mr ) );
                }
                catch ( FormatException )
                {
                    throw new FonetException( "Invalid number for 'maximum-repeats' property" );
                }
            }
        }

        public string GetNextPageMaster(
            int currentPageNumber, bool thisIsFirstPage, bool isEmptyPage )
        {
            string pm = null;
            if ( GetMaximumRepeats() != Infinite )
            {
                if ( _numberConsumed < GetMaximumRepeats() )
                    _numberConsumed++;
                else
                    return null;
            }

            foreach ( ConditionalPageMasterReference cpmr in _conditionalPageMasterRefs )
            {
                if ( cpmr.IsValid( currentPageNumber + 1, thisIsFirstPage, isEmptyPage ) )
                {
                    pm = cpmr.GetMasterName();
                    break;
                }
            }
            return pm;
        }

        public void Reset()
        {
            _numberConsumed = 0;
        }

        public new static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        private void SetMaximumRepeats( int maximumRepeats )
        {
            if ( maximumRepeats == Infinite )
                this._maximumRepeats = maximumRepeats;
            else
                this._maximumRepeats = maximumRepeats < 0 ? 0 : maximumRepeats;
        }

        private int GetMaximumRepeats()
        {
            return _maximumRepeats;
        }

        public void AddConditionalPageMasterReference( ConditionalPageMasterReference cpmr )
        {
            _conditionalPageMasterRefs.Add( cpmr );
        }

        protected PageSequenceMaster GetPageSequenceMaster()
        {
            return _pageSequenceMaster;
        }

        internal new class Maker : FObj.Maker
        {
            public override FObj Make( FObj parent, PropertyList propertyList )
            {
                return new RepeatablePageMasterAlternatives( parent, propertyList );
            }
        }
    }
}