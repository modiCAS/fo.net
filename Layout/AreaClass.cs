namespace Fonet.Layout
{
    internal static class AreaClass
    {
        public const string Unassigned = "unassigned";

        public const string XslNormal = "xsl-normal";

        public const string XslAbsolute = "xsl-absolute";

        public const string XslFootnote = "xsl-footnote";

        public const string XslSideFloat = "xsl-side-float";

        public const string XslBeforeFloat = "xsl-before-float";

        public static string SetAreaClass( string areaClass )
        {
            if ( areaClass.Equals( XslNormal ) ||
                areaClass.Equals( XslAbsolute ) ||
                areaClass.Equals( XslFootnote ) ||
                areaClass.Equals( XslSideFloat ) ||
                areaClass.Equals( XslBeforeFloat ) )
                return areaClass;
            throw new FonetException( "Unknown area class '" + areaClass + "'" );
        }
    }
}