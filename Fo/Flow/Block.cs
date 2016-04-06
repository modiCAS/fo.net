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
            BlockArea blockArea;

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
                    area.end();

                if ( area.getIDReferences() != null )
                    area.getIDReferences().CreateID( _id );

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
                area.addDisplaySpace( _spaceBefore );

            if ( _anythingLaidOut )
                _textIndent = 0;

            if ( Marker == 0 && area.getIDReferences() != null )
                area.getIDReferences().ConfigureID( _id, area );

            int spaceLeft = area.spaceLeft();
            blockArea =
                new BlockArea( PropMgr.GetFontState( area.getFontInfo() ),
                    area.getAllocationWidth(), area.spaceLeft(),
                    _startIndent, _endIndent, _textIndent, _align,
                    _alignLast, _lineHeight );
            blockArea.setGeneratedBy( this );
            AreasGenerated++;
            if ( AreasGenerated == 1 )
                blockArea.isFirst( true );
            blockArea.addLineagePair( this, AreasGenerated );
            blockArea.setParent( area );
            blockArea.setPage( area.getPage() );
            blockArea.setBackground( PropMgr.GetBackgroundProps() );
            blockArea.setBorderAndPadding( PropMgr.GetBorderAndPadding() );
            blockArea.setHyphenation( PropMgr.GetHyphenationProps() );
            blockArea.start();

            blockArea.setAbsoluteHeight( area.getAbsoluteHeight() );
            blockArea.setIDReferences( area.getIDReferences() );

            blockArea.setTableCellXOffset( area.getTableCellXOffset() );

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
                            area.addChild( blockArea );
                            area.setMaxHeight( area.getMaxHeight() - spaceLeft
                                + blockArea.getMaxHeight() );
                            area.increaseHeight( blockArea.GetHeight() );
                            _anythingLaidOut = true;

                            return status;
                        }
                        _anythingLaidOut = false;
                        return status;
                    }
                    area.addChild( blockArea );
                    area.setMaxHeight( area.getMaxHeight() - spaceLeft
                        + blockArea.getMaxHeight() );
                    area.increaseHeight( blockArea.GetHeight() );
                    _anythingLaidOut = true;
                    return status;
                }
                _anythingLaidOut = true;
            }

            blockArea.end();

            area.setMaxHeight( area.getMaxHeight() - spaceLeft
                + blockArea.getMaxHeight() );

            area.addChild( blockArea );

            area.increaseHeight( blockArea.GetHeight() );

            if ( _spaceAfter != 0 )
                area.addDisplaySpace( _spaceAfter );

            if ( area is BlockArea )
                area.start();
            _areaHeight = blockArea.GetHeight();
            _contentWidth = blockArea.getContentWidth();
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

            blockArea.isLast( true );
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