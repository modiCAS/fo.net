using Fonet.DataTypes;
using Fonet.Fo.Properties;
using Fonet.Layout;

namespace Fonet.Fo.Flow
{
    internal sealed class Leader : FObjMixed
    {
        private Leader( FObj parent, PropertyList propertyList )
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
            if ( !( area is BlockArea ) )
            {
                FonetDriver.ActiveDriver.FireFonetWarning(
                    "fo:leader must be a direct child of fo:block " );
                return new Status( Status.Ok );
            }

            var blockArea = (BlockArea)area;

            AccessibilityProps accessibilityProps = PropMgr.GetAccessibilityProps();
            AuralProps auralProps = PropMgr.GetAuralProps();
            BorderAndPadding borderAndPadding = PropMgr.GetBorderAndPadding();
            BackgroundProps backgroundProps = PropMgr.GetBackgroundProps();
            MarginInlineProps marginInlineProps = PropMgr.GetMarginInlineProps();
            RelativePositionProps relativePositionProps = PropMgr.GetRelativePositionProps();
            ColorType colorType = Properties.GetProperty( "color" ).GetColorType();
            float red = colorType.Red;
            float green = colorType.Green;
            float blue = colorType.Blue;

            var leaderPattern = Properties.GetProperty<LeaderPattern>( "leader-pattern" );
            int leaderLengthOptimum =
                Properties.GetProperty( "leader-length.optimum" ).GetLength().MValue();
            int leaderLengthMinimum =
                Properties.GetProperty( "leader-length.minimum" ).GetLength().MValue();
            Length maxlength = Properties.GetProperty( "leader-length.maximum" ).GetLength();
            int leaderLengthMaximum;

            var length = maxlength as PercentLength;
            if ( length != null )
            {
                leaderLengthMaximum = (int)( length.Value() * area.GetAllocationWidth() );
            }
            else
            {
                leaderLengthMaximum = maxlength.MValue();
            }

            int ruleThickness = Properties.GetProperty( "rule-thickness" ).GetLength().MValue();
            int ruleStyle = Properties.GetProperty( "rule-style" ).GetEnum();
            int leaderPatternWidth = Properties.GetProperty( "leader-pattern-width" ).GetLength().MValue();
            int leaderAlignment = Properties.GetProperty( "leader-alignment" ).GetEnum();

            string id = Properties.GetProperty( "id" ).GetString();
            blockArea.GetIDReferences().InitializeID( id, blockArea );

            int succeeded = AddLeader( blockArea,
                PropMgr.GetFontState( area.GetFontInfo() ),
                red, green, blue, leaderPattern,
                leaderLengthMinimum, leaderLengthOptimum,
                leaderLengthMaximum, ruleThickness,
                ruleStyle, leaderPatternWidth,
                leaderAlignment );
            return succeeded == 1 ? new Status( Status.Ok ) : new Status( Status.AreaFullSome );
        }

        private static int AddLeader( BlockArea ba, FontState fontState, float red,
            float green, float blue, LeaderPattern leaderPattern,
            int leaderLengthMinimum, int leaderLengthOptimum,
            int leaderLengthMaximum, int ruleThickness,
            int ruleStyle, int leaderPatternWidth,
            int leaderAlignment )
        {
            LineArea la = ba.GetCurrentLineArea();
            if ( la == null ) return -1;

            la.ChangeFont( fontState );
            la.ChangeColor( red, green, blue );

            if ( leaderLengthOptimum <= la.GetRemainingWidth() )
            {
                la.AddLeader( leaderPattern, leaderLengthMinimum,
                    leaderLengthOptimum, leaderLengthMaximum, ruleStyle,
                    ruleThickness, leaderPatternWidth, leaderAlignment );
            }
            else
            {
                la = ba.CreateNextLineArea();
                if ( la == null ) return -1;

                la.ChangeFont( fontState );
                la.ChangeColor( red, green, blue );

                if ( leaderLengthMinimum <= la.GetContentWidth() )
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
                    la.AddLeader( leaderPattern, la.GetRemainingWidth(),
                        leaderLengthOptimum, leaderLengthMaximum,
                        ruleStyle, ruleThickness, leaderPatternWidth,
                        leaderAlignment );
                }
            }
            return 1;
        }

        private new sealed class Maker : FObj.Maker
        {
            public override FObj Make( FObj parent, PropertyList propertyList )
            {
                return new Leader( parent, propertyList );
            }
        }
    }
}