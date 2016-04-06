using Fonet.Fo.Properties;
using Fonet.Render.Pdf;

namespace Fonet.Layout.Inline
{
    internal sealed class LeaderArea : InlineArea
    {
        private int _leaderLengthOptimum;
        private readonly LeaderPattern _pattern;
        private readonly int _ruleStyle;
        private readonly int _ruleThickness;

        public LeaderArea(
            FontState fontState, float red, float green,
            float blue, string text, int leaderLengthOptimum,
            LeaderPattern pattern, int ruleThickness, int ruleStyle )
            : base( fontState, leaderLengthOptimum, red, green, blue )
        {
            _pattern = pattern;
            _leaderLengthOptimum = leaderLengthOptimum;
            _ruleStyle = ruleStyle;
            if ( ruleStyle == RuleStyle.None ) ruleThickness = 0;
            _ruleThickness = ruleThickness;
        }

        public LeaderPattern Pattern
        {
            get { return _pattern; }
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

        public int GetLeaderLength()
        {
            return ContentRectangleWidth;
        }
    }
}