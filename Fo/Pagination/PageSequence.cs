using System;
using System.Collections;
using Fonet.Fo.Flow;
using Fonet.Fo.Properties;
using Fonet.Layout;

namespace Fonet.Fo.Pagination
{
    internal class PageSequence : FObj
    {
        private const int EXPLICIT = 0;
        private const int AUTO = 1;
        private const int AUTO_EVEN = 2;
        private const int AUTO_ODD = 3;
        private readonly Hashtable _flowMap;
        private Page currentPage;
        private string currentPageMasterName;
        private SubSequenceSpecifier currentSubsequence;
        private int currentSubsequenceNumber = -1;
        private readonly int forcePageCount;
        private bool isForcing;
        private readonly LayoutMasterSet layoutMasterSet;
        private readonly string masterName;
        private readonly PageNumberGenerator pageNumberGenerator;
        private readonly int pageNumberType;

        private readonly Root root;
        private bool thisIsFirstPage;

        protected PageSequence( FObj parent, PropertyList propertyList )
            : base( parent, propertyList )
        {
            PageCount = 0;
            CurrentPageNumber = 0;
            IsFlowSet = false;
            name = "fo:page-sequence";

            if ( parent.GetName().Equals( "fo:root" ) )
                root = (Root)parent;
            else
            {
                throw new FonetException( "page-sequence must be child of root, not "
                    + parent.GetName() );
            }

            layoutMasterSet = root.getLayoutMasterSet();
            layoutMasterSet.checkRegionNames();

            _flowMap = new Hashtable();

            thisIsFirstPage = true;
            IpnValue = properties.GetProperty( "initial-page-number" ).GetString();

            if ( IpnValue.Equals( "auto" ) )
                pageNumberType = AUTO;
            else if ( IpnValue.Equals( "auto-even" ) )
                pageNumberType = AUTO_EVEN;
            else if ( IpnValue.Equals( "auto-odd" ) )
                pageNumberType = AUTO_ODD;
            else
            {
                pageNumberType = EXPLICIT;
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

            masterName = properties.GetProperty( "master-reference" ).GetString();

            pageNumberGenerator =
                new PageNumberGenerator( properties.GetProperty( "format" ).GetString(),
                    properties.GetProperty( "grouping-separator" ).GetCharacter(),
                    properties.GetProperty( "grouping-size" ).GetNumber().IntValue(),
                    properties.GetProperty( "letter-value" ).GetEnum() );

            forcePageCount =
                properties.GetProperty( "force-page-count" ).GetEnum();
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
            if ( !layoutMasterSet.regionNameExists( flow.GetFlowName() ) )
            {
                FonetDriver.ActiveDriver.FireFonetError(
                    "region-name '" + flow.GetFlowName() + "' doesn't exist in the layout-master-set." );
            }
            _flowMap.Add( flow.GetFlowName(), flow );
            IsFlowSet = true;
        }

        public void Format( AreaTree areaTree )
        {
            var status = new Status( Status.OK );

            layoutMasterSet.resetPageMasters();

            var firstAvailPageNumber = 0;
            do
            {
                firstAvailPageNumber = root.getRunningPageNumberCounter();
                var tempIsFirstPage = false;

                if ( thisIsFirstPage )
                {
                    tempIsFirstPage = thisIsFirstPage;
                    if ( pageNumberType == AUTO )
                    {
                        CurrentPageNumber =
                            root.getRunningPageNumberCounter();
                    }
                    else if ( pageNumberType == AUTO_ODD )
                    {
                        CurrentPageNumber =
                            root.getRunningPageNumberCounter();
                        if ( CurrentPageNumber % 2 == 1 )
                            CurrentPageNumber++;
                    }
                    else if ( pageNumberType == AUTO_EVEN )
                    {
                        CurrentPageNumber =
                            root.getRunningPageNumberCounter();
                        if ( CurrentPageNumber % 2 == 0 )
                            CurrentPageNumber++;
                    }
                    thisIsFirstPage = false;
                }

                CurrentPageNumber++;
                var isEmptyPage = false;

                if ( status.getCode() == Status.FORCE_PAGE_BREAK_EVEN
                    && CurrentPageNumber % 2 == 1 )
                    isEmptyPage = true;
                else if ( status.getCode() == Status.FORCE_PAGE_BREAK_ODD
                    && CurrentPageNumber % 2 == 0 )
                    isEmptyPage = true;
                else
                    isEmptyPage = false;

                currentPage = MakePage( areaTree, firstAvailPageNumber,
                    tempIsFirstPage, isEmptyPage );

                currentPage.setNumber( CurrentPageNumber );
                string formattedPageNumber =
                    pageNumberGenerator.makeFormattedPageNumber( CurrentPageNumber );
                currentPage.setFormattedNumber( formattedPageNumber );
                root.setRunningPageNumberCounter( CurrentPageNumber );

                FonetDriver.ActiveDriver.FireFonetInfo(
                    "[" + CurrentPageNumber + "]" );

                if ( status.getCode() == Status.FORCE_PAGE_BREAK_EVEN
                    && CurrentPageNumber % 2 == 1 )
                {
                }
                else if ( status.getCode() == Status.FORCE_PAGE_BREAK_ODD
                    && CurrentPageNumber % 2 == 0 )
                {
                }
                else
                {
                    BodyAreaContainer bodyArea = currentPage.getBody();
                    bodyArea.setIDReferences( areaTree.getIDReferences() );

                    Flow.Flow flow = GetCurrentFlow( RegionBody.REGION_CLASS );

                    if ( flow == null )
                    {
                        FonetDriver.ActiveDriver.FireFonetError(
                            "No flow found for region-body in page-master '" + currentPageMasterName + "'" );
                        break;
                    }
                    status = flow.Layout( bodyArea );
                }

                currentPage.setPageSequence( this );
                FormatStaticContent( areaTree );

                areaTree.addPage( currentPage );
                PageCount++;
            }
            while ( FlowsAreIncomplete() );
            ForcePage( areaTree, firstAvailPageNumber );
            currentPage = null;
        }

        private Page MakePage( AreaTree areaTree, int firstAvailPageNumber,
            bool isFirstPage,
            bool isEmptyPage )
        {
            PageMaster pageMaster = GetNextPageMaster( masterName,
                firstAvailPageNumber,
                isFirstPage, isEmptyPage );
            if ( pageMaster == null )
                throw new FonetException( "page masters exhausted. Cannot recover." );
            Page p = pageMaster.makePage( areaTree );
            if ( currentPage != null )
            {
                ArrayList foots = currentPage.getPendingFootnotes();
                p.setPendingFootnotes( foots );
            }
            return p;
        }

        private void FormatStaticContent( AreaTree areaTree )
        {
            SimplePageMaster simpleMaster = GetCurrentSimplePageMaster();

            if ( simpleMaster.getRegion( RegionBefore.REGION_CLASS ) != null
                && currentPage.getBefore() != null )
            {
                var staticFlow =
                    (Flow.Flow)_flowMap[ simpleMaster.getRegion( RegionBefore.REGION_CLASS ).getRegionName() ];
                if ( staticFlow != null )
                {
                    AreaContainer beforeArea = currentPage.getBefore();
                    beforeArea.setIDReferences( areaTree.getIDReferences() );
                    LayoutStaticContent( staticFlow,
                        simpleMaster.getRegion( RegionBefore.REGION_CLASS ),
                        beforeArea );
                }
            }

            if ( simpleMaster.getRegion( RegionAfter.REGION_CLASS ) != null
                && currentPage.getAfter() != null )
            {
                var staticFlow =
                    (Flow.Flow)_flowMap[ simpleMaster.getRegion( RegionAfter.REGION_CLASS ).getRegionName() ];
                if ( staticFlow != null )
                {
                    AreaContainer afterArea = currentPage.getAfter();
                    afterArea.setIDReferences( areaTree.getIDReferences() );
                    LayoutStaticContent( staticFlow,
                        simpleMaster.getRegion( RegionAfter.REGION_CLASS ),
                        afterArea );
                }
            }

            if ( simpleMaster.getRegion( RegionStart.REGION_CLASS ) != null
                && currentPage.getStart() != null )
            {
                var staticFlow =
                    (Flow.Flow)_flowMap[ simpleMaster.getRegion( RegionStart.REGION_CLASS ).getRegionName() ];
                if ( staticFlow != null )
                {
                    AreaContainer startArea = currentPage.getStart();
                    startArea.setIDReferences( areaTree.getIDReferences() );
                    LayoutStaticContent( staticFlow,
                        simpleMaster.getRegion( RegionStart.REGION_CLASS ),
                        startArea );
                }
            }

            if ( simpleMaster.getRegion( RegionEnd.REGION_CLASS ) != null
                && currentPage.getEnd() != null )
            {
                var staticFlow =
                    (Flow.Flow)_flowMap[ simpleMaster.getRegion( RegionEnd.REGION_CLASS ).getRegionName() ];
                if ( staticFlow != null )
                {
                    AreaContainer endArea = currentPage.getEnd();
                    endArea.setIDReferences( areaTree.getIDReferences() );
                    LayoutStaticContent( staticFlow,
                        simpleMaster.getRegion( RegionEnd.REGION_CLASS ),
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

        private SubSequenceSpecifier GetNextSubsequence( PageSequenceMaster master )
        {
            if ( master.GetSubSequenceSpecifierCount()
                > currentSubsequenceNumber + 1 )
            {
                currentSubsequence =
                    master.getSubSequenceSpecifier( currentSubsequenceNumber + 1 );
                currentSubsequenceNumber++;
                return currentSubsequence;
            }
            return null;
        }

        private SimplePageMaster GetNextSimplePageMaster( PageSequenceMaster sequenceMaster,
            int currentPageNumber, bool thisIsFirstPage,
            bool isEmptyPage )
        {
            if ( isForcing )
            {
                return layoutMasterSet.getSimplePageMaster(
                    GetNextPageMasterName( sequenceMaster, currentPageNumber, false, true ) );
            }
            string nextPageMaster =
                GetNextPageMasterName( sequenceMaster, currentPageNumber, thisIsFirstPage, isEmptyPage );
            return layoutMasterSet.getSimplePageMaster( nextPageMaster );
        }

        private string GetNextPageMasterName( PageSequenceMaster sequenceMaster,
            int currentPageNumber,
            bool thisIsFirstPage,
            bool isEmptyPage )
        {
            if ( null == currentSubsequence )
                currentSubsequence = GetNextSubsequence( sequenceMaster );

            string nextPageMaster =
                currentSubsequence.GetNextPageMaster( currentPageNumber,
                    thisIsFirstPage,
                    isEmptyPage );


            if ( null == nextPageMaster
                || IsFlowForMasterNameDone( currentPageMasterName ) )
            {
                SubSequenceSpecifier nextSubsequence =
                    GetNextSubsequence( sequenceMaster );
                if ( nextSubsequence == null )
                {
                    FonetDriver.ActiveDriver.FireFonetError(
                        "Page subsequences exhausted. Using previous subsequence." );
                    thisIsFirstPage =
                        true;
                    currentSubsequence.Reset();
                }
                else
                    currentSubsequence = nextSubsequence;

                nextPageMaster =
                    currentSubsequence.GetNextPageMaster( currentPageNumber,
                        thisIsFirstPage,
                        isEmptyPage );
            }
            currentPageMasterName = nextPageMaster;

            return nextPageMaster;
        }

        private SimplePageMaster GetCurrentSimplePageMaster()
        {
            return layoutMasterSet.getSimplePageMaster( currentPageMasterName );
        }

        private string GetCurrentPageMasterName()
        {
            return currentPageMasterName;
        }

        private PageMaster GetNextPageMaster( string pageSequenceName,
            int currentPageNumber,
            bool thisIsFirstPage,
            bool isEmptyPage )
        {
            PageMaster pageMaster = null;

            PageSequenceMaster sequenceMaster =
                layoutMasterSet.getPageSequenceMaster( pageSequenceName );

            if ( sequenceMaster != null )
            {
                pageMaster = GetNextSimplePageMaster( sequenceMaster,
                    currentPageNumber,
                    thisIsFirstPage,
                    isEmptyPage ).getPageMaster();
            }
            else
            {
                SimplePageMaster simpleMaster =
                    layoutMasterSet.getSimplePageMaster( pageSequenceName );
                if ( simpleMaster == null )
                {
                    throw new FonetException( "'master-reference' for 'fo:page-sequence'"
                        + "matches no 'simple-page-master' or 'page-sequence-master'" );
                }
                currentPageMasterName = pageSequenceName;

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
                Status status = flow.getStatus();
                isIncomplete |= status.isIncomplete();
            }
            return isIncomplete;
        }

        private Flow.Flow GetCurrentFlow( string regionClass )
        {
            Region region = GetCurrentSimplePageMaster().getRegion( regionClass );
            if ( region != null )
            {
                var flow = (Flow.Flow)_flowMap[ region.getRegionName() ];
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
            if ( isForcing )
                return false;
            if ( masterName != null )
            {
                SimplePageMaster spm =
                    layoutMasterSet.getSimplePageMaster( masterName );
                Region region = spm.getRegion( RegionBody.REGION_CLASS );


                var flow = (Flow.Flow)_flowMap[ region.getRegionName() ];
                if ( null == flow || flow.getStatus().isIncomplete() )
                    return false;
                return true;
            }
            return false;
        }

        private void ForcePage( AreaTree areaTree, int firstAvailPageNumber )
        {
            var bmakePage = false;
            if ( forcePageCount == ForcePageCount.AUTO )
            {
                PageSequence nextSequence =
                    root.getSucceedingPageSequence( this );
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
            else if ( forcePageCount == ForcePageCount.EVEN
                && PageCount % 2 != 0 )
                bmakePage = true;
            else if ( forcePageCount == ForcePageCount.ODD
                && PageCount % 2 == 0 )
                bmakePage = true;
            else if ( forcePageCount == ForcePageCount.END_ON_EVEN
                && firstAvailPageNumber % 2 == 0 )
                bmakePage = true;
            else if ( forcePageCount == ForcePageCount.END_ON_ODD
                && firstAvailPageNumber % 2 != 0 )
                bmakePage = true;
            else if ( forcePageCount == ForcePageCount.NO_FORCE )
            {
                // do nothing
            }

            if ( bmakePage )
            {
                try
                {
                    isForcing = true;
                    CurrentPageNumber++;
                    firstAvailPageNumber = CurrentPageNumber;
                    currentPage = MakePage( areaTree, firstAvailPageNumber, false, true );
                    string formattedPageNumber =
                        pageNumberGenerator.makeFormattedPageNumber( CurrentPageNumber );
                    currentPage.setFormattedNumber( formattedPageNumber );
                    currentPage.setPageSequence( this );
                    FormatStaticContent( areaTree );

                    FonetDriver.ActiveDriver.FireFonetInfo(
                        "[forced-" + firstAvailPageNumber + "]" );

                    areaTree.addPage( currentPage );
                    root.setRunningPageNumberCounter( CurrentPageNumber );
                    isForcing = false;
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