using Fonet.Layout;

namespace Fonet.Fo.Pagination
{
    internal abstract class Region : FObj
    {
        public const string PropRegionName = "region-name";

        private readonly SimplePageMaster _layoutMaster;
        private string _regionName;

        protected Region( FObj parent, PropertyList propertyList )
            : base( parent, propertyList )
        {
            Name = GetElementName();

            if ( null == Properties.GetProperty( PropRegionName ) )
                SetRegionName( GetDefaultRegionName() );
            else if ( Properties.GetProperty( PropRegionName ).GetString().Equals( "" ) )
                SetRegionName( GetDefaultRegionName() );
            else
            {
                SetRegionName( Properties.GetProperty( PropRegionName ).GetString() );
                if ( IsReserved( GetRegionName() )
                    && !GetRegionName().Equals( GetDefaultRegionName() ) )
                {
                    throw new FonetException( PropRegionName + " '" + _regionName
                        + "' for " + Name
                        + " not permitted." );
                }
            }

            if ( parent.GetName().Equals( "fo:simple-page-master" ) )
            {
                _layoutMaster = (SimplePageMaster)parent;
                GetPageMaster().AddRegion( this );
            }
            else
            {
                throw new FonetException( GetElementName() + " must be child "
                    + "of simple-page-master, not "
                    + parent.GetName() );
            }
        }

        public abstract RegionArea MakeRegionArea( int allocationRectangleXPosition,
            int allocationRectangleYPosition,
            int allocationRectangleWidth,
            int allocationRectangleHeight );

        protected abstract string GetDefaultRegionName();

        protected abstract string GetElementName();

        public abstract string GetRegionClass();

        public string GetRegionName()
        {
            return _regionName;
        }

        private void SetRegionName( string name )
        {
            _regionName = name;
        }

        protected SimplePageMaster GetPageMaster()
        {
            return _layoutMaster;
        }

        protected bool IsReserved( string name )
        {
            return name.Equals( "xsl-region-before" )
                || name.Equals( "xsl-region-start" )
                || name.Equals( "xsl-region-end" )
                || name.Equals( "xsl-region-after" )
                || name.Equals( "xsl-before-float-separator" )
                || name.Equals( "xsl-footnote-separator" );
        }

        public override bool GeneratesReferenceAreas()
        {
            return true;
        }
    }
}