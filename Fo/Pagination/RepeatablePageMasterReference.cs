using System;

namespace Fonet.Fo.Pagination
{
    internal class RepeatablePageMasterReference :
        PageMasterReference, SubSequenceSpecifier
    {
        private const int INFINITE = -1;

        private int maximumRepeats;

        private int numberConsumed;

        public RepeatablePageMasterReference( FObj parent, PropertyList propertyList )
            : base( parent, propertyList )
        {
            string mr = GetProperty( "maximum-repeats" ).GetString();
            if ( mr.Equals( "no-limit" ) )
                setMaximumRepeats( INFINITE );
            else
            {
                try
                {
                    setMaximumRepeats( int.Parse( mr ) );
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
            if ( getMaximumRepeats() != INFINITE )
            {
                if ( numberConsumed < getMaximumRepeats() )
                    numberConsumed++;
                else
                    pm = null;
            }
            return pm;
        }

        public override void Reset()
        {
            numberConsumed = 0;
        }

        public new static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        private void setMaximumRepeats( int maximumRepeats )
        {
            if ( maximumRepeats == INFINITE )
                this.maximumRepeats = maximumRepeats;
            else
                this.maximumRepeats = maximumRepeats < 0 ? 0 : maximumRepeats;
        }

        private int getMaximumRepeats()
        {
            return maximumRepeats;
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