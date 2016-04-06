using System.Collections;
using Fonet.Layout;

namespace Fonet.Fo.Pagination
{
    internal class SimplePageMaster : FObj
    {
        private readonly Hashtable _regions;
        private int afterHeight;
        private bool afterPrecedence;
        private int beforeHeight;
        private bool beforePrecedence;

        private readonly LayoutMasterSet layoutMasterSet;
        private readonly string masterName;
        private PageMaster pageMaster;

        protected SimplePageMaster( FObj parent, PropertyList propertyList )
            : base( parent, propertyList )
        {
            name = "fo:simple-page-master";

            if ( parent.GetName().Equals( "fo:layout-master-set" ) )
            {
                layoutMasterSet = (LayoutMasterSet)parent;
                masterName = properties.GetProperty( "master-name" ).GetString();
                if ( masterName == null )
                {
                    FonetDriver.ActiveDriver.FireFonetWarning(
                        "simple-page-master does not have a master-name and so is being ignored" );
                }
                else
                    layoutMasterSet.addSimplePageMaster( this );
            }
            else
            {
                throw new FonetException( "fo:simple-page-master must be child "
                    + "of fo:layout-master-set, not "
                    + parent.GetName() );
            }
            _regions = new Hashtable();
        }

        public new static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        protected internal override void End()
        {
            int pageWidth =
                properties.GetProperty( "page-width" ).GetLength().MValue();
            int pageHeight =
                properties.GetProperty( "page-height" ).GetLength().MValue();
            MarginProps mProps = propMgr.GetMarginProps();

            int contentRectangleXPosition = mProps.marginLeft;
            int contentRectangleYPosition = pageHeight - mProps.marginTop;
            int contentRectangleWidth = pageWidth - mProps.marginLeft
                - mProps.marginRight;
            int contentRectangleHeight = pageHeight - mProps.marginTop
                - mProps.marginBottom;

            pageMaster = new PageMaster( pageWidth, pageHeight );
            if ( getRegion( RegionBody.REGION_CLASS ) != null )
            {
                var body =
                    (BodyRegionArea)getRegion( RegionBody.REGION_CLASS ).MakeRegionArea( contentRectangleXPosition,
                        contentRectangleYPosition,
                        contentRectangleWidth,
                        contentRectangleHeight );
                pageMaster.addBody( body );
            }
            else
            {
                FonetDriver.ActiveDriver.FireFonetError(
                    "simple-page-master must have a region of class " +
                        RegionBody.REGION_CLASS );
            }

            if ( getRegion( RegionBefore.REGION_CLASS ) != null )
            {
                RegionArea before =
                    getRegion( RegionBefore.REGION_CLASS ).MakeRegionArea( contentRectangleXPosition,
                        contentRectangleYPosition, contentRectangleWidth,
                        contentRectangleHeight );
                pageMaster.addBefore( before );
                beforePrecedence =
                    ( (RegionBefore)getRegion( RegionBefore.REGION_CLASS ) ).getPrecedence();
                beforeHeight = before.GetHeight();
            }
            else
                beforePrecedence = false;

            if ( getRegion( RegionAfter.REGION_CLASS ) != null )
            {
                RegionArea after =
                    getRegion( RegionAfter.REGION_CLASS ).MakeRegionArea( contentRectangleXPosition,
                        contentRectangleYPosition, contentRectangleWidth,
                        contentRectangleHeight );
                pageMaster.addAfter( after );
                afterPrecedence =
                    ( (RegionAfter)getRegion( RegionAfter.REGION_CLASS ) ).getPrecedence();
                afterHeight = after.GetHeight();
            }
            else
                afterPrecedence = false;

            if ( getRegion( RegionStart.REGION_CLASS ) != null )
            {
                RegionArea start =
                    ( (RegionStart)getRegion( RegionStart.REGION_CLASS ) ).MakeRegionArea( contentRectangleXPosition,
                        contentRectangleYPosition, contentRectangleWidth,
                        contentRectangleHeight, beforePrecedence,
                        afterPrecedence, beforeHeight, afterHeight );
                pageMaster.addStart( start );
            }

            if ( getRegion( RegionEnd.REGION_CLASS ) != null )
            {
                RegionArea end =
                    ( (RegionEnd)getRegion( RegionEnd.REGION_CLASS ) ).MakeRegionArea( contentRectangleXPosition,
                        contentRectangleYPosition, contentRectangleWidth,
                        contentRectangleHeight, beforePrecedence,
                        afterPrecedence, beforeHeight, afterHeight );
                pageMaster.addEnd( end );
            }
        }

        public PageMaster getPageMaster()
        {
            return pageMaster;
        }

        public PageMaster GetNextPageMaster()
        {
            return pageMaster;
        }

        public string GetMasterName()
        {
            return masterName;
        }


        protected internal void addRegion( Region region )
        {
            if ( _regions.ContainsKey( region.GetRegionClass() ) )
            {
                throw new FonetException( "Only one region of class "
                    + region.GetRegionClass()
                    + " allowed within a simple-page-master." );
            }
            _regions.Add( region.GetRegionClass(), region );
        }

        protected internal Region getRegion( string regionClass )
        {
            return (Region)_regions[ regionClass ];
        }

        protected internal Hashtable getRegions()
        {
            return _regions;
        }

        protected internal bool regionNameExists( string regionName )
        {
            foreach ( Region r in _regions.Values )
            {
                if ( r.getRegionName().Equals( regionName ) )
                    return true;
            }
            return false;
        }

        internal new class Maker : FObj.Maker
        {
            public override FObj Make( FObj parent, PropertyList propertyList )
            {
                return new SimplePageMaster( parent, propertyList );
            }
        }
    }
}