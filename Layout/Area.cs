using System.Collections;
using Fonet.DataTypes;
using Fonet.Fo;
using Fonet.Fo.Flow;
using Fonet.Layout.Inline;

namespace Fonet.Layout
{
    internal abstract class Area : Box
    {
        protected bool _isFirst;
        protected bool _isLast;
        private int absoluteYTop;
        protected int allocationWidth;
        protected string areaClass = null;
        protected BackgroundProps background;
        protected BorderAndPadding bp;
        protected ArrayList children = new ArrayList();
        protected int contentRectangleWidth;
        protected int currentHeight;

        public FObj foCreator;
        protected FontState fontState;
        protected FObj generatedBy;
        private IDReferences idReferences;
        protected ArrayList markers;
        protected int maxHeight;
        protected Page page;
        protected Hashtable returnedBy;
        protected int tableCellXOffset;

        public Area( FontState fontState )
        {
            setFontState( fontState );
            markers = new ArrayList();
            returnedBy = new Hashtable();
        }

        public Area( FontState fontState, int allocationWidth, int maxHeight )
        {
            setFontState( fontState );
            this.allocationWidth = allocationWidth;
            contentRectangleWidth = allocationWidth;
            this.maxHeight = maxHeight;
            markers = new ArrayList();
            returnedBy = new Hashtable();
        }

        private void setFontState( FontState fontState )
        {
            this.fontState = fontState;
        }

        public void addChild( Box child )
        {
            children.Add( child );
            child.parent = this;
        }

        public void addChildAtStart( Box child )
        {
            children.Insert( 0, child );
            child.parent = this;
        }

        public void addDisplaySpace( int size )
        {
            addChild( new DisplaySpace( size ) );
            currentHeight += size;
        }

        public void addInlineSpace( int size )
        {
            addChild( new InlineSpace( size ) );
        }

        public FontInfo getFontInfo()
        {
            return page.getFontInfo();
        }

        public virtual void end()
        {
        }

        public int getAllocationWidth()
        {
            return allocationWidth;
        }

        public void setAllocationWidth( int w )
        {
            allocationWidth = w;
            contentRectangleWidth = allocationWidth;
        }

        public ArrayList getChildren()
        {
            return children;
        }

        public bool hasChildren()
        {
            return children.Count != 0;
        }

        public bool hasNonSpaceChildren()
        {
            if ( children.Count > 0 )
            {
                foreach ( object child in children )
                {
                    if ( !( child is DisplaySpace ) )
                        return true;
                }
            }
            return false;
        }

        public virtual int getContentWidth()
        {
            return contentRectangleWidth;
        }

        public FontState GetFontState()
        {
            return fontState;
        }

        public virtual int getContentHeight()
        {
            return currentHeight;
        }

        public virtual int GetHeight()
        {
            return currentHeight + getPaddingTop() + getPaddingBottom()
                + getBorderTopWidth() + getBorderBottomWidth();
        }

        public int getMaxHeight()
        {
            return maxHeight;
        }

        public Page getPage()
        {
            return page;
        }

        public BackgroundProps getBackground()
        {
            return background;
        }

        public int getPaddingTop()
        {
            return bp == null ? 0 : bp.getPaddingTop( false );
        }

        public int getPaddingLeft()
        {
            return bp == null ? 0 : bp.getPaddingLeft( false );
        }

        public int getPaddingBottom()
        {
            return bp == null ? 0 : bp.getPaddingBottom( false );
        }

        public int getPaddingRight()
        {
            return bp == null ? 0 : bp.getPaddingRight( false );
        }

        public int getBorderTopWidth()
        {
            return bp == null ? 0 : bp.getBorderTopWidth( false );
        }

        public int getBorderRightWidth()
        {
            return bp == null ? 0 : bp.getBorderRightWidth( false );
        }

        public int getBorderLeftWidth()
        {
            return bp == null ? 0 : bp.getBorderLeftWidth( false );
        }

        public int getBorderBottomWidth()
        {
            return bp == null ? 0 : bp.getBorderBottomWidth( false );
        }

        public int getTableCellXOffset()
        {
            return tableCellXOffset;
        }

        public void setTableCellXOffset( int offset )
        {
            tableCellXOffset = offset;
        }

        public int getAbsoluteHeight()
        {
            return absoluteYTop + getPaddingTop() + getBorderTopWidth() + currentHeight;
        }

        public void setAbsoluteHeight( int value )
        {
            absoluteYTop = value;
        }

        public void increaseHeight( int amount )
        {
            currentHeight += amount;
        }

        public void removeChild( Area area )
        {
            currentHeight -= area.GetHeight();
            children.Remove( area );
        }

        public void removeChild( DisplaySpace spacer )
        {
            currentHeight -= spacer.getSize();
            children.Remove( spacer );
        }

        public void remove()
        {
            parent.removeChild( this );
        }

        public virtual void setPage( Page page )
        {
            this.page = page;
        }

        public void setBackground( BackgroundProps bg )
        {
            background = bg;
        }

        public void setBorderAndPadding( BorderAndPadding bp )
        {
            this.bp = bp;
        }

        public virtual int spaceLeft()
        {
            return maxHeight - currentHeight;
        }

        public virtual void start()
        {
        }

        public virtual void SetHeight( int height )
        {
            int prevHeight = currentHeight;
            if ( height > currentHeight )
                currentHeight = height;

            if ( currentHeight > getMaxHeight() )
                currentHeight = getMaxHeight();
        }

        public void setMaxHeight( int height )
        {
            maxHeight = height;
        }

        public Area getParent()
        {
            return parent;
        }

        public void setParent( Area parent )
        {
            this.parent = parent;
        }

        public virtual void setIDReferences( IDReferences idReferences )
        {
            this.idReferences = idReferences;
        }

        public virtual IDReferences getIDReferences()
        {
            return idReferences;
        }

        public FObj getfoCreator()
        {
            return foCreator;
        }

        public AreaContainer getNearestAncestorAreaContainer()
        {
            Area area = getParent();
            while ( area != null && !( area is AreaContainer ) )
                area = area.getParent();
            return (AreaContainer)area;
        }

        public BorderAndPadding GetBorderAndPadding()
        {
            return bp;
        }

        public void addMarker( Marker marker )
        {
            markers.Add( marker );
        }

        public void addMarkers( ArrayList markers )
        {
            foreach ( object o in markers )
                this.markers.Add( o );
        }

        public void addLineagePair( FObj fo, int areaPosition )
        {
            returnedBy.Add( fo, areaPosition );
        }

        public ArrayList getMarkers()
        {
            return markers;
        }

        public void setGeneratedBy( FObj generatedBy )
        {
            this.generatedBy = generatedBy;
        }

        public FObj getGeneratedBy()
        {
            return generatedBy;
        }

        public void isFirst( bool isFirst )
        {
            _isFirst = isFirst;
        }

        public bool isFirst()
        {
            return _isFirst;
        }

        public void isLast( bool isLast )
        {
            _isLast = isLast;
        }

        public bool isLast()
        {
            return _isLast;
        }
    }
}