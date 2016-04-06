using Fonet.Layout;

namespace Fonet.Fo.Flow
{
    internal class Block : FObjMixed
    {
        private int align;
        private int alignLast;
        private bool anythingLaidOut;
        private int areaHeight;
        private int blockOrphans;
        private int blockWidows;
        private int breakAfter;
        private int contentWidth;
        private int endIndent;
        private string id;
        private int keepWithNext;
        private int lineHeight;
        private int spaceAfter;
        private int spaceBefore;
        private readonly int span;
        private int startIndent;
        private int textIndent;

        public Block( FObj parent, PropertyList propertyList ) : base( parent, propertyList )
        {
            name = "fo:block";

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
            span = properties.GetProperty( "span" ).GetEnum();
            ts = propMgr.getTextDecoration( parent );
        }

        public new static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        public override Status Layout( Area area )
        {
            BlockArea blockArea;

            if ( marker == MarkerBreakAfter )
                return new Status( Status.OK );

            if ( marker == MarkerStart )
            {
                AccessibilityProps mAccProps = propMgr.GetAccessibilityProps();
                AuralProps mAurProps = propMgr.GetAuralProps();
                BorderAndPadding bap = propMgr.GetBorderAndPadding();
                BackgroundProps bProps = propMgr.GetBackgroundProps();
                HyphenationProps mHyphProps = propMgr.GetHyphenationProps();
                MarginProps mProps = propMgr.GetMarginProps();
                RelativePositionProps mRelProps = propMgr.GetRelativePositionProps();

                align = properties.GetProperty( "text-align" ).GetEnum();
                alignLast = properties.GetProperty( "text-align-last" ).GetEnum();
                breakAfter = properties.GetProperty( "break-after" ).GetEnum();
                lineHeight =
                    properties.GetProperty( "line-height" ).GetLength().MValue();
                startIndent =
                    properties.GetProperty( "start-indent" ).GetLength().MValue();
                endIndent =
                    properties.GetProperty( "end-indent" ).GetLength().MValue();
                spaceBefore =
                    properties.GetProperty( "space-before.optimum" ).GetLength().MValue();
                spaceAfter =
                    properties.GetProperty( "space-after.optimum" ).GetLength().MValue();
                textIndent =
                    properties.GetProperty( "text-indent" ).GetLength().MValue();
                keepWithNext =
                    properties.GetProperty( "keep-with-next" ).GetEnum();

                blockWidows =
                    properties.GetProperty( "widows" ).GetNumber().IntValue();
                blockOrphans = properties.GetProperty( "orphans" ).GetNumber().IntValue();
                id = properties.GetProperty( "id" ).GetString();

                if ( area is BlockArea )
                    area.end();

                if ( area.getIDReferences() != null )
                    area.getIDReferences().CreateID( id );

                marker = 0;

                int breakBeforeStatus = propMgr.CheckBreakBefore( area );
                if ( breakBeforeStatus != Status.OK )
                    return new Status( breakBeforeStatus );

                int numChildren = children.Count;
                for ( var i = 0; i < numChildren; i++ )
                {
                    var fo = (FONode)children[ i ];
                    if ( fo is FOText )
                    {
                        if ( ( (FOText)fo ).willCreateArea() )
                        {
                            fo.SetWidows( blockWidows );
                            break;
                        }
                        children.RemoveAt( i );
                        numChildren = children.Count;
                        i--;
                    }
                    else
                    {
                        fo.SetWidows( blockWidows );
                        break;
                    }
                }

                for ( int i = numChildren - 1; i >= 0; i-- )
                {
                    var fo = (FONode)children[ i ];
                    if ( fo is FOText )
                    {
                        if ( ( (FOText)fo ).willCreateArea() )
                        {
                            fo.SetOrphans( blockOrphans );
                            break;
                        }
                    }
                    else
                    {
                        fo.SetOrphans( blockOrphans );
                        break;
                    }
                }
            }

            if ( spaceBefore != 0 && marker == 0 )
                area.addDisplaySpace( spaceBefore );

            if ( anythingLaidOut )
                textIndent = 0;

            if ( marker == 0 && area.getIDReferences() != null )
                area.getIDReferences().ConfigureID( id, area );

            int spaceLeft = area.spaceLeft();
            blockArea =
                new BlockArea( propMgr.GetFontState( area.getFontInfo() ),
                    area.getAllocationWidth(), area.spaceLeft(),
                    startIndent, endIndent, textIndent, align,
                    alignLast, lineHeight );
            blockArea.setGeneratedBy( this );
            areasGenerated++;
            if ( areasGenerated == 1 )
                blockArea.isFirst( true );
            blockArea.addLineagePair( this, areasGenerated );
            blockArea.setParent( area );
            blockArea.setPage( area.getPage() );
            blockArea.setBackground( propMgr.GetBackgroundProps() );
            blockArea.setBorderAndPadding( propMgr.GetBorderAndPadding() );
            blockArea.setHyphenation( propMgr.GetHyphenationProps() );
            blockArea.start();

            blockArea.setAbsoluteHeight( area.getAbsoluteHeight() );
            blockArea.setIDReferences( area.getIDReferences() );

            blockArea.setTableCellXOffset( area.getTableCellXOffset() );

            for ( int i = marker; i < children.Count; i++ )
            {
                var fo = (FONode)children[ i ];
                Status status;
                if ( ( status = fo.Layout( blockArea ) ).isIncomplete() )
                {
                    marker = i;
                    if ( status.getCode() == Status.AREA_FULL_NONE )
                    {
                        if ( i != 0 )
                        {
                            status = new Status( Status.AREA_FULL_SOME );
                            area.addChild( blockArea );
                            area.setMaxHeight( area.getMaxHeight() - spaceLeft
                                + blockArea.getMaxHeight() );
                            area.increaseHeight( blockArea.GetHeight() );
                            anythingLaidOut = true;

                            return status;
                        }
                        anythingLaidOut = false;
                        return status;
                    }
                    area.addChild( blockArea );
                    area.setMaxHeight( area.getMaxHeight() - spaceLeft
                        + blockArea.getMaxHeight() );
                    area.increaseHeight( blockArea.GetHeight() );
                    anythingLaidOut = true;
                    return status;
                }
                anythingLaidOut = true;
            }

            blockArea.end();

            area.setMaxHeight( area.getMaxHeight() - spaceLeft
                + blockArea.getMaxHeight() );

            area.addChild( blockArea );

            area.increaseHeight( blockArea.GetHeight() );

            if ( spaceAfter != 0 )
                area.addDisplaySpace( spaceAfter );

            if ( area is BlockArea )
                area.start();
            areaHeight = blockArea.GetHeight();
            contentWidth = blockArea.getContentWidth();
            int breakAfterStatus = propMgr.CheckBreakAfter( area );
            if ( breakAfterStatus != Status.OK )
            {
                marker = MarkerBreakAfter;
                blockArea = null;
                return new Status( breakAfterStatus );
            }

            if ( keepWithNext != 0 )
            {
                blockArea = null;
                return new Status( Status.KEEP_WITH_NEXT );
            }

            blockArea.isLast( true );
            blockArea = null;
            return new Status( Status.OK );
        }

        public int GetAreaHeight()
        {
            return areaHeight;
        }

        public override int GetContentWidth()
        {
            return contentWidth;
        }

        public int GetSpan()
        {
            return span;
        }

        public override void ResetMarker()
        {
            anythingLaidOut = false;
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