using System;

namespace Fonet.Fo.Pagination
{
    internal class RepeatablePageMasterReference :
        PageMasterReference
    {
        private const int Infinite = -1;

        private int _maximumRepeats;

        private int _numberConsumed;

        public RepeatablePageMasterReference( FObj parent, PropertyList propertyList )
            : base( parent, propertyList )
        {
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

        public override string GetNextPageMaster(
            int currentPageNumber, bool thisIsFirstPage, bool isEmptyPage )
        {
            string pm = MasterName;
            if ( GetMaximumRepeats() != Infinite )
            {
                if ( _numberConsumed < GetMaximumRepeats() )
                    _numberConsumed++;
                else
                    pm = null;
            }
            return pm;
        }

        public override void Reset()
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

        protected override string GetElementName()
        {
            return "fo:repeatable-page-master-reference";
        }

        internal new class Maker : FObj.Maker
        {
            public override FObj Make( FObj parent, PropertyList propertyList )
            {
                return new RepeatablePageMasterReference( parent, propertyList );
            }
        }
    }
}