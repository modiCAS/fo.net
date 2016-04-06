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
            BlockArea blockArea;
            if ( !( area is BlockArea ) )
            {
                FonetDriver.ActiveDriver.FireFonetWarning(
                    "Currently Character can only be in a BlockArea" );
                return new Status( Status.Ok );
            }
            blockArea = (BlockArea)area;
            bool textDecoration;

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
            if ( tmp == TextDecoration.Underline )
                textDecoration = true;
            else
                textDecoration = false;

            char characterValue = Properties.GetProperty( "character" ).GetCharacter();
            string id = Properties.GetProperty( "id" ).GetString();
            blockArea.getIDReferences().InitializeID( id, blockArea );

            LineArea la = blockArea.getCurrentLineArea();
            if ( la == null )
                return new Status( Status.AreaFullNone );
            la.changeFont( PropMgr.GetFontState( area.getFontInfo() ) );
            la.changeColor( red, green, blue );
            la.changeWrapOption( wrapOption );
            la.changeWhiteSpaceCollapse( whiteSpaceCollapse );
            blockArea.setupLinkSet( GetLinkSet() );
            int result = la.addCharacter( characterValue, GetLinkSet(),
                textDecoration );
            if ( result == DoesnotFit )
            {
                la = blockArea.createNextLineArea();
                if ( la == null )
                    return new Status( Status.AreaFullNone );
                la.changeFont( PropMgr.GetFontState( area.getFontInfo() ) );
                la.changeColor( red, green, blue );
                la.changeWrapOption( wrapOption );
                la.changeWhiteSpaceCollapse( whiteSpaceCollapse );
                blockArea.setupLinkSet( GetLinkSet() );
                la.addCharacter( characterValue, GetLinkSet(),
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