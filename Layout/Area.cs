using System.Collections;
using Fonet.DataTypes;
using Fonet.Fo;
using Fonet.Fo.Flow;
using Fonet.Layout.Inline;

namespace Fonet.Layout
{
    internal abstract class Area : Box
    {
        private bool _isFirst;
        private bool _isLast;
        private int _absoluteYTop;
        protected int AllocationWidth;
        protected string AreaClass = null;
        private BackgroundProps _background;
        private BorderAndPadding _bp;
        protected ArrayList Children = new ArrayList();
        protected int ContentRectangleWidth;
        protected int CurrentHeight;

        public FObj FoCreator;
        protected FontState FontState;
        private FObj _generatedBy;
        private IDReferences _idReferences;
        private ArrayList _markers;
        protected int MaxHeight;
        protected Page Page;
        private Hashtable _returnedBy;
        private int _tableCellXOffset;

        protected Area( FontState fontState )
        {
            SetFontState( fontState );
            _markers = new ArrayList();
            _returnedBy = new Hashtable();
        }

        protected Area( FontState fontState, int allocationWidth, int maxHeight )
        {
            SetFontState( fontState );
            this.AllocationWidth = allocationWidth;
            ContentRectangleWidth = allocationWidth;
            this.MaxHeight = maxHeight;
            _markers = new ArrayList();
            _returnedBy = new Hashtable();
        }

        private void SetFontState( FontState fontState )
        {
            this.FontState = fontState;
        }

        public void AddChild( Box child )
        {
            Children.Add( child );
            child.Parent = this;
        }

        public void AddChildAtStart( Box child )
        {
            Children.Insert( 0, child );
            child.Parent = this;
        }

        public void AddDisplaySpace( int size )
        {
            AddChild( new DisplaySpace( size ) );
            CurrentHeight += size;
        }

        public void AddInlineSpace( int size )
        {
            AddChild( new InlineSpace( size ) );
        }

        public FontInfo GetFontInfo()
        {
            return Page.GetFontInfo();
        }

        public virtual void End()
        {
        }

        public int GetAllocationWidth()
        {
            return AllocationWidth;
        }

        public void SetAllocationWidth( int w )
        {
            AllocationWidth = w;
            ContentRectangleWidth = AllocationWidth;
        }

        public ArrayList GetChildren()
        {
            return Children;
        }

        public bool HasChildren()
        {
            return Children.Count != 0;
        }

        public bool HasNonSpaceChildren()
        {
            if ( Children.Count > 0 )
            {
                foreach ( object child in Children )
                {
                    if ( !( child is DisplaySpace ) )
                        return true;
                }
            }
            return false;
        }

        public virtual int GetContentWidth()
        {
            return ContentRectangleWidth;
        }

        public FontState GetFontState()
        {
            return FontState;
        }

        public virtual int GetContentHeight()
        {
            return CurrentHeight;
        }

        public virtual int GetHeight()
        {
            return CurrentHeight + GetPaddingTop() + GetPaddingBottom()
                + GetBorderTopWidth() + GetBorderBottomWidth();
        }

        public int GetMaxHeight()
        {
            return MaxHeight;
        }

        public Page GetPage()
        {
            return Page;
        }

        public BackgroundProps GetBackground()
        {
            return _background;
        }

        public int GetPaddingTop()
        {
            return _bp == null ? 0 : _bp.GetPaddingTop( false );
        }

        public int GetPaddingLeft()
        {
            return _bp == null ? 0 : _bp.GetPaddingLeft( false );
        }

        public int GetPaddingBottom()
        {
            return _bp == null ? 0 : _bp.GetPaddingBottom( false );
        }

        public int GetPaddingRight()
        {
            return _bp == null ? 0 : _bp.GetPaddingRight( false );
        }

        public int GetBorderTopWidth()
        {
            return _bp == null ? 0 : _bp.GetBorderTopWidth( false );
        }

        public int GetBorderRightWidth()
        {
            return _bp == null ? 0 : _bp.GetBorderRightWidth( false );
        }

        public int GetBorderLeftWidth()
        {
            return _bp == null ? 0 : _bp.GetBorderLeftWidth( false );
        }

        public int GetBorderBottomWidth()
        {
            return _bp == null ? 0 : _bp.GetBorderBottomWidth( false );
        }

        public int GetTableCellXOffset()
        {
            return _tableCellXOffset;
        }

        public void SetTableCellXOffset( int offset )
        {
            _tableCellXOffset = offset;
        }

        public int GetAbsoluteHeight()
        {
            return _absoluteYTop + GetPaddingTop() + GetBorderTopWidth() + CurrentHeight;
        }

        public void SetAbsoluteHeight( int value )
        {
            _absoluteYTop = value;
        }

        public void IncreaseHeight( int amount )
        {
            CurrentHeight += amount;
        }

        public void RemoveChild( Area area )
        {
            CurrentHeight -= area.GetHeight();
            Children.Remove( area );
        }

        public void RemoveChild( DisplaySpace spacer )
        {
            CurrentHeight -= spacer.GetSize();
            Children.Remove( spacer );
        }

        public void Remove()
        {
            Parent.RemoveChild( this );
        }

        public virtual void SetPage( Page page )
        {
            this.Page = page;
        }

        public void SetBackground( BackgroundProps bg )
        {
            _background = bg;
        }

        public void SetBorderAndPadding( BorderAndPadding bp )
        {
            this._bp = bp;
        }

        public virtual int SpaceLeft()
        {
            return MaxHeight - CurrentHeight;
        }

        public virtual void Start()
        {
        }

        public virtual void SetHeight( int height )
        {
            int prevHeight = CurrentHeight;
            if ( height > CurrentHeight )
                CurrentHeight = height;

            if ( CurrentHeight > GetMaxHeight() )
                CurrentHeight = GetMaxHeight();
        }

        public void SetMaxHeight( int height )
        {
            MaxHeight = height;
        }

        public Area GetParent()
        {
            return Parent;
        }

        public void SetParent( Area parent )
        {
            this.Parent = parent;
        }

        public virtual void SetIDReferences( IDReferences idReferences )
        {
            this._idReferences = idReferences;
        }

        public virtual IDReferences GetIDReferences()
        {
            return _idReferences;
        }

        public FObj GetfoCreator()
        {
            return FoCreator;
        }

        public AreaContainer GetNearestAncestorAreaContainer()
        {
            Area area = GetParent();
            while ( area != null && !( area is AreaContainer ) )
                area = area.GetParent();
            return (AreaContainer)area;
        }

        public BorderAndPadding GetBorderAndPadding()
        {
            return _bp;
        }

        public void AddMarker( Marker marker )
        {
            _markers.Add( marker );
        }

        public void AddMarkers( ArrayList markers )
        {
            foreach ( object o in markers )
                this._markers.Add( o );
        }

        public void AddLineagePair( FObj fo, int areaPosition )
        {
            _returnedBy.Add( fo, areaPosition );
        }

        public ArrayList GetMarkers()
        {
            return _markers;
        }

        public void SetGeneratedBy( FObj generatedBy )
        {
            this._generatedBy = generatedBy;
        }

        public FObj GetGeneratedBy()
        {
            return _generatedBy;
        }

        public void IsFirst( bool isFirst )
        {
            _isFirst = isFirst;
        }

        public bool IsFirst()
        {
            return _isFirst;
        }

        public void IsLast( bool isLast )
        {
            _isLast = isLast;
        }

        public bool IsLast()
        {
            return _isLast;
        }
    }
}