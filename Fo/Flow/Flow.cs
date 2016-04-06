using System.Collections;
using Fonet.Fo.Pagination;
using Fonet.Layout;

namespace Fonet.Fo.Flow
{
    internal class Flow : FObj
    {
        private string _flowName;
        private Status _status = new Status( Status.AreaFullNone );
        private int _contentWidth;
        private ArrayList _markerSnapshot;

        protected Flow( FObj parent, PropertyList propertyList )
            : base( parent, propertyList )
        {
            PageSequence pageSequence;
            Name = GetElementName();

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
                if ( Name.Equals( "fo:flow" ) )
                {
                    throw new FonetException( "Only a single fo:flow permitted"
                        + " per fo:page-sequence" );
                }
                throw new FonetException( Name
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
            if ( Marker == MarkerStart )
                Marker = 0;

            var bac = (BodyAreaContainer)area;

            var prevChildMustKeepWithNext = false;
            ArrayList pageMarker = GetMarkerSnapshot( new ArrayList() );

            int numChildren = Children.Count;
            if ( numChildren == 0 )
                throw new FonetException( "fo:flow must contain block-level children" );
            for ( int i = Marker; i < numChildren; i++ )
            {
                var fo = (FObj)Children[ i ];

                if ( bac.IsBalancingRequired( fo ) )
                {
                    bac.ResetSpanArea();

                    Rollback( _markerSnapshot );
                    i = Marker - 1;
                    continue;
                }
                Area currentArea = bac.GetNextArea( fo );
                currentArea.SetIDReferences( bac.GetIDReferences() );
                if ( bac.IsNewSpanArea() )
                {
                    Marker = i;
                    _markerSnapshot = GetMarkerSnapshot( new ArrayList() );
                }
                SetContentWidth( currentArea.GetContentWidth() );

                _status = fo.Layout( currentArea );

                if ( _status.IsIncomplete() )
                {
                    if ( prevChildMustKeepWithNext && _status.LaidOutNone() )
                    {
                        Marker = i - 1;
                        var prevChild = (FObj)Children[ Marker ];
                        prevChild.RemoveAreas();
                        prevChild.ResetMarker();
                        prevChild.RemoveID( area.GetIDReferences() );
                        _status = new Status( Status.AreaFullSome );
                        return _status;
                    }
                    if ( bac.IsLastColumn() )
                    {
                        if ( _status.GetCode() == Status.ForceColumnBreak )
                        {
                            Marker = i;
                            _status =
                                new Status( Status.ForcePageBreak );
                            return _status;
                        }
                        Marker = i;
                        return _status;
                    }
                    if ( _status.IsPageBreak() )
                    {
                        Marker = i;
                        return _status;
                    }
                    ( (ColumnArea)currentArea ).IncrementSpanIndex();
                    i--;
                }
                prevChildMustKeepWithNext = _status.GetCode() == Status.KeepWithNext;
            }
            return _status;
        }

        protected void SetContentWidth( int contentWidth )
        {
            this._contentWidth = contentWidth;
        }

        public override int GetContentWidth()
        {
            return _contentWidth;
        }

        protected virtual string GetElementName()
        {
            return "fo:flow";
        }

        public Status GetStatus()
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