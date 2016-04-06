using Fonet.DataTypes;
using Fonet.Layout;

namespace Fonet.Fo.Flow
{
    internal class Leader : FObjMixed
    {
        public Leader( FObj parent, PropertyList propertyList )
            : base( parent, propertyList )
        {
            name = "fo:leader";
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
                return new Status( Status.OK );
            }
            blockArea = (BlockArea)area;

            AccessibilityProps mAccProps = propMgr.GetAccessibilityProps();
            AuralProps mAurProps = propMgr.GetAuralProps();
            BorderAndPadding bap = propMgr.GetBorderAndPadding();
            BackgroundProps bProps = propMgr.GetBackgroundProps();
            MarginInlineProps mProps = propMgr.GetMarginInlineProps();
            RelativePositionProps mRelProps = propMgr.GetRelativePositionProps();
            ColorType c = properties.GetProperty( "color" ).GetColorType();
            float red = c.Red;
            float green = c.Green;
            float blue = c.Blue;

            int leaderPattern = properties.GetProperty( "leader-pattern" ).GetEnum();
            int leaderLengthOptimum =
                properties.GetProperty( "leader-length.optimum" ).GetLength().MValue();
            int leaderLengthMinimum =
                properties.GetProperty( "leader-length.minimum" ).GetLength().MValue();
            Length maxlength = properties.GetProperty( "leader-length.maximum" ).GetLength();
            int leaderLengthMaximum;
            if ( maxlength is PercentLength )
            {
                leaderLengthMaximum = (int)( ( (PercentLength)maxlength ).value()
                    * area.getAllocationWidth() );
            }
            else
                leaderLengthMaximum = maxlength.MValue();
            int ruleThickness =
                properties.GetProperty( "rule-thickness" ).GetLength().MValue();
            int ruleStyle = properties.GetProperty( "rule-style" ).GetEnum();
            int leaderPatternWidth =
                properties.GetProperty( "leader-pattern-width" ).GetLength().MValue();
            int leaderAlignment =
                properties.GetProperty( "leader-alignment" ).GetEnum();

            string id = properties.GetProperty( "id" ).GetString();
            blockArea.getIDReferences().InitializeID( id, blockArea );

            int succeeded = AddLeader( blockArea,
                propMgr.GetFontState( area.getFontInfo() ),
                red, green, blue, leaderPattern,
                leaderLengthMinimum, leaderLengthOptimum,
                leaderLengthMaximum, ruleThickness,
                ruleStyle, leaderPatternWidth,
                leaderAlignment );
            if ( succeeded == 1 )
                return new Status( Status.OK );
            return new Status( Status.AREA_FULL_SOME );
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