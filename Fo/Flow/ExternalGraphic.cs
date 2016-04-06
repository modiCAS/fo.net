using Fonet.DataTypes;
using Fonet.Fo.Properties;
using Fonet.Image;
using Fonet.Layout;

namespace Fonet.Fo.Flow
{
    internal class ExternalGraphic : FObj
    {
        private int _align;
        private readonly int _breakAfter = 0;
        private readonly int _breakBefore = 0;
        private int _endIndent;
        private int _height;
        private string _id;
        private ImageArea _imageArea;
        private int _spaceAfter;
        private int _spaceBefore;
        private string _src;
        private int _startIndent;
        private int _width;

        public ExternalGraphic( FObj parent, PropertyList propertyList ) : base( parent, propertyList )
        {
            Name = "fo:external-graphic";
        }

        public override Status Layout( Area area )
        {
            if ( Marker == MarkerStart )
            {
                AccessibilityProps mAccProps = PropMgr.GetAccessibilityProps();
                AuralProps mAurProps = PropMgr.GetAuralProps();
                BorderAndPadding bap = PropMgr.GetBorderAndPadding();
                BackgroundProps bProps = PropMgr.GetBackgroundProps();
                MarginInlineProps mProps = PropMgr.GetMarginInlineProps();
                RelativePositionProps mRelProps = PropMgr.GetRelativePositionProps();

                _align = Properties.GetProperty( "text-align" ).GetEnum();
                _startIndent =
                    Properties.GetProperty( "start-indent" ).GetLength().MValue();
                _endIndent =
                    Properties.GetProperty( "end-indent" ).GetLength().MValue();

                _spaceBefore =
                    Properties.GetProperty( "space-before.optimum" ).GetLength().MValue();
                _spaceAfter =
                    Properties.GetProperty( "space-after.optimum" ).GetLength().MValue();

                _width = Properties.GetProperty( "width" ).GetLength().MValue();
                _height = Properties.GetProperty( "height" ).GetLength().MValue();

                _src = Properties.GetProperty( "src" ).GetString();
                _id = Properties.GetProperty( "id" ).GetString();

                area.getIDReferences().CreateID( _id );
                Marker = 0;
            }

            try
            {
                FonetImage img = FonetImageFactory.Make( _src );
                if ( _width == 0 || _height == 0 )
                {
                    double imgWidth = img.Width;
                    double imgHeight = img.Height;

                    if ( _width == 0 && _height == 0 )
                    {
                        _width = (int)( imgWidth * 1000d );
                        _height = (int)( imgHeight * 1000d );
                    }
                    else if ( _height == 0 )
                        _height = (int)( imgHeight * _width / imgWidth );
                    else if ( _width == 0 )
                        _width = (int)( imgWidth * _height / imgHeight );
                }

                double ratio = _width / (double)_height;

                Length maxWidth = Properties.GetProperty( "max-width" ).GetLength();
                Length maxHeight = Properties.GetProperty( "max-height" ).GetLength();

                if ( maxWidth != null && _width > maxWidth.MValue() )
                {
                    _width = maxWidth.MValue();
                    _height = (int)( _width / ratio );
                }
                if ( maxHeight != null && _height > maxHeight.MValue() )
                {
                    _height = maxHeight.MValue();
                    _width = (int)( ratio * _height );
                }

                int areaWidth = area.getAllocationWidth() - _startIndent - _endIndent;
                int pageHeight = area.getPage().getBody().getMaxHeight() - _spaceBefore;

                if ( _height > pageHeight )
                {
                    _height = pageHeight;
                    _width = (int)( ratio * _height );
                }
                if ( _width > areaWidth )
                {
                    _width = areaWidth;
                    _height = (int)( _width / ratio );
                }

                if ( area.spaceLeft() < _height + _spaceBefore )
                    return new Status( Status.AreaFullNone );

                _imageArea =
                    new ImageArea( PropMgr.GetFontState( area.getFontInfo() ), img,
                        area.getAllocationWidth(), _width, _height,
                        _startIndent, _endIndent, _align );

                if ( _spaceBefore != 0 && Marker == 0 )
                    area.addDisplaySpace( _spaceBefore );

                if ( Marker == 0 )
                    area.getIDReferences().ConfigureID( _id, area );

                _imageArea.start();
                _imageArea.end();

                if ( _spaceAfter != 0 )
                    area.addDisplaySpace( _spaceAfter );
                if ( _breakBefore == GenericBreak.Enums.Page
                    || _spaceBefore + _imageArea.GetHeight()
                        > area.spaceLeft() )
                    return new Status( Status.ForcePageBreak );

                if ( _breakBefore == GenericBreak.Enums.OddPage )
                    return new Status( Status.ForcePageBreakOdd );

                if ( _breakBefore == GenericBreak.Enums.EvenPage )
                    return new Status( Status.ForcePageBreakEven );

                if ( area is BlockArea )
                {
                    var ba = (BlockArea)area;
                    LineArea la = ba.getCurrentLineArea();
                    if ( la == null )
                        return new Status( Status.AreaFullNone );
                    la.addPending();
                    if ( _imageArea.getContentWidth() > la.getRemainingWidth() )
                    {
                        la = ba.createNextLineArea();
                        if ( la == null )
                            return new Status( Status.AreaFullNone );
                    }
                    la.addInlineArea( _imageArea, GetLinkSet() );
                }
                else
                {
                    area.addChild( _imageArea );
                    area.increaseHeight( _imageArea.getContentHeight() );
                }
                _imageArea.setPage( area.getPage() );

                if ( _breakAfter == GenericBreak.Enums.Page )
                {
                    Marker = MarkerBreakAfter;
                    return new Status( Status.ForcePageBreak );
                }

                if ( _breakAfter == GenericBreak.Enums.OddPage )
                {
                    Marker = MarkerBreakAfter;
                    return new Status( Status.ForcePageBreakOdd );
                }

                if ( _breakAfter == GenericBreak.Enums.EvenPage )
                {
                    Marker = MarkerBreakAfter;
                    return new Status( Status.ForcePageBreakEven );
                }
            }
            catch ( FonetImageException imgex )
            {
                FonetDriver.ActiveDriver.FireFonetError( "Error while creating area : " + imgex.Message );
            }

            return new Status( Status.Ok );
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