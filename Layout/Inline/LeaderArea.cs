using Fonet.Fo.Properties;
using Fonet.Render.Pdf;

namespace Fonet.Layout.Inline
{
    internal class LeaderArea : InlineArea
    {
        private int leaderLengthOptimum;
        private readonly int leaderPattern;
        private readonly int ruleStyle;
        private readonly int ruleThickness;

        public LeaderArea(
            FontState fontState, float red, float green,
            float blue, string text, int leaderLengthOptimum,
            int leaderPattern, int ruleThickness, int ruleStyle )
            : base( fontState, leaderLengthOptimum, red, green, blue )
        {
            this.leaderPattern = leaderPattern;
            this.leaderLengthOptimum = leaderLengthOptimum;
            this.ruleStyle = ruleStyle;
            if ( ruleStyle == RuleStyle.None )
                ruleThickness = 0;
            this.ruleThickness = ruleThickness;
        }

        public override void render( PdfRenderer renderer )
        {
            renderer.RenderLeaderArea( this );
        }

        public int getRuleThickness()
        {
            return ruleThickness;
        }

        public int getRuleStyle()
        {
            return ruleStyle;
        }

        public int getLeaderPattern()
        {
            return leaderPattern;
        }

        public int getLeaderLength()
        {
            return contentRectangleWidth;
        }
    }
}