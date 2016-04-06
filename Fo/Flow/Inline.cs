using Fonet.Layout;

namespace Fonet.Fo.Flow
{
    internal class Inline : FObjMixed
    {
        public Inline( FObj parent, PropertyList propertyList )
            : base( parent, propertyList )
        {
            Name = "fo:inline";
            if ( parent.GetName().Equals( "fo:flow" ) )
            {
                throw new FonetException( "inline formatting objects cannot"
                    + " be directly under flow" );
            }

            AccessibilityProps mAccProps = PropMgr.GetAccessibilityProps();
            AuralProps mAurProps = PropMgr.GetAuralProps();
            BorderAndPadding bap = PropMgr.GetBorderAndPadding();
            BackgroundProps bProps = PropMgr.GetBackgroundProps();
            MarginInlineProps mProps = PropMgr.GetMarginInlineProps();
            RelativePositionProps mRelProps = PropMgr.GetRelativePositionProps();
            Ts = PropMgr.GetTextDecoration( parent );
        }

        public new static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        protected internal override void AddCharacters( char[] data, int start, int length )
        {
            var ft = new FoText( data, start, length, this );
            ft.SetUnderlined( Ts.GetUnderlined() );
            ft.SetOverlined( Ts.GetOverlined() );
            ft.SetLineThrough( Ts.GetLineThrough() );
            Children.Add( ft );
        }

        internal new class Maker : FObj.Maker
        {
            public override FObj Make( FObj parent, PropertyList propertyList )
            {
                return new Inline( parent, propertyList );
            }
        }
    }
}