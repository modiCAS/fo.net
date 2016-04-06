using Fonet.Fo.Properties;
using Fonet.Layout;
using Fonet.Layout.Inline;

namespace Fonet.Fo.Flow
{
    internal class InstreamForeignObject : FObj
    {
        private ForeignObjectArea _areaCurrent;
        private int _breakAfter;

        private int _breakBefore;
        private bool _chauto;
        private int _contheight;
        private int _contwidth;
        private bool _cwauto;
        private int _endIndent;
        private bool _hauto;
        private int _height;
        private int _scaling;
        private int _spaceAfter;
        private int _spaceBefore;
        private int _startIndent;
        private bool _wauto;
        private int _width;

        public InstreamForeignObject( FObj parent, PropertyList propertyList )
            : base( parent, propertyList )
        {
            Name = "fo:instream-foreign-object";
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
                MarginInlineProps mProps = PropMgr.GetMarginInlineProps();
                RelativePositionProps mRelProps = PropMgr.GetRelativePositionProps();

                string id = Properties.GetProperty( "id" ).GetString();
                int align = Properties.GetProperty( "text-align" ).GetEnum();
                int valign = Properties.GetProperty( "vertical-align" ).GetEnum();
                int overflow = Properties.GetProperty( "overflow" ).GetEnum();

                _breakBefore = Properties.GetProperty( "break-before" ).GetEnum();
                _breakAfter = Properties.GetProperty( "break-after" ).GetEnum();
                _width = Properties.GetProperty( "width" ).GetLength().MValue();
                _height = Properties.GetProperty( "height" ).GetLength().MValue();
                _contwidth =
                    Properties.GetProperty( "content-width" ).GetLength().MValue();
                _contheight =
                    Properties.GetProperty( "content-height" ).GetLength().MValue();
                _wauto = Properties.GetProperty( "width" ).GetLength().IsAuto();
                _hauto = Properties.GetProperty( "height" ).GetLength().IsAuto();
                _cwauto =
                    Properties.GetProperty( "content-width" ).GetLength().IsAuto();
                _chauto =
                    Properties.GetProperty( "content-height" ).GetLength().IsAuto();

                _startIndent =
                    Properties.GetProperty( "start-indent" ).GetLength().MValue();
                _endIndent =
                    Properties.GetProperty( "end-indent" ).GetLength().MValue();
                _spaceBefore =
                    Properties.GetProperty( "space-before.optimum" ).GetLength().MValue();
                _spaceAfter =
                    Properties.GetProperty( "space-after.optimum" ).GetLength().MValue();

                _scaling = Properties.GetProperty( "scaling" ).GetEnum();

                area.GetIDReferences().CreateID( id );
                if ( _areaCurrent == null )
                {
                    _areaCurrent =
                        new ForeignObjectArea( PropMgr.GetFontState( area.GetFontInfo() ),
                            area.GetAllocationWidth() );

                    _areaCurrent.Start();
                    _areaCurrent.SetWidth( _width );
                    _areaCurrent.SetHeight( _height );
                    _areaCurrent.SetContentWidth( _contwidth );
                    _areaCurrent.SetContentHeight( _contheight );
                    _areaCurrent.SetScaling( _scaling );
                    _areaCurrent.SetAlign( align );
                    _areaCurrent.SetVerticalAlign( valign );
                    _areaCurrent.SetOverflow( overflow );
                    _areaCurrent.SetSizeAuto( _wauto, _hauto );
                    _areaCurrent.SetContentSizeAuto( _cwauto, _chauto );

                    _areaCurrent.SetPage( area.GetPage() );

                    int numChildren = Children.Count;
                    if ( numChildren > 1 )
                        throw new FonetException( "Only one child element is allowed in an instream-foreign-object" );
                    if ( Children.Count > 0 )
                    {
                        var fo = (FoNode)Children[ 0 ];
                        Status status;
                        if ( ( status =
                            fo.Layout( _areaCurrent ) ).IsIncomplete() )
                            return status;
                        _areaCurrent.End();
                    }
                }

                Marker = 0;

                if ( _breakBefore == GenericBreak.Enums.Page
                    || _spaceBefore + _areaCurrent.GetEffectiveHeight()
                        > area.SpaceLeft() )
                    return new Status( Status.ForcePageBreak );

                if ( _breakBefore == GenericBreak.Enums.OddPage )
                    return new Status( Status.ForcePageBreakOdd );

                if ( _breakBefore == GenericBreak.Enums.EvenPage )
                    return new Status( Status.ForcePageBreakEven );
            }

            if ( _areaCurrent == null )
                return new Status( Status.Ok );

            if ( area is BlockArea )
            {
                var ba = (BlockArea)area;
                LineArea la = ba.GetCurrentLineArea();
                if ( la == null )
                    return new Status( Status.AreaFullNone );
                la.AddPending();
                if ( _areaCurrent.GetEffectiveWidth() > la.GetRemainingWidth() )
                {
                    la = ba.CreateNextLineArea();
                    if ( la == null )
                        return new Status( Status.AreaFullNone );
                }
                la.AddInlineArea( _areaCurrent, GetLinkSet() );
            }
            else
            {
                area.AddChild( _areaCurrent );
                area.IncreaseHeight( _areaCurrent.GetEffectiveHeight() );
            }

            if ( IsInTableCell )
                _startIndent += ForcedStartOffset;

            _areaCurrent.SetStartIndent( _startIndent );
            _areaCurrent.SetPage( area.GetPage() );

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

            _areaCurrent = null;
            return new Status( Status.Ok );
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