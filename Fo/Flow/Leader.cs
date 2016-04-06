using Fonet.DataTypes;
using Fonet.Layout;

namespace Fonet.Fo.Flow
{
    internal class Leader : FObjMixed
    {
        public Leader( FObj parent, PropertyList propertyList )
            : base( parent, propertyList )
        {
            Name = "fo:leader";
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
                    "fo:leader must be a direct child of fo:block " );
                return new Status( Status.Ok );
            }
            blockArea = (BlockArea)area;

            AccessibilityProps mAccProps = PropMgr.GetAccessibilityProps();
            AuralProps mAurProps = PropMgr.GetAuralProps();
            BorderAndPadding bap = PropMgr.GetBorderAndPadding();
            BackgroundProps bProps = PropMgr.GetBackgroundProps();
            MarginInlineProps mProps = PropMgr.GetMarginInlineProps();
            RelativePositionProps mRelProps = PropMgr.GetRelativePositionProps();
            ColorType c = Properties.GetProperty( "color" ).GetColorType();
            float red = c.Red;
            float green = c.Green;
            float blue = c.Blue;

            int leaderPattern = Properties.GetProperty( "leader-pattern" ).GetEnum();
            int leaderLengthOptimum =
                Properties.GetProperty( "leader-length.optimum" ).GetLength().MValue();
            int leaderLengthMinimum =
                Properties.GetProperty( "leader-length.minimum" ).GetLength().MValue();
            Length maxlength = Properties.GetProperty( "leader-length.maximum" ).GetLength();
            int leaderLengthMaximum;
            if ( maxlength is PercentLength )
            {
                leaderLengthMaximum = (int)( ( (PercentLength)maxlength ).Value()
                    * area.getAllocationWidth() );
            }
            else
                leaderLengthMaximum = maxlength.MValue();
            int ruleThickness =
                Properties.GetProperty( "rule-thickness" ).GetLength().MValue();
            int ruleStyle = Properties.GetProperty( "rule-style" ).GetEnum();
            int leaderPatternWidth =
                Properties.GetProperty( "leader-pattern-width" ).GetLength().MValue();
            int leaderAlignment =
                Properties.GetProperty( "leader-alignment" ).GetEnum();

            string id = Properties.GetProperty( "id" ).GetString();
            blockArea.getIDReferences().InitializeID( id, blockArea );

            int succeeded = AddLeader( blockArea,
                PropMgr.GetFontState( area.getFontInfo() ),
                red, green, blue, leaderPattern,
                leaderLengthMinimum, leaderLengthOptimum,
                leaderLengthMaximum, ruleThickness,
                ruleStyle, leaderPatternWidth,
                leaderAlignment );
            if ( succeeded == 1 )
                return new Status( Status.Ok );
            return new Status( Status.AreaFullSome );
        }

        public int AddLeader( BlockArea ba, FontState fontState, float red,
            float green, float blue, int leaderPattern,
            int leaderLengthMinimum, int leaderLengthOptimum,
            int leaderLengthMaximum, int ruleThickness,
            int ruleStyle, int leaderPatternWidth,
            int leaderAlignment )
        {
            LineArea la = ba.getCurrentLineArea();
            if ( la == null )
                return -1;

            la.changeFont( fontState );
            la.changeColor( red, green, blue );

            if ( leaderLengthOptimum <= la.getRemainingWidth() )
            {
                la.AddLeader( leaderPattern, leaderLengthMinimum,
                    leaderLengthOptimum, leaderLengthMaximum, ruleStyle,
                    ruleThickness, leaderPatternWidth, leaderAlignment );
            }
            else
            {
                la = ba.createNextLineArea();
                if ( la == null )
                    return -1;
                la.changeFont( fontState );
                la.changeColor( red, green, blue );

                if ( leaderLengthMinimum <= la.getContentWidth() )
                {
                    la.AddLeader( leaderPattern, leaderLengthMinimum,
                        leaderLengthOptimum, leaderLengthMaximum,
                        ruleStyle, ruleThickness, leaderPatternWidth,
                        leaderAlignment );
                }
                else
                {
                    FonetDriver.ActiveDriver.FireFonetWarning(
                        "Leader doesn't fit into line, it will be clipped to fit." );
                    la.AddLeader( leaderPattern, la.getRemainingWidth(),
                        leaderLengthOptimum, leaderLengthMaximum,
                        ruleStyle, ruleThickness, leaderPatternWidth,
                        leaderAlignment );
                }
            }
            return 1;
        }

        internal new class Maker : FObj.Maker
        {
            public override FObj Make( FObj parent, PropertyList propertyList )
            {
                return new Leader( parent, propertyList );
            }
        }
    }
}