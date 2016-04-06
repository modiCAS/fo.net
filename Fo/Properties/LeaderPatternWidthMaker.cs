using System.Collections;
using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class LeaderPatternWidthMaker : LengthProperty.Maker
    {
        private static Hashtable _sHtKeywords;

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
            return Make( propertyList, "use-font-metrics", propertyList.GetParentFObj() );
        }

        private static void InitKeywords()
        {
            _sHtKeywords = new Hashtable( 1 );

            _sHtKeywords.Add( "use-font-metrics", "0pt" );
        }

        protected override string CheckValueKeywords( string keyword )
        {
            if ( _sHtKeywords == null )
                InitKeywords();
            var value = (string)_sHtKeywords[ keyword ];
            if ( value == null )
                return base.CheckValueKeywords( keyword );
            return value;
        }

        public override IPercentBase GetPercentBase( FObj fo, PropertyList propertyList )
        {
            return new LengthBase( fo, propertyList, LengthBase.ContainingBox );
        }
    }
}