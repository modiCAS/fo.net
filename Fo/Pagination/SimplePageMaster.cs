using System.Collections;
using Fonet.Layout;

namespace Fonet.Fo.Pagination
{
    internal class SimplePageMaster : FObj
    {
        private readonly Hashtable _regions;
        private int _afterHeight;
        private bool _afterPrecedence;
        private int _beforeHeight;
        private bool _beforePrecedence;

        private readonly string _masterName;
        private PageMaster _pageMaster;

        protected SimplePageMaster( FObj parent, PropertyList propertyList )
            : base( parent, propertyList )
        {
            Name = "fo:simple-page-master";

            if ( parent.GetName().Equals( "fo:layout-master-set" ) )
            {
                var layoutMasterSet = (LayoutMasterSet)parent;
                _masterName = Properties.GetProperty( "master-name" ).GetString();
                if ( _masterName == null )
                {
                    FonetDriver.ActiveDriver.FireFonetWarning(
                        "simple-page-master does not have a master-name and so is being ignored" );
                }
                else
                    layoutMasterSet.AddSimplePageMaster( this );
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
                Properties.GetProperty( "page-width" ).GetLength().MValue();
            int pageHeight =
                Properties.GetProperty( "page-height" ).GetLength().MValue();
            MarginProps mProps = PropMgr.GetMarginProps();

            int contentRectangleXPosition = mProps.MarginLeft;
            int contentRectangleYPosition = pageHeight - mProps.MarginTop;
            int contentRectangleWidth = pageWidth - mProps.MarginLeft
                - mProps.MarginRight;
            int contentRectangleHeight = pageHeight - mProps.MarginTop
                - mProps.MarginBottom;

            _pageMaster = new PageMaster( pageWidth, pageHeight );
            if ( GetRegion( RegionBody.RegionClass ) != null )
            {
                var body =
                    (BodyRegionArea)GetRegion( RegionBody.RegionClass ).MakeRegionArea( contentRectangleXPosition,
                        contentRectangleYPosition,
                        contentRectangleWidth,
                        contentRectangleHeight );
                _pageMaster.AddBody( body );
            }
            else
            {
                FonetDriver.ActiveDriver.FireFonetError(
                    "simple-page-master must have a region of class " +
                        RegionBody.RegionClass );
            }

            if ( GetRegion( RegionBefore.RegionClass ) != null )
            {
                RegionArea before =
                    GetRegion( RegionBefore.RegionClass ).MakeRegionArea( contentRectangleXPosition,
                        contentRectangleYPosition, contentRectangleWidth,
                        contentRectangleHeight );
                _pageMaster.AddBefore( before );
                _beforePrecedence =
                    ( (RegionBefore)GetRegion( RegionBefore.RegionClass ) ).GetPrecedence();
                _beforeHeight = before.GetHeight();
            }
            else
                _beforePrecedence = false;

            if ( GetRegion( RegionAfter.RegionClass ) != null )
            {
                RegionArea after =
                    GetRegion( RegionAfter.RegionClass ).MakeRegionArea( contentRectangleXPosition,
                        contentRectangleYPosition, contentRectangleWidth,
                        contentRectangleHeight );
                _pageMaster.AddAfter( after );
                _afterPrecedence =
                    ( (RegionAfter)GetRegion( RegionAfter.RegionClass ) ).GetPrecedence();
                _afterHeight = after.GetHeight();
            }
            else
                _afterPrecedence = false;

            if ( GetRegion( RegionStart.RegionClass ) != null )
            {
                RegionArea start =
                    ( (RegionStart)GetRegion( RegionStart.RegionClass ) ).MakeRegionArea( contentRectangleXPosition,
                        contentRectangleYPosition, contentRectangleWidth,
                        contentRectangleHeight, _beforePrecedence,
                        _afterPrecedence, _beforeHeight, _afterHeight );
                _pageMaster.AddStart( start );
            }

            if ( GetRegion( RegionEnd.RegionClass ) != null )
            {
                RegionArea end =
                    ( (RegionEnd)GetRegion( RegionEnd.RegionClass ) ).MakeRegionArea( contentRectangleXPosition,
                        contentRectangleYPosition, contentRectangleWidth,
                        contentRectangleHeight, _beforePrecedence,
                        _afterPrecedence, _beforeHeight, _afterHeight );
                _pageMaster.AddEnd( end );
            }
        }

        public PageMaster GetPageMaster()
        {
            return _pageMaster;
        }

        public PageMaster GetNextPageMaster()
        {
            return _pageMaster;
        }

        public string GetMasterName()
        {
            return _masterName;
        }


        protected internal void AddRegion( Region region )
        {
            if ( _regions.ContainsKey( region.GetRegionClass() ) )
            {
                throw new FonetException( "Only one region of class "
                    + region.GetRegionClass()
                    + " allowed within a simple-page-master." );
            }
            _regions.Add( region.GetRegionClass(), region );
        }

        protected internal Region GetRegion( string regionClass )
        {
            return (Region)_regions[ regionClass ];
        }

        protected internal Hashtable GetRegions()
        {
            return _regions;
        }

        protected internal bool RegionNameExists( string regionName )
        {
            foreach ( Region r in _regions.Values )
            {
                if ( r.GetRegionName().Equals( regionName ) )
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