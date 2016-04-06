using System;
using System.Collections;
using Fonet.Fo.Flow;
using Fonet.Fo.Properties;
using Fonet.Layout;

namespace Fonet.Fo.Pagination
{
    internal class PageSequence : FObj
    {
        private const int Explicit = 0;
        private const int Auto = 1;
        private const int AutoEven = 2;
        private const int AutoOdd = 3;
        private readonly Hashtable _flowMap;
        private Page _currentPage;
        private string _currentPageMasterName;
        private ISubSequenceSpecifier _currentSubsequence;
        private int _currentSubsequenceNumber = -1;
        private readonly int _forcePageCount;
        private bool _isForcing;
        private readonly LayoutMasterSet _layoutMasterSet;
        private readonly string _masterName;
        private readonly PageNumberGenerator _pageNumberGenerator;
        private readonly int _pageNumberType;

        private readonly Root _root;
        private bool _thisIsFirstPage;

        protected PageSequence( FObj parent, PropertyList propertyList )
            : base( parent, propertyList )
        {
            PageCount = 0;
            CurrentPageNumber = 0;
            IsFlowSet = false;
            Name = "fo:page-sequence";

            if ( parent.GetName().Equals( "fo:root" ) )
                _root = (Root)parent;
            else
            {
                throw new FonetException( "page-sequence must be child of root, not "
                    + parent.GetName() );
            }

            _layoutMasterSet = _root.GetLayoutMasterSet();
            _layoutMasterSet.CheckRegionNames();

            _flowMap = new Hashtable();

            _thisIsFirstPage = true;
            IpnValue = Properties.GetProperty( "initial-page-number" ).GetString();

            if ( IpnValue.Equals( "auto" ) )
                _pageNumberType = Auto;
            else if ( IpnValue.Equals( "auto-even" ) )
                _pageNumberType = AutoEven;
            else if ( IpnValue.Equals( "auto-odd" ) )
                _pageNumberType = AutoOdd;
            else
            {
                _pageNumberType = Explicit;
                try
                {
                    int pageStart = int.Parse( IpnValue );
                    CurrentPageNumber = pageStart > 0 ? pageStart - 1 : 0;
                }
                catch ( FormatException )
                {
                    throw new FonetException( "\"" + IpnValue
                        + "\" is not a valid value for initial-page-number" );
                }
            }

            _masterName = Properties.GetProperty( "master-reference" ).GetString();

            _pageNumberGenerator =
                new PageNumberGenerator( Properties.GetProperty( "format" ).GetString(),
                    Properties.GetProperty( "grouping-separator" ).GetCharacter(),
                    Properties.GetProperty( "grouping-size" ).GetNumber().IntValue(),
                    Properties.GetProperty( "letter-value" ).GetEnum() );

            _forcePageCount =
                Properties.GetProperty( "force-page-count" ).GetEnum();
        }

        public bool IsFlowSet { get; set; }

        public string IpnValue { get; private set; }

        public int CurrentPageNumber { get; private set; }

        public int PageCount { get; private set; }

        public new static FObj.Maker GetMaker()
        {
            return new Maker();
        }


        public void AddFlow( Flow.Flow flow )
        {
            if ( _flowMap.ContainsKey( flow.GetFlowName() ) )
                throw new FonetException( "flow-names must be unique within an fo:page-sequence" );
            if ( !_layoutMasterSet.RegionNameExists( flow.GetFlowName() ) )
            {
                FonetDriver.ActiveDriver.FireFonetError(
                    "region-name '" + flow.GetFlowName() + "' doesn't exist in the layout-master-set." );
            }
            _flowMap.Add( flow.GetFlowName(), flow );
            IsFlowSet = true;
        }

        public void Format( AreaTree areaTree )
        {
            var status = new Status( Status.Ok );

            _layoutMasterSet.ResetPageMasters();

            var firstAvailPageNumber = 0;
            do
            {
                firstAvailPageNumber = _root.GetRunningPageNumberCounter();
                var tempIsFirstPage = false;

                if ( _thisIsFirstPage )
                {
                    tempIsFirstPage = _thisIsFirstPage;
                    if ( _pageNumberType == Auto )
                    {
                        CurrentPageNumber =
                            _root.GetRunningPageNumberCounter();
                    }
                    else if ( _pageNumberType == AutoOdd )
                    {
                        CurrentPageNumber =
                            _root.GetRunningPageNumberCounter();
                        if ( CurrentPageNumber % 2 == 1 )
                            CurrentPageNumber++;
                    }
                    else if ( _pageNumberType == AutoEven )
                    {
                        CurrentPageNumber =
                            _root.GetRunningPageNumberCounter();
                        if ( CurrentPageNumber % 2 == 0 )
                            CurrentPageNumber++;
                    }
                    _thisIsFirstPage = false;
                }

                CurrentPageNumber++;
                var isEmptyPage = false;

                if ( status.GetCode() == Status.ForcePageBreakEven
                    && CurrentPageNumber % 2 == 1 )
                    isEmptyPage = true;
                else if ( status.GetCode() == Status.ForcePageBreakOdd
                    && CurrentPageNumber % 2 == 0 )
                    isEmptyPage = true;
                else
                    isEmptyPage = false;

                _currentPage = MakePage( areaTree, firstAvailPageNumber,
                    tempIsFirstPage, isEmptyPage );

                _currentPage.SetNumber( CurrentPageNumber );
                string formattedPageNumber =
                    _pageNumberGenerator.MakeFormattedPageNumber( CurrentPageNumber );
                _currentPage.SetFormattedNumber( formattedPageNumber );
                _root.SetRunningPageNumberCounter( CurrentPageNumber );

                FonetDriver.ActiveDriver.FireFonetInfo(
                    "[" + CurrentPageNumber + "]" );

                if ( status.GetCode() == Status.ForcePageBreakEven
                    && CurrentPageNumber % 2 == 1 )
                {
                }
                else if ( status.GetCode() == Status.ForcePageBreakOdd
                    && CurrentPageNumber % 2 == 0 )
                {
                }
                else
                {
                    BodyAreaContainer bodyArea = _currentPage.GetBody();
                    bodyArea.SetIDReferences( areaTree.GetIDReferences() );

                    Flow.Flow flow = GetCurrentFlow( RegionBody.RegionClass );

                    if ( flow == null )
                    {
                        FonetDriver.ActiveDriver.FireFonetError(
                            "No flow found for region-body in page-master '" + _currentPageMasterName + "'" );
                        break;
                    }
                    status = flow.Layout( bodyArea );
                }

                _currentPage.SetPageSequence( this );
                FormatStaticContent( areaTree );

                areaTree.AddPage( _currentPage );
                PageCount++;
            }
            while ( FlowsAreIncomplete() );
            ForcePage( areaTree, firstAvailPageNumber );
            _currentPage = null;
        }

        private Page MakePage( AreaTree areaTree, int firstAvailPageNumber,
            bool isFirstPage,
            bool isEmptyPage )
        {
            PageMaster pageMaster = GetNextPageMaster( _masterName,
                firstAvailPageNumber,
                isFirstPage, isEmptyPage );
            if ( pageMaster == null )
                throw new FonetException( "page masters exhausted. Cannot recover." );
            Page p = pageMaster.MakePage( areaTree );
            if ( _currentPage != null )
            {
                ArrayList foots = _currentPage.GetPendingFootnotes();
                p.SetPendingFootnotes( foots );
            }
            return p;
        }

        private void FormatStaticContent( AreaTree areaTree )
        {
            SimplePageMaster simpleMaster = GetCurrentSimplePageMaster();

            if ( simpleMaster.GetRegion( RegionBefore.RegionClass ) != null
                && _currentPage.GetBefore() != null )
            {
                var staticFlow =
                    (Flow.Flow)_flowMap[ simpleMaster.GetRegion( RegionBefore.RegionClass ).GetRegionName() ];
                if ( staticFlow != null )
                {
                    AreaContainer beforeArea = _currentPage.GetBefore();
                    beforeArea.SetIDReferences( areaTree.GetIDReferences() );
                    LayoutStaticContent( staticFlow,
                        simpleMaster.GetRegion( RegionBefore.RegionClass ),
                        beforeArea );
                }
            }

            if ( simpleMaster.GetRegion( RegionAfter.RegionClass ) != null
                && _currentPage.GetAfter() != null )
            {
                var staticFlow =
                    (Flow.Flow)_flowMap[ simpleMaster.GetRegion( RegionAfter.RegionClass ).GetRegionName() ];
                if ( staticFlow != null )
                {
                    AreaContainer afterArea = _currentPage.GetAfter();
                    afterArea.SetIDReferences( areaTree.GetIDReferences() );
                    LayoutStaticContent( staticFlow,
                        simpleMaster.GetRegion( RegionAfter.RegionClass ),
                        afterArea );
                }
            }

            if ( simpleMaster.GetRegion( RegionStart.RegionClass ) != null
                && _currentPage.GetStart() != null )
            {
                var staticFlow =
                    (Flow.Flow)_flowMap[ simpleMaster.GetRegion( RegionStart.RegionClass ).GetRegionName() ];
                if ( staticFlow != null )
                {
                    AreaContainer startArea = _currentPage.GetStart();
                    startArea.SetIDReferences( areaTree.GetIDReferences() );
                    LayoutStaticContent( staticFlow,
                        simpleMaster.GetRegion( RegionStart.RegionClass ),
                        startArea );
                }
            }

            if ( simpleMaster.GetRegion( RegionEnd.RegionClass ) != null
                && _currentPage.GetEnd() != null )
            {
                var staticFlow =
                    (Flow.Flow)_flowMap[ simpleMaster.GetRegion( RegionEnd.RegionClass ).GetRegionName() ];
                if ( staticFlow != null )
                {
                    AreaContainer endArea = _currentPage.GetEnd();
                    endArea.SetIDReferences( areaTree.GetIDReferences() );
                    LayoutStaticContent( staticFlow,
                        simpleMaster.GetRegion( RegionEnd.RegionClass ),
                        endArea );
                }
            }
        }

        private void LayoutStaticContent( Flow.Flow flow, Region region,
            AreaContainer area )
        {
            if ( flow is StaticContent )
                ( (StaticContent)flow ).Layout( area, region );
            else
            {
                FonetDriver.ActiveDriver.FireFonetError( region.GetName()
                    + " only supports static-content flows currently. "
                    + "Cannot use flow named '"
                    + flow.GetFlowName() + "'" );
            }
        }

        private ISubSequenceSpecifier GetNextSubsequence( PageSequenceMaster master )
        {
            if ( master.GetSubSequenceSpecifierCount()
                > _currentSubsequenceNumber + 1 )
            {
                _currentSubsequence =
                    master.GetSubSequenceSpecifier( _currentSubsequenceNumber + 1 );
                _currentSubsequenceNumber++;
                return _currentSubsequence;
            }
            return null;
        }

        private SimplePageMaster GetNextSimplePageMaster( PageSequenceMaster sequenceMaster,
            int currentPageNumber, bool thisIsFirstPage,
            bool isEmptyPage )
        {
            if ( _isForcing )
            {
                return _layoutMasterSet.GetSimplePageMaster(
                    GetNextPageMasterName( sequenceMaster, currentPageNumber, false, true ) );
            }
            string nextPageMaster =
                GetNextPageMasterName( sequenceMaster, currentPageNumber, thisIsFirstPage, isEmptyPage );
            return _layoutMasterSet.GetSimplePageMaster( nextPageMaster );
        }

        private string GetNextPageMasterName( PageSequenceMaster sequenceMaster,
            int currentPageNumber,
            bool thisIsFirstPage,
            bool isEmptyPage )
        {
            if ( null == _currentSubsequence )
                _currentSubsequence = GetNextSubsequence( sequenceMaster );

            string nextPageMaster =
                _currentSubsequence.GetNextPageMaster( currentPageNumber,
                    thisIsFirstPage,
                    isEmptyPage );


            if ( null == nextPageMaster
                || IsFlowForMasterNameDone( _currentPageMasterName ) )
            {
                ISubSequenceSpecifier nextSubsequence =
                    GetNextSubsequence( sequenceMaster );
                if ( nextSubsequence == null )
                {
                    FonetDriver.ActiveDriver.FireFonetError(
                        "Page subsequences exhausted. Using previous subsequence." );
                    thisIsFirstPage =
                        true;
                    _currentSubsequence.Reset();
                }
                else
                    _currentSubsequence = nextSubsequence;

                nextPageMaster =
                    _currentSubsequence.GetNextPageMaster( currentPageNumber,
                        thisIsFirstPage,
                        isEmptyPage );
            }
            _currentPageMasterName = nextPageMaster;

            return nextPageMaster;
        }

        private SimplePageMaster GetCurrentSimplePageMaster()
        {
            return _layoutMasterSet.GetSimplePageMaster( _currentPageMasterName );
        }

        private string GetCurrentPageMasterName()
        {
            return _currentPageMasterName;
        }

        private PageMaster GetNextPageMaster( string pageSequenceName,
            int currentPageNumber,
            bool thisIsFirstPage,
            bool isEmptyPage )
        {
            PageMaster pageMaster = null;

            PageSequenceMaster sequenceMaster =
                _layoutMasterSet.GetPageSequenceMaster( pageSequenceName );

            if ( sequenceMaster != null )
            {
                pageMaster = GetNextSimplePageMaster( sequenceMaster,
                    currentPageNumber,
                    thisIsFirstPage,
                    isEmptyPage ).GetPageMaster();
            }
            else
            {
                SimplePageMaster simpleMaster =
                    _layoutMasterSet.GetSimplePageMaster( pageSequenceName );
                if ( simpleMaster == null )
                {
                    throw new FonetException( "'master-reference' for 'fo:page-sequence'"
                        + "matches no 'simple-page-master' or 'page-sequence-master'" );
                }
                _currentPageMasterName = pageSequenceName;

                pageMaster = simpleMaster.GetNextPageMaster();
            }
            return pageMaster;
        }

        private bool FlowsAreIncomplete()
        {
            var isIncomplete = false;

            foreach ( Flow.Flow flow in _flowMap.Values )
            {
                if ( flow is StaticContent )
                    continue;
                Status status = flow.GetStatus();
                isIncomplete |= status.IsIncomplete();
            }
            return isIncomplete;
        }

        private Flow.Flow GetCurrentFlow( string regionClass )
        {
            Region region = GetCurrentSimplePageMaster().GetRegion( regionClass );
            if ( region != null )
            {
                var flow = (Flow.Flow)_flowMap[ region.GetRegionName() ];
                return flow;
            }
            FonetDriver.ActiveDriver.FireFonetInfo(
                "flow is null. regionClass = '" + regionClass
                    + "' currentSPM = "
                    + GetCurrentSimplePageMaster() );
            return null;
        }

        private bool IsFlowForMasterNameDone( string masterName )
        {
            if ( _isForcing )
                return false;
            if ( masterName != null )
            {
                SimplePageMaster spm =
                    _layoutMasterSet.GetSimplePageMaster( masterName );
                Region region = spm.GetRegion( RegionBody.RegionClass );


                var flow = (Flow.Flow)_flowMap[ region.GetRegionName() ];
                if ( null == flow || flow.GetStatus().IsIncomplete() )
                    return false;
                return true;
            }
            return false;
        }

        private void ForcePage( AreaTree areaTree, int firstAvailPageNumber )
        {
            var bmakePage = false;
            if ( _forcePageCount == ForcePageCount.Auto )
            {
                PageSequence nextSequence =
                    _root.GetSucceedingPageSequence( this );
                if ( nextSequence != null )
                {
                    if ( nextSequence.IpnValue.Equals( "auto" ) )
                    {
                        // do nothing
                    }
                    else if ( nextSequence.IpnValue.Equals( "auto-odd" ) )
                    {
                        if ( firstAvailPageNumber % 2 == 0 )
                            bmakePage = true;
                    }
                    else if ( nextSequence.IpnValue.Equals( "auto-even" ) )
                    {
                        if ( firstAvailPageNumber % 2 != 0 )
                            bmakePage = true;
                    }
                    else
                    {
                        int nextSequenceStartPageNumber =
                            nextSequence.CurrentPageNumber;
                        if ( nextSequenceStartPageNumber % 2 == 0
                            && firstAvailPageNumber % 2 == 0 )
                            bmakePage = true;
                        else if ( nextSequenceStartPageNumber % 2 != 0
                            && firstAvailPageNumber % 2 != 0 )
                            bmakePage = true;
                    }
                }
            }
            else if ( _forcePageCount == ForcePageCount.Even
                && PageCount % 2 != 0 )
                bmakePage = true;
            else if ( _forcePageCount == ForcePageCount.Odd
                && PageCount % 2 == 0 )
                bmakePage = true;
            else if ( _forcePageCount == ForcePageCount.EndOnEven
                && firstAvailPageNumber % 2 == 0 )
                bmakePage = true;
            else if ( _forcePageCount == ForcePageCount.EndOnOdd
                && firstAvailPageNumber % 2 != 0 )
                bmakePage = true;
            else if ( _forcePageCount == ForcePageCount.NoForce )
            {
                // do nothing
            }

            if ( bmakePage )
            {
                try
                {
                    _isForcing = true;
                    CurrentPageNumber++;
                    firstAvailPageNumber = CurrentPageNumber;
                    _currentPage = MakePage( areaTree, firstAvailPageNumber, false, true );
                    string formattedPageNumber =
                        _pageNumberGenerator.MakeFormattedPageNumber( CurrentPageNumber );
                    _currentPage.SetFormattedNumber( formattedPageNumber );
                    _currentPage.SetPageSequence( this );
                    FormatStaticContent( areaTree );

                    FonetDriver.ActiveDriver.FireFonetInfo(
                        "[forced-" + firstAvailPageNumber + "]" );

                    areaTree.AddPage( _currentPage );
                    _root.SetRunningPageNumberCounter( CurrentPageNumber );
                    _isForcing = false;
                }
                catch ( FonetException )
                {
                    FonetDriver.ActiveDriver.FireFonetInfo(
                        "'force-page-count' failure" );
                }
            }
        }

        internal new class Maker : FObj.Maker
        {
            public override FObj Make( FObj parent, PropertyList propertyList )
            {
                return new PageSequence( parent, propertyList );
            }
        }
    }
}