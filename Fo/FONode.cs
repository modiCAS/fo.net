using System.Collections;
using Fonet.Layout;

namespace Fonet.Fo
{
    internal abstract class FONode
    {
        public const int MarkerStart = -1000;

        public const int MarkerBreakAfter = -1001;

        protected string areaClass = AreaClass.UNASSIGNED;

        public int areasGenerated = 0;

        protected ArrayList children = new ArrayList();

        protected int forcedStartOffset;

        protected int forcedWidth;

        protected bool isInTableCell;

        protected LinkSet linkSet;

        protected int marker = MarkerStart;

        protected int orphans;
        protected FObj parent;

        protected int widows;

        protected FONode( FObj parent )
        {
            this.parent = parent;

            if ( null != parent )
                areaClass = parent.areaClass;
        }

        public virtual void SetIsInTableCell()
        {
            isInTableCell = true;
            foreach ( FONode child in children )
                child.SetIsInTableCell();
        }

        public virtual void ForceStartOffset( int offset )
        {
            forcedStartOffset = offset;
            foreach ( FONode child in children )
                child.ForceStartOffset( offset );
        }

        public virtual void ForceWidth( int width )
        {
            forcedWidth = width;
            foreach ( FONode child in children )
                child.ForceWidth( width );
        }

        public virtual void ResetMarker()
        {
            marker = MarkerStart;
            foreach ( FONode child in children )
                child.ResetMarker();
        }

        public void SetWidows( int wid )
        {
            widows = wid;
        }

        public void SetOrphans( int orph )
        {
            orphans = orph;
        }

        public void RemoveAreas()
        {
        }

        protected internal virtual void AddChild( FONode child )
        {
            children.Add( child );
        }

        public FObj getParent()
        {
            return parent;
        }

        public virtual void SetLinkSet( LinkSet linkSet )
        {
            this.linkSet = linkSet;
            foreach ( FONode child in children )
                child.SetLinkSet( linkSet );
        }

        public virtual LinkSet GetLinkSet()
        {
            return linkSet;
        }

        public abstract Status Layout( Area area );

        public virtual Property GetProperty( string name )
        {
            return null;
        }

        public virtual ArrayList getMarkerSnapshot( ArrayList snapshot )
        {
            snapshot.Add( marker );

            if ( marker < 0 )
                return snapshot;
            if ( children.Count == 0 )
                return snapshot;
            return ( (FONode)children[ marker ] ).getMarkerSnapshot( snapshot );
        }

        public virtual void Rollback( ArrayList snapshot )
        {
            marker = (int)snapshot[ 0 ];
            snapshot.RemoveAt( 0 );

            if ( marker == MarkerStart )
            {
                ResetMarker();
                return;
            }
            if ( marker == -1 || children.Count == 0 )
                return;

            if ( marker <= MarkerStart )
                return;

            int numChildren = children.Count;
            for ( int i = marker + 1; i < numChildren; i++ )
            {
                var fo = (FONode)children[ i ];
                fo.ResetMarker();
            }
            ( (FONode)children[ marker ] ).Rollback( snapshot );
        }

        public virtual bool MayPrecedeMarker()
        {
            return false;
        }
    }
}