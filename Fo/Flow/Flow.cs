using System.Collections;
using Fonet.Fo.Pagination;
using Fonet.Layout;

namespace Fonet.Fo.Flow
{
    internal class Flow : FObj
    {
        private string _flowName;
        private Status _status = new Status( Status.AREA_FULL_NONE );
        private int contentWidth;
        private ArrayList markerSnapshot;

        private readonly PageSequence pageSequence;

        protected Flow( FObj parent, PropertyList propertyList )
            : base( parent, propertyList )
        {
            name = GetElementName();

            if ( parent.GetName().Equals( "fo:page-sequence" ) )
                pageSequence = (PageSequence)parent;
            else
            {
                throw new FonetException( "flow must be child of "
                    + "page-sequence, not "
                    + parent.GetName() );
            }
            SetFlowName( GetProperty( "flow-name" ).GetString() );

            if ( pageSequence.IsFlowSet )
            {
                if ( name.Equals( "fo:flow" ) )
                {
                    throw new FonetException( "Only a single fo:flow permitted"
                        + " per fo:page-sequence" );
                }
                throw new FonetException( name
                    + " not allowed after fo:flow" );
            }
            pageSequence.AddFlow( this );
        }

        public new static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        protected virtual void SetFlowName( string name )
        {
            if ( name == null || name.Equals( "" ) )
            {
                FonetDriver.ActiveDriver.FireFonetWarning(
                    "A 'flow-name' is required for " + GetElementName() + "." );
                _flowName = "xsl-region-body";
            }
            else
                _flowName = name;
        }

        public string GetFlowName()
        {
            return _flowName;
        }

        public override Status Layout( Area area )
        {
            return Layout( area, null );
        }

        public virtual Status Layout( Area area, Region region )
        {
            if ( marker == MarkerStart )
                marker = 0;

            var bac = (BodyAreaContainer)area;

            var prevChildMustKeepWithNext = false;
            ArrayList pageMarker = getMarkerSnapshot( new ArrayList() );

            int numChildren = children.Count;
            if ( numChildren == 0 )
                throw new FonetException( "fo:flow must contain block-level children" );
            for ( int i = marker; i < numChildren; i++ )
            {
                var fo = (FObj)children[ i ];

                if ( bac.isBalancingRequired( fo ) )
                {
                    bac.resetSpanArea();

                    Rollback( markerSnapshot );
                    i = marker - 1;
                    continue;
                }
                Area currentArea = bac.getNextArea( fo );
                currentArea.setIDReferences( bac.getIDReferences() );
                if ( bac.isNewSpanArea() )
                {
                    marker = i;
                    markerSnapshot = getMarkerSnapshot( new ArrayList() );
                }
                SetContentWidth( currentArea.getContentWidth() );

                _status = fo.Layout( currentArea );

                if ( _status.isIncomplete() )
                {
                    if ( prevChildMustKeepWithNext && _status.laidOutNone() )
                    {
                        marker = i - 1;
                        var prevChild = (FObj)children[ marker ];
                        prevChild.RemoveAreas();
                        prevChild.ResetMarker();
                        prevChild.RemoveID( area.getIDReferences() );
                        _status = new Status( Status.AREA_FULL_SOME );
                        return _status;
                    }
                    if ( bac.isLastColumn() )
                    {
                        if ( _status.getCode() == Status.FORCE_COLUMN_BREAK )
                        {
                            marker = i;
                            _status =
                                new Status( Status.FORCE_PAGE_BREAK );
                            return _status;
                        }
                        marker = i;
                        return _status;
                    }
                    if ( _status.isPageBreak() )
                    {
                        marker = i;
                        return _status;
                    }
                    ( (ColumnArea)currentArea ).incrementSpanIndex();
                    i--;
                }
                if ( _status.getCode() == Status.KEEP_WITH_NEXT )
                    prevChildMustKeepWithNext = true;
                else
                    prevChildMustKeepWithNext = false;
            }
            return _status;
        }

        protected void SetContentWidth( int contentWidth )
        {
            this.contentWidth = contentWidth;
        }

        public override int GetContentWidth()
        {
            return contentWidth;
        }

        protected virtual string GetElementName()
        {
            return "fo:flow";
        }

        public Status getStatus()
        {
            return _status;
        }

        public override bool GeneratesReferenceAreas()
        {
            return true;
        }

        internal new class Maker : FObj.Maker
        {
            public override FObj Make( FObj parent, PropertyList propertyList )
            {
                return new Flow( parent, propertyList );
            }
        }
    }
}