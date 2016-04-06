using System.Collections;
using Fonet.DataTypes;
using Fonet.Fo.Properties;
using Fonet.Layout;

namespace Fonet.Fo.Flow
{
    internal abstract class AbstractTableBody : FObj
    {
        protected AreaContainer AreaContainer;
        protected ArrayList Columns;
        protected string ID;
        protected RowSpanMgr RowSpanMgr;
        protected int SpaceAfter;
        protected int SpaceBefore;

        protected AbstractTableBody( FObj parent, PropertyList propertyList ) : base( parent, propertyList )
        {
            if ( !( parent is Table ) )
            {
                FonetDriver.ActiveDriver.FireFonetError(
                    "A table body must be child of fo:table, not " + parent.GetName() );
            }
        }

        public void SetColumns( ArrayList columns )
        {
            this.Columns = columns;
        }

        public virtual void SetYPosition( int value )
        {
            AreaContainer.SetYPosition( value );
        }

        public virtual int GetYPosition()
        {
            return AreaContainer.GetCurrentYPosition();
        }

        public int GetHeight()
        {
            return AreaContainer.GetHeight() + SpaceBefore + SpaceAfter;
        }

        public override Status Layout( Area area )
        {
            if ( Marker == MarkerBreakAfter )
                return new Status( Status.Ok );

            if ( Marker == MarkerStart )
            {
                AccessibilityProps mAccProps = PropMgr.GetAccessibilityProps();
                AuralProps mAurProps = PropMgr.GetAuralProps();
                BorderAndPadding bap = PropMgr.GetBorderAndPadding();
                BackgroundProps bProps = PropMgr.GetBackgroundProps();
                RelativePositionProps mRelProps = PropMgr.GetRelativePositionProps();

                SpaceBefore = Properties.GetProperty( "space-before.optimum" ).GetLength().MValue();
                SpaceAfter = Properties.GetProperty( "space-after.optimum" ).GetLength().MValue();
                ID = Properties.GetProperty( "id" ).GetString();

                try
                {
                    area.GetIDReferences().CreateID( ID );
                }
                catch ( FonetException e )
                {
                    throw e;
                }

                if ( area is BlockArea )
                    area.End();

                if ( RowSpanMgr == null )
                    RowSpanMgr = new RowSpanMgr( Columns.Count );

                Marker = 0;
            }

            if ( SpaceBefore != 0 && Marker == 0 )
                area.IncreaseHeight( SpaceBefore );

            if ( Marker == 0 )
                area.GetIDReferences().ConfigureID( ID, area );

            int spaceLeft = area.SpaceLeft();

            AreaContainer =
                new AreaContainer( PropMgr.GetFontState( area.GetFontInfo() ), 0,
                    area.GetContentHeight(),
                    area.GetContentWidth(),
                    area.SpaceLeft(), Position.Relative ) { FoCreator = this };
            AreaContainer.SetPage( area.GetPage() );
            AreaContainer.SetParent( area );
            AreaContainer.SetBackground( PropMgr.GetBackgroundProps() );
            AreaContainer.SetBorderAndPadding( PropMgr.GetBorderAndPadding() );
            AreaContainer.Start();

            AreaContainer.SetAbsoluteHeight( area.GetAbsoluteHeight() );
            AreaContainer.SetIDReferences( area.GetIDReferences() );

            var keepWith = new Hashtable();
            int numChildren = Children.Count;
            TableRow lastRow = null;
            var endKeepGroup = true;
            for ( int i = Marker; i < numChildren; i++ )
            {
                object child = Children[ i ];
                if ( child is Marker )
                {
                    ( (Marker)child ).Layout( area );
                    continue;
                }
                if ( !( child is TableRow ) )
                    throw new FonetException( "Currently only Table Rows are supported in table body, header and footer" );
                var row = (TableRow)child;

                row.SetRowSpanMgr( RowSpanMgr );
                row.SetColumns( Columns );
                row.DoSetup( AreaContainer );
                if ( ( row.GetKeepWithPrevious().GetKeepType() != KeepValue.KeepWithAuto ||
                    row.GetKeepWithNext().GetKeepType() != KeepValue.KeepWithAuto ||
                    row.GetKeepTogether().GetKeepType() != KeepValue.KeepWithAuto ) &&
                    lastRow != null && !keepWith.Contains( lastRow ) )
                    keepWith.Add( lastRow, null );
                else
                {
                    if ( endKeepGroup && keepWith.Count > 0 )
                        keepWith = new Hashtable();
                    if ( endKeepGroup && i > Marker )
                        RowSpanMgr.SetIgnoreKeeps( false );
                }

                bool bRowStartsArea = i == Marker;
                if ( bRowStartsArea == false && keepWith.Count > 0 )
                {
                    if ( Children.IndexOf( keepWith[ 0 ] ) == Marker )
                        bRowStartsArea = true;
                }
                row.SetIgnoreKeepTogether( bRowStartsArea && StartsAc( area ) );
                Status status = row.Layout( AreaContainer );
                if ( status.IsIncomplete() )
                {
                    if ( status.IsPageBreak() )
                    {
                        Marker = i;
                        area.AddChild( AreaContainer );

                        area.IncreaseHeight( AreaContainer.GetHeight() );
                        if ( i == numChildren - 1 )
                        {
                            Marker = MarkerBreakAfter;
                            if ( SpaceAfter != 0 )
                                area.IncreaseHeight( SpaceAfter );
                        }
                        return status;
                    }
                    if ( keepWith.Count > 0
                        && !RowSpanMgr.IgnoreKeeps() )
                    {
                        row.RemoveLayout( AreaContainer );
                        foreach ( TableRow tr in keepWith.Keys )
                        {
                            tr.RemoveLayout( AreaContainer );
                            i--;
                        }
                        if ( i == 0 )
                        {
                            ResetMarker();

                            RowSpanMgr.SetIgnoreKeeps( true );

                            return new Status( Status.AreaFullNone );
                        }
                    }
                    Marker = i;
                    if ( i != 0 && status.GetCode() == Status.AreaFullNone )
                        status = new Status( Status.AreaFullSome );
                    if ( !( i == 0 && AreaContainer.GetContentHeight() <= 0 ) )
                    {
                        area.AddChild( AreaContainer );

                        area.IncreaseHeight( AreaContainer.GetHeight() );
                    }

                    RowSpanMgr.SetIgnoreKeeps( true );

                    return status;
                }
                if ( status.GetCode() == Status.KeepWithNext
                    || RowSpanMgr.HasUnfinishedSpans() )
                {
                    keepWith.Add( row, null );
                    endKeepGroup = false;
                }
                else
                    endKeepGroup = true;
                lastRow = row;
                area.SetMaxHeight( area.GetMaxHeight() - spaceLeft
                    + AreaContainer.GetMaxHeight() );
                spaceLeft = area.SpaceLeft();
            }
            area.AddChild( AreaContainer );
            AreaContainer.End();

            area.IncreaseHeight( AreaContainer.GetHeight() );

            if ( SpaceAfter != 0 )
            {
                area.IncreaseHeight( SpaceAfter );
                area.SetMaxHeight( area.GetMaxHeight() - SpaceAfter );
            }

            if ( area is BlockArea )
                area.Start();

            return new Status( Status.Ok );
        }

        internal void RemoveLayout( Area area )
        {
            if ( AreaContainer != null )
                area.RemoveChild( AreaContainer );
            if ( SpaceBefore != 0 )
                area.IncreaseHeight( -SpaceBefore );
            if ( SpaceAfter != 0 )
                area.IncreaseHeight( -SpaceAfter );
            ResetMarker();
            RemoveID( area.GetIDReferences() );
        }

        private bool StartsAc( Area area )
        {
            Area parent = null;

            while ( ( parent = area.GetParent() ) != null &&
                parent.HasNonSpaceChildren() == false )
            {
                if ( parent is AreaContainer &&
                    ( (AreaContainer)parent ).GetPosition() == Position.Absolute )
                    return true;
                area = parent;
            }
            return false;
        }
    }
}