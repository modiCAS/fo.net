using System.Collections;

namespace Fonet.Fo.Pagination
{
    internal class LayoutMasterSet : FObj
    {
        private readonly Hashtable _allRegions;
        private readonly Hashtable _pageSequenceMasters;

        private readonly Root _root;

        private readonly Hashtable _simplePageMasters;

        protected internal LayoutMasterSet( FObj parent, PropertyList propertyList )
            : base( parent, propertyList )
        {
            Name = "fo:layout-master-set";
            _simplePageMasters = new Hashtable();
            _pageSequenceMasters = new Hashtable();

            if ( parent.GetName().Equals( "fo:root" ) )
            {
                _root = (Root)parent;
                _root.SetLayoutMasterSet( this );
            }
            else
            {
                throw new FonetException( "fo:layout-master-set must be child of fo:root, not "
                    + parent.GetName() );
            }
            _allRegions = new Hashtable();
        }

        public new static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        protected internal void AddSimplePageMaster( SimplePageMaster simplePageMaster )
        {
            if ( ExistsName( simplePageMaster.GetMasterName() ) )
            {
                throw new FonetException( "'master-name' ("
                    + simplePageMaster.GetMasterName()
                    + ") must be unique "
                    + "across page-masters and page-sequence-masters" );
            }
            _simplePageMasters.Add( simplePageMaster.GetMasterName(),
                simplePageMaster );
        }

        protected internal SimplePageMaster GetSimplePageMaster( string masterName )
        {
            return (SimplePageMaster)_simplePageMasters[ masterName ];
        }

        protected internal void AddPageSequenceMaster( string masterName, PageSequenceMaster pageSequenceMaster )
        {
            if ( ExistsName( masterName ) )
            {
                throw new FonetException( "'master-name' (" + masterName
                    + ") must be unique "
                    + "across page-masters and page-sequence-masters" );
            }
            _pageSequenceMasters.Add( masterName, pageSequenceMaster );
        }

        protected internal PageSequenceMaster GetPageSequenceMaster( string masterName )
        {
            return (PageSequenceMaster)_pageSequenceMasters[ masterName ];
        }

        private bool ExistsName( string masterName )
        {
            if ( _simplePageMasters.ContainsKey( masterName )
                || _pageSequenceMasters.ContainsKey( masterName ) )
                return true;
            return false;
        }

        protected internal void ResetPageMasters()
        {
            foreach ( PageSequenceMaster psm in _pageSequenceMasters.Values )
                psm.Reset();
        }

        protected internal void CheckRegionNames()
        {
            foreach ( SimplePageMaster spm in _simplePageMasters.Values )
            {
                foreach ( Region region in spm.GetRegions().Values )
                {
                    if ( _allRegions.ContainsKey( region.GetRegionName() ) )
                    {
                        var localClass = (string)_allRegions[ region.GetRegionName() ];
                        if ( !localClass.Equals( region.GetRegionClass() ) )
                        {
                            throw new FonetException( "Duplicate region-names ("
                                + region.GetRegionName()
                                + ") must map "
                                + "to the same region-class ("
                                + localClass + "!="
                                + region.GetRegionClass()
                                + ")" );
                        }
                    }
                    _allRegions[ region.GetRegionName() ] = region.GetRegionClass();
                }
            }
        }

        protected internal bool RegionNameExists( string regionName )
        {
            var result = false;
            foreach ( SimplePageMaster spm in _simplePageMasters.Values )
            {
                result = spm.RegionNameExists( regionName );
                if ( result )
                    return result;
            }
            return result;
        }

        internal new class Maker : FObj.Maker
        {
            public override FObj Make( FObj parent, PropertyList propertyList )
            {
                return new LayoutMasterSet( parent, propertyList );
            }
        }
    }
}