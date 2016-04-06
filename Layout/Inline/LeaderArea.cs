using Fonet.Fo.Properties;
using Fonet.Render.Pdf;

namespace Fonet.Layout.Inline
{
    internal class LeaderArea : InlineArea
    {
        private int _leaderLengthOptimum;
        private readonly int _leaderPattern;
        private readonly int _ruleStyle;
        private readonly int _ruleThickness;

        public LeaderArea(
            FontState fontState, float red, float green,
            float blue, string text, int leaderLengthOptimum,
            int leaderPattern, int ruleThickness, int ruleStyle )
            : base( fontState, leaderLengthOptimum, red, green, blue )
        {
            this._leaderPattern = leaderPattern;
            this._leaderLengthOptimum = leaderLengthOptimum;
            this._ruleStyle = ruleStyle;
            if ( ruleStyle == RuleStyle.None )
                ruleThickness = 0;
            this._ruleThickness = ruleThickness;
        }

        public override void Render( PdfRenderer renderer )
        {
            renderer.RenderLeaderArea( this );
        }

        public int GetRuleThickness()
        {
            return _ruleThickness;
        }

        public int GetRuleStyle()
        {
            return _ruleStyle;
        }

        public int GetLeaderPattern()
        {
            return _leaderPattern;
        }

        public int GetLeaderLength()
        {
            return ContentRectangleWidth;
        }
    }
}