using Fonet.DataTypes;
using Fonet.Fo.Properties;
using Fonet.Layout;

namespace Fonet.Fo.Flow
{
    internal class Character : FObj
    {
        public const int Ok = 0;

        public const int DoesnotFit = 1;

        public Character( FObj parent, PropertyList propertyList )
            : base( parent, propertyList )
        {
            Name = "fo:character";
        }

        public new static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        public override Status Layout( Area area )
        {
            if ( !( area is BlockArea ) )
            {
                FonetDriver.ActiveDriver.FireFonetWarning(
                    "Currently Character can only be in a BlockArea" );
                return new Status( Status.Ok );
            }
            var blockArea = (BlockArea)area;

            AuralProps mAurProps = PropMgr.GetAuralProps();
            BorderAndPadding bap = PropMgr.GetBorderAndPadding();
            BackgroundProps bProps = PropMgr.GetBackgroundProps();
            HyphenationProps mHyphProps = PropMgr.GetHyphenationProps();
            MarginInlineProps mProps = PropMgr.GetMarginInlineProps();
            RelativePositionProps mRelProps = PropMgr.GetRelativePositionProps();
            ColorType c = Properties.GetProperty( "color" ).GetColorType();
            float red = c.Red;
            float green = c.Green;
            float blue = c.Blue;

            int whiteSpaceCollapse =
                Properties.GetProperty( "white-space-collapse" ).GetEnum();
            int wrapOption = Parent.Properties.GetProperty( "wrap-option" ).GetEnum();

            int tmp = Properties.GetProperty( "text-decoration" ).GetEnum();
            bool textDecoration = tmp == TextDecoration.Underline;

            char characterValue = Properties.GetProperty( "character" ).GetCharacter();
            string id = Properties.GetProperty( "id" ).GetString();
            blockArea.GetIDReferences().InitializeID( id, blockArea );

            LineArea la = blockArea.GetCurrentLineArea();
            if ( la == null )
                return new Status( Status.AreaFullNone );
            la.ChangeFont( PropMgr.GetFontState( area.GetFontInfo() ) );
            la.ChangeColor( red, green, blue );
            la.ChangeWrapOption( wrapOption );
            la.ChangeWhiteSpaceCollapse( whiteSpaceCollapse );
            blockArea.SetupLinkSet( GetLinkSet() );
            int result = la.AddCharacter( characterValue, GetLinkSet(),
                textDecoration );
            if ( result == DoesnotFit )
            {
                la = blockArea.CreateNextLineArea();
                if ( la == null )
                    return new Status( Status.AreaFullNone );
                la.ChangeFont( PropMgr.GetFontState( area.GetFontInfo() ) );
                la.ChangeColor( red, green, blue );
                la.ChangeWrapOption( wrapOption );
                la.ChangeWhiteSpaceCollapse( whiteSpaceCollapse );
                blockArea.SetupLinkSet( GetLinkSet() );
                la.AddCharacter( characterValue, GetLinkSet(),
                    textDecoration );
            }
            return new Status( Status.Ok );
        }

        internal new class Maker : FObj.Maker
        {
            public override FObj Make( FObj parent, PropertyList propertyList )
            {
                return new Character( parent, propertyList );
            }
        }
    }
}