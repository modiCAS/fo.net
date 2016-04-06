using Fonet.Fo.Properties;
using Fonet.Layout;
using Fonet.Layout.Inline;

namespace Fonet.Fo.Flow
{
    internal class InstreamForeignObject : FObj
    {
        private ForeignObjectArea areaCurrent;
        private int breakAfter;

        private int breakBefore;
        private bool chauto;
        private int contheight;
        private int contwidth;
        private bool cwauto;
        private int endIndent;
        private bool hauto;
        private int height;
        private int scaling;
        private int spaceAfter;
        private int spaceBefore;
        private int startIndent;
        private bool wauto;
        private int width;

        public InstreamForeignObject( FObj parent, PropertyList propertyList )
            : base( parent, propertyList )
        {
            name = "fo:instream-foreign-object";
        }

        public new static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        public override Status Layout( Area area )
        {
            if ( marker == MarkerBreakAfter )
                return new Status( Status.OK );

            if ( marker == MarkerStart )
            {
                AccessibilityProps mAccProps = propMgr.GetAccessibilityProps();
                AuralProps mAurProps = propMgr.GetAuralProps();
                BorderAndPadding bap = propMgr.GetBorderAndPadding();
                BackgroundProps bProps = propMgr.GetBackgroundProps();
                MarginInlineProps mProps = propMgr.GetMarginInlineProps();
                RelativePositionProps mRelProps = propMgr.GetRelativePositionProps();

                string id = properties.GetProperty( "id" ).GetString();
                int align = properties.GetProperty( "text-align" ).GetEnum();
                int valign = properties.GetProperty( "vertical-align" ).GetEnum();
                int overflow = properties.GetProperty( "overflow" ).GetEnum();

                breakBefore = properties.GetProperty( "break-before" ).GetEnum();
                breakAfter = properties.GetProperty( "break-after" ).GetEnum();
                width = properties.GetProperty( "width" ).GetLength().MValue();
                height = properties.GetProperty( "height" ).GetLength().MValue();
                contwidth =
                    properties.GetProperty( "content-width" ).GetLength().MValue();
                contheight =
                    properties.GetProperty( "content-height" ).GetLength().MValue();
                wauto = properties.GetProperty( "width" ).GetLength().IsAuto();
                hauto = properties.GetProperty( "height" ).GetLength().IsAuto();
                cwauto =
                    properties.GetProperty( "content-width" ).GetLength().IsAuto();
                chauto =
                    properties.GetProperty( "content-height" ).GetLength().IsAuto();

                startIndent =
                    properties.GetProperty( "start-indent" ).GetLength().MValue();
                endIndent =
                    properties.GetProperty( "end-indent" ).GetLength().MValue();
                spaceBefore =
                    properties.GetProperty( "space-before.optimum" ).GetLength().MValue();
                spaceAfter =
                    properties.GetProperty( "space-after.optimum" ).GetLength().MValue();

                scaling = properties.GetProperty( "scaling" ).GetEnum();

                area.getIDReferences().CreateID( id );
                if ( areaCurrent == null )
                {
                    areaCurrent =
                        new ForeignObjectArea( propMgr.GetFontState( area.getFontInfo() ),
                            area.getAllocationWidth() );

                    areaCurrent.start();
                    areaCurrent.SetWidth( width );
                    areaCurrent.SetHeight( height );
                    areaCurrent.SetContentWidth( contwidth );
                    areaCurrent.setContentHeight( contheight );
                    areaCurrent.setScaling( scaling );
                    areaCurrent.setAlign( align );
                    areaCurrent.setVerticalAlign( valign );
                    areaCurrent.setOverflow( overflow );
                    areaCurrent.setSizeAuto( wauto, hauto );
                    areaCurrent.setContentSizeAuto( cwauto, chauto );

                    areaCurrent.setPage( area.getPage() );

                    int numChildren = children.Count;
                    if ( numChildren > 1 )
                        throw new FonetException( "Only one child element is allowed in an instream-foreign-object" );
                    if ( children.Count > 0 )
                    {
                        var fo = (FONode)children[ 0 ];
                        Status status;
                        if ( ( status =
                            fo.Layout( areaCurrent ) ).isIncomplete() )
                            return status;
                        areaCurrent.end();
                    }
                }

                marker = 0;

                if ( breakBefore == GenericBreak.Enums.PAGE
                    || spaceBefore + areaCurrent.getEffectiveHeight()
                        > area.spaceLeft() )
                    return new Status( Status.FORCE_PAGE_BREAK );

                if ( breakBefore == GenericBreak.Enums.ODD_PAGE )
                    return new Status( Status.FORCE_PAGE_BREAK_ODD );

                if ( breakBefore == GenericBreak.Enums.EVEN_PAGE )
                    return new Status( Status.FORCE_PAGE_BREAK_EVEN );
            }

            if ( areaCurrent == null )
                return new Status( Status.OK );

            if ( area is BlockArea )
            {
                var ba = (BlockArea)area;
                LineArea la = ba.getCurrentLineArea();
                if ( la == null )
                    return new Status( Status.AREA_FULL_NONE );
                la.addPending();
                if ( areaCurrent.getEffectiveWidth() > la.getRemainingWidth() )
                {
                    la = ba.createNextLineArea();
                    if ( la == null )
                        return new Status( Status.AREA_FULL_NONE );
                }
                la.addInlineArea( areaCurrent, GetLinkSet() );
            }
            else
            {
                area.addChild( areaCurrent );
                area.increaseHeight( areaCurrent.getEffectiveHeight() );
            }

            if ( isInTableCell )
                startIndent += forcedStartOffset;

            areaCurrent.setStartIndent( startIndent );
            areaCurrent.setPage( area.getPage() );

            if ( breakAfter == GenericBreak.Enums.PAGE )
            {
                marker = MarkerBreakAfter;
                return new Status( Status.FORCE_PAGE_BREAK );
            }

            if ( breakAfter == GenericBreak.Enums.ODD_PAGE )
            {
                marker = MarkerBreakAfter;
                return new Status( Status.FORCE_PAGE_BREAK_ODD );
            }

            if ( breakAfter == GenericBreak.Enums.EVEN_PAGE )
            {
                marker = MarkerBreakAfter;
                return new Status( Status.FORCE_PAGE_BREAK_EVEN );
            }

            areaCurrent = null;
            return new Status( Status.OK );
        }

        internal new class Maker : FObj.Maker
        {
            public override FObj Make( FObj parent, PropertyList propertyList )
            {
                return new InstreamForeignObject( parent, propertyList );
            }
        }
    }
}