using System.Collections;

namespace Fonet.Fo.Pagination
{
    internal class LayoutMasterSet : FObj
    {
        private readonly Hashtable allRegions;
        private readonly Hashtable pageSequenceMasters;

        private readonly Root root;

        private readonly Hashtable simplePageMasters;

        protected internal LayoutMasterSet( FObj parent, PropertyList propertyList )
            : base( parent, propertyList )
        {
            name = "fo:layout-master-set";
            simplePageMasters = new Hashtable();
            pageSequenceMasters = new Hashtable();

            if ( parent.GetName().Equals( "fo:root" ) )
            {
                root = (Root)parent;
                root.setLayoutMasterSet( this );
            }
            else
            {
                throw new FonetException( "fo:layout-master-set must be child of fo:root, not "
                    + parent.GetName() );
            }
            allRegions = new Hashtable();
        }

        public new static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        protected internal void addSimplePageMaster( SimplePageMaster simplePageMaster )
        {
            if ( existsName( simplePageMaster.GetMasterName() ) )
            {
                throw new FonetException( "'master-name' ("
                    + simplePageMaster.GetMasterName()
                    + ") must be unique "
                    + "across page-masters and page-sequence-masters" );
            }
            simplePageMasters.Add( simplePageMaster.GetMasterName(),
                simplePageMaster );
        }

        protected internal SimplePageMaster getSimplePageMaster( string masterName )
        {
            return (SimplePageMaster)simplePageMasters[ masterName ];
        }

        protected internal void addPageSequenceMaster( string masterName, PageSequenceMaster pageSequenceMaster )
        {
            if ( existsName( masterName ) )
            {
                throw new FonetException( "'master-name' (" + masterName
                    + ") must be unique "
                    + "across page-masters and page-sequence-masters" );
            }
            pageSequenceMasters.Add( masterName, pageSequenceMaster );
        }

        protected internal PageSequenceMaster getPageSequenceMaster( string masterName )
        {
            return (PageSequenceMaster)pageSequenceMasters[ masterName ];
        }

        private bool existsName( string masterName )
        {
            if ( simplePageMasters.ContainsKey( masterName )
                || pageSequenceMasters.ContainsKey( masterName ) )
                return true;
            return false;
        }

        protected internal void resetPageMasters()
        {
            foreach ( PageSequenceMaster psm in pageSequenceMasters.Values )
                psm.Reset();
        }

        protected internal void checkRegionNames()
        {
            foreach ( SimplePageMaster spm in simplePageMasters.Values )
            {
                foreach ( Region region in spm.getRegions().Values )
                {
                    if ( allRegions.ContainsKey( region.getRegionName() ) )
                    {
                        var localClass = (string)allRegions[ region.getRegionName() ];
                        if ( !localClass.Equals( region.GetRegionClass() ) )
                        {
                            throw new FonetException( "Duplicate region-names ("
                                + region.getRegionName()
                                + ") must map "
                                + "to the same region-class ("
                                + localClass + "!="
                                + region.GetRegionClass()
                                + ")" );
                        }
                    }
                    allRegions[ region.getRegionName() ] = region.GetRegionClass();
                }
            }
        }

        protected internal bool regionNameExists( string regionName )
        {
            var result = false;
            foreach ( SimplePageMaster spm in simplePageMasters.Values )
            {
                result = spm.regionNameExists( regionName );
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