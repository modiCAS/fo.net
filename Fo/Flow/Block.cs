using Fonet.Layout;

namespace Fonet.Fo.Flow
{
    internal class Block : FObjMixed
    {
        private int _align;
        private int _alignLast;
        private bool _anythingLaidOut;
        private int _areaHeight;
        private int _blockOrphans;
        private int _blockWidows;
        private int _breakAfter;
        private int _contentWidth;
        private int _endIndent;
        private string _id;
        private int _keepWithNext;
        private int _lineHeight;
        private int _spaceAfter;
        private int _spaceBefore;
        private readonly int _span;
        private int _startIndent;
        private int _textIndent;

        public Block( FObj parent, PropertyList propertyList ) : base( parent, propertyList )
        {
            Name = "fo:block";

            switch ( parent.GetName() )
            {
            case "fo:basic-link":
            case "fo:block":
            case "fo:block-container":
            case "fo:float":
            case "fo:flow":
            case "fo:footnote-body":
            case "fo:inline":
            case "fo:inline-container":
            case "fo:list-item-body":
            case "fo:list-item-label":
            case "fo:marker":
            case "fo:multi-case":
            case "fo:static-content":
            case "fo:table-caption":
            case "fo:table-cell":
            case "fo:wrapper":
                break;
            default:
                throw new FonetException(
                    "fo:block must be child of " +
                        "fo:basic-link, fo:block, fo:block-container, fo:float, fo:flow, fo:footnote-body, fo:inline, fo:inline-container, fo:list-item-body, fo:list-item-label, fo:marker, fo:multi-case, fo:static-content, fo:table-caption, fo:table-cell or fo:wrapper " +
                        "not " + parent.GetName() );
            }
            _span = Properties.GetProperty( "span" ).GetEnum();
            Ts = PropMgr.GetTextDecoration( parent );
        }

        public new static FObj.Maker GetMaker()
        {
            return new Maker();
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
                HyphenationProps mHyphProps = PropMgr.GetHyphenationProps();
                MarginProps mProps = PropMgr.GetMarginProps();
                RelativePositionProps mRelProps = PropMgr.GetRelativePositionProps();

                _align = Properties.GetProperty( "text-align" ).GetEnum();
                _alignLast = Properties.GetProperty( "text-align-last" ).GetEnum();
                _breakAfter = Properties.GetProperty( "break-after" ).GetEnum();
                _lineHeight =
                    Properties.GetProperty( "line-height" ).GetLength().MValue();
                _startIndent =
                    Properties.GetProperty( "start-indent" ).GetLength().MValue();
                _endIndent =
                    Properties.GetProperty( "end-indent" ).GetLength().MValue();
                _spaceBefore =
                    Properties.GetProperty( "space-before.optimum" ).GetLength().MValue();
                _spaceAfter =
                    Properties.GetProperty( "space-after.optimum" ).GetLength().MValue();
                _textIndent =
                    Properties.GetProperty( "text-indent" ).GetLength().MValue();
                _keepWithNext =
                    Properties.GetProperty( "keep-with-next" ).GetEnum();

                _blockWidows =
                    Properties.GetProperty( "widows" ).GetNumber().IntValue();
                _blockOrphans = Properties.GetProperty( "orphans" ).GetNumber().IntValue();
                _id = Properties.GetProperty( "id" ).GetString();

                if ( area is BlockArea )
                    area.End();

                if ( area.GetIDReferences() != null )
                    area.GetIDReferences().CreateID( _id );

                Marker = 0;

                int breakBeforeStatus = PropMgr.CheckBreakBefore( area );
                if ( breakBeforeStatus != Status.Ok )
                    return new Status( breakBeforeStatus );

                int numChildren = Children.Count;
                for ( var i = 0; i < numChildren; i++ )
                {
                    var fo = (FoNode)Children[ i ];
                    if ( fo is FoText )
                    {
                        if ( ( (FoText)fo ).WillCreateArea() )
                        {
                            fo.SetWidows( _blockWidows );
                            break;
                        }
                        Children.RemoveAt( i );
                        numChildren = Children.Count;
                        i--;
                    }
                    else
                    {
                        fo.SetWidows( _blockWidows );
                        break;
                    }
                }

                for ( int i = numChildren - 1; i >= 0; i-- )
                {
                    var fo = (FoNode)Children[ i ];
                    if ( fo is FoText )
                    {
                        if ( ( (FoText)fo ).WillCreateArea() )
                        {
                            fo.SetOrphans( _blockOrphans );
                            break;
                        }
                    }
                    else
                    {
                        fo.SetOrphans( _blockOrphans );
                        break;
                    }
                }
            }

            if ( _spaceBefore != 0 && Marker == 0 )
                area.AddDisplaySpace( _spaceBefore );

            if ( _anythingLaidOut )
                _textIndent = 0;

            if ( Marker == 0 && area.GetIDReferences() != null )
                area.GetIDReferences().ConfigureID( _id, area );

            int spaceLeft = area.SpaceLeft();
            var blockArea = new BlockArea( PropMgr.GetFontState( area.GetFontInfo() ),
                area.GetAllocationWidth(), area.SpaceLeft(),
                _startIndent, _endIndent, _textIndent, _align,
                _alignLast, _lineHeight );
            blockArea.SetGeneratedBy( this );
            AreasGenerated++;
            if ( AreasGenerated == 1 )
                blockArea.IsFirst( true );
            blockArea.AddLineagePair( this, AreasGenerated );
            blockArea.SetParent( area );
            blockArea.SetPage( area.GetPage() );
            blockArea.SetBackground( PropMgr.GetBackgroundProps() );
            blockArea.SetBorderAndPadding( PropMgr.GetBorderAndPadding() );
            blockArea.SetHyphenation( PropMgr.GetHyphenationProps() );
            blockArea.Start();

            blockArea.SetAbsoluteHeight( area.GetAbsoluteHeight() );
            blockArea.SetIDReferences( area.GetIDReferences() );

            blockArea.SetTableCellXOffset( area.GetTableCellXOffset() );

            for ( int i = Marker; i < Children.Count; i++ )
            {
                var fo = (FoNode)Children[ i ];
                Status status;
                if ( ( status = fo.Layout( blockArea ) ).IsIncomplete() )
                {
                    Marker = i;
                    if ( status.GetCode() == Status.AreaFullNone )
                    {
                        if ( i != 0 )
                        {
                            status = new Status( Status.AreaFullSome );
                            area.AddChild( blockArea );
                            area.SetMaxHeight( area.GetMaxHeight() - spaceLeft
                                + blockArea.GetMaxHeight() );
                            area.IncreaseHeight( blockArea.GetHeight() );
                            _anythingLaidOut = true;

                            return status;
                        }
                        _anythingLaidOut = false;
                        return status;
                    }
                    area.AddChild( blockArea );
                    area.SetMaxHeight( area.GetMaxHeight() - spaceLeft
                        + blockArea.GetMaxHeight() );
                    area.IncreaseHeight( blockArea.GetHeight() );
                    _anythingLaidOut = true;
                    return status;
                }
                _anythingLaidOut = true;
            }

            blockArea.End();

            area.SetMaxHeight( area.GetMaxHeight() - spaceLeft
                + blockArea.GetMaxHeight() );

            area.AddChild( blockArea );

            area.IncreaseHeight( blockArea.GetHeight() );

            if ( _spaceAfter != 0 )
                area.AddDisplaySpace( _spaceAfter );

            if ( area is BlockArea )
                area.Start();
            _areaHeight = blockArea.GetHeight();
            _contentWidth = blockArea.GetContentWidth();
            int breakAfterStatus = PropMgr.CheckBreakAfter( area );
            if ( breakAfterStatus != Status.Ok )
            {
                Marker = MarkerBreakAfter;
                blockArea = null;
                return new Status( breakAfterStatus );
            }

            if ( _keepWithNext != 0 )
            {
                blockArea = null;
                return new Status( Status.KeepWithNext );
            }

            blockArea.IsLast( true );
            blockArea = null;
            return new Status( Status.Ok );
        }

        public int GetAreaHeight()
        {
            return _areaHeight;
        }

        public override int GetContentWidth()
        {
            return _contentWidth;
        }

        public int GetSpan()
        {
            return _span;
        }

        public override void ResetMarker()
        {
            _anythingLaidOut = false;
            base.ResetMarker();
        }

        internal new class Maker : FObj.Maker
        {
            public override FObj Make( FObj parent, PropertyList propertyList )
            {
                return new Block( parent, propertyList );
            }
        }
    }
}