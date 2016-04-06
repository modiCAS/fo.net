using System.Collections;
using Fonet.Layout;

namespace Fonet.Fo
{
    internal abstract class FoNode
    {
        public const int MarkerStart = -1000;

        public const int MarkerBreakAfter = -1001;

        protected string AreaClass = Fonet.Layout.AreaClass.UNASSIGNED;

        public int AreasGenerated = 0;

        protected ArrayList Children = new ArrayList();

        protected int ForcedStartOffset;

        protected int ForcedWidth;

        protected bool IsInTableCell;

        protected LinkSet LinkSet;

        protected int Marker = MarkerStart;

        protected int Orphans;
        protected FObj Parent;

        protected int Widows;

        protected FoNode( FObj parent )
        {
            this.Parent = parent;

            if ( null != parent )
                AreaClass = parent.AreaClass;
        }

        public virtual void SetIsInTableCell()
        {
            IsInTableCell = true;
            foreach ( FoNode child in Children )
                child.SetIsInTableCell();
        }

        public virtual void ForceStartOffset( int offset )
        {
            ForcedStartOffset = offset;
            foreach ( FoNode child in Children )
                child.ForceStartOffset( offset );
        }

        public virtual void ForceWidth( int width )
        {
            ForcedWidth = width;
            foreach ( FoNode child in Children )
                child.ForceWidth( width );
        }

        public virtual void ResetMarker()
        {
            Marker = MarkerStart;
            foreach ( FoNode child in Children )
                child.ResetMarker();
        }

        public void SetWidows( int wid )
        {
            Widows = wid;
        }

        public void SetOrphans( int orph )
        {
            Orphans = orph;
        }

        public void RemoveAreas()
        {
        }

        protected internal virtual void AddChild( FoNode child )
        {
            Children.Add( child );
        }

        public FObj GetParent()
        {
            return Parent;
        }

        public virtual void SetLinkSet( LinkSet linkSet )
        {
            this.LinkSet = linkSet;
            foreach ( FoNode child in Children )
                child.SetLinkSet( linkSet );
        }

        public virtual LinkSet GetLinkSet()
        {
            return LinkSet;
        }

        public abstract Status Layout( Area area );

        public virtual Property GetProperty( string name )
        {
            return null;
        }

        public virtual ArrayList GetMarkerSnapshot( ArrayList snapshot )
        {
            snapshot.Add( Marker );

            if ( Marker < 0 )
                return snapshot;
            if ( Children.Count == 0 )
                return snapshot;
            return ( (FoNode)Children[ Marker ] ).GetMarkerSnapshot( snapshot );
        }

        public virtual void Rollback( ArrayList snapshot )
        {
            Marker = (int)snapshot[ 0 ];
            snapshot.RemoveAt( 0 );

            if ( Marker == MarkerStart )
            {
                ResetMarker();
                return;
            }
            if ( Marker == -1 || Children.Count == 0 )
                return;

            if ( Marker <= MarkerStart )
                return;

            int numChildren = Children.Count;
            for ( int i = Marker + 1; i < numChildren; i++ )
            {
                var fo = (FoNode)Children[ i ];
                fo.ResetMarker();
            }
            ( (FoNode)Children[ Marker ] ).Rollback( snapshot );
        }

        public virtual bool MayPrecedeMarker()
        {
            return false;
        }
    }
}