using Fonet.DataTypes;
using Fonet.Fo.Properties;
using Fonet.Image;
using Fonet.Layout;

namespace Fonet.Fo.Flow
{
    internal class ExternalGraphic : FObj
    {
        private int align;
        private readonly int breakAfter = 0;
        private readonly int breakBefore = 0;
        private int endIndent;
        private int height;
        private string id;
        private ImageArea imageArea;
        private int spaceAfter;
        private int spaceBefore;
        private string src;
        private int startIndent;
        private int width;

        public ExternalGraphic( FObj parent, PropertyList propertyList ) : base( parent, propertyList )
        {
            name = "fo:external-graphic";
        }

        public override Status Layout( Area area )
        {
            if ( marker == MarkerStart )
            {
                AccessibilityProps mAccProps = propMgr.GetAccessibilityProps();
                AuralProps mAurProps = propMgr.GetAuralProps();
                BorderAndPadding bap = propMgr.GetBorderAndPadding();
                BackgroundProps bProps = propMgr.GetBackgroundProps();
                MarginInlineProps mProps = propMgr.GetMarginInlineProps();
                RelativePositionProps mRelProps = propMgr.GetRelativePositionProps();

                align = properties.GetProperty( "text-align" ).GetEnum();
                startIndent =
                    properties.GetProperty( "start-indent" ).GetLength().MValue();
                endIndent =
                    properties.GetProperty( "end-indent" ).GetLength().MValue();

                spaceBefore =
                    properties.GetProperty( "space-before.optimum" ).GetLength().MValue();
                spaceAfter =
                    properties.GetProperty( "space-after.optimum" ).GetLength().MValue();

                width = properties.GetProperty( "width" ).GetLength().MValue();
                height = properties.GetProperty( "height" ).GetLength().MValue();

                src = properties.GetProperty( "src" ).GetString();
                id = properties.GetProperty( "id" ).GetString();

                area.getIDReferences().CreateID( id );
                marker = 0;
            }

            try
            {
                FonetImage img = FonetImageFactory.Make( src );
                if ( width == 0 || height == 0 )
                {
                    double imgWidth = img.Width;
                    double imgHeight = img.Height;

                    if ( width == 0 && height == 0 )
                    {
                        width = (int)( imgWidth * 1000d );
                        height = (int)( imgHeight * 1000d );
                    }
                    else if ( height == 0 )
                        height = (int)( imgHeight * width / imgWidth );
                    else if ( width == 0 )
                        width = (int)( imgWidth * height / imgHeight );
                }

                double ratio = width / (double)height;

                Length maxWidth = properties.GetProperty( "max-width" ).GetLength();
                Length maxHeight = properties.GetProperty( "max-height" ).GetLength();

                if ( maxWidth != null && width > maxWidth.MValue() )
                {
                    width = maxWidth.MValue();
                    height = (int)( width / ratio );
                }
                if ( maxHeight != null && height > maxHeight.MValue() )
                {
                    height = maxHeight.MValue();
                    width = (int)( ratio * height );
                }

                int areaWidth = area.getAllocationWidth() - startIndent - endIndent;
                int pageHeight = area.getPage().getBody().getMaxHeight() - spaceBefore;

                if ( height > pageHeight )
                {
                    height = pageHeight;
                    width = (int)( ratio * height );
                }
                if ( width > areaWidth )
                {
                    width = areaWidth;
                    height = (int)( width / ratio );
                }

                if ( area.spaceLeft() < height + spaceBefore )
                    return new Status( Status.AREA_FULL_NONE );

                imageArea =
                    new ImageArea( propMgr.GetFontState( area.getFontInfo() ), img,
                        area.getAllocationWidth(), width, height,
                        startIndent, endIndent, align );

                if ( spaceBefore != 0 && marker == 0 )
                    area.addDisplaySpace( spaceBefore );

                if ( marker == 0 )
                    area.getIDReferences().ConfigureID( id, area );

                imageArea.start();
                imageArea.end();

                if ( spaceAfter != 0 )
                    area.addDisplaySpace( spaceAfter );
                if ( breakBefore == GenericBreak.Enums.PAGE
                    || spaceBefore + imageArea.GetHeight()
                        > area.spaceLeft() )
                    return new Status( Status.FORCE_PAGE_BREAK );

                if ( breakBefore == GenericBreak.Enums.ODD_PAGE )
                    return new Status( Status.FORCE_PAGE_BREAK_ODD );

                if ( breakBefore == GenericBreak.Enums.EVEN_PAGE )
                    return new Status( Status.FORCE_PAGE_BREAK_EVEN );

                if ( area is BlockArea )
                {
                    var ba = (BlockArea)area;
                    LineArea la = ba.getCurrentLineArea();
                    if ( la == null )
                        return new Status( Status.AREA_FULL_NONE );
                    la.addPending();
                    if ( imageArea.getContentWidth() > la.getRemainingWidth() )
                    {
                        la = ba.createNextLineArea();
                        if ( la == null )
                            return new Status( Status.AREA_FULL_NONE );
                    }
                    la.addInlineArea( imageArea, GetLinkSet() );
                }
                else
                {
                    area.addChild( imageArea );
                    area.increaseHeight( imageArea.getContentHeight() );
                }
                imageArea.setPage( area.getPage() );

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
            }
            catch ( FonetImageException imgex )
            {
                FonetDriver.ActiveDriver.FireFonetError( "Error while creating area : " + imgex.Message );
            }

            return new Status( Status.OK );
        }

        public new static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        internal new class Maker : FObj.Maker
        {
            public override FObj Make( FObj parent, PropertyList propertyList )
            {
                return new ExternalGraphic( parent, propertyList );
            }
        }
    }
}