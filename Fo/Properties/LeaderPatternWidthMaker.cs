using System.Collections;
using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class LeaderPatternWidthMaker : LengthProperty.Maker
    {
        private static Hashtable s_htKeywords;

        protected LeaderPatternWidthMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new LeaderPatternWidthMaker( propName );
        }


        public override bool IsInherited()
        {
            return true;
        }

        public override Property Make( PropertyList propertyList )
        {
            return Make( propertyList, "use-font-metrics", propertyList.getParentFObj() );
        }

        private static void initKeywords()
        {
            s_htKeywords = new Hashtable( 1 );

            s_htKeywords.Add( "use-font-metrics", "0pt" );
        }

        protected override string CheckValueKeywords( string keyword )
        {
            if ( s_htKeywords == null )
                initKeywords();
            var value = (string)s_htKeywords[ keyword ];
            if ( value == null )
                return base.CheckValueKeywords( keyword );
            return value;
        }

        public override IPercentBase GetPercentBase( FObj fo, PropertyList propertyList )
        {
            return new LengthBase( fo, propertyList, LengthBase.CONTAINING_BOX );
        }
    }
}