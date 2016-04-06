using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Fonet.Fo.Properties
{
    internal sealed class LeaderPatternMaker : PropertyMaker
    {
        private LeaderPatternMaker( string name )
            : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new LeaderPatternMaker( propName );
        }

        public override bool IsInherited()
        {
            return true;
        }

        public override Property CheckEnumValues( string value )
        {
            // TODO: handle 'inherit' somewhere higher
            // if ( value == "inherit" ) return null;
            // TODO: this should work for all enumerations
            value = CultureInfo.InvariantCulture.TextInfo.ToTitleCase( value ).Replace( "-", string.Empty );

            LeaderPattern result;
            if ( !Enum.TryParse( value, out result ) ) result = LeaderPattern.Space;

            return new EnumProperty<LeaderPattern>( result );
        }

        public override Property Make( PropertyList propertyList )
        {
            return new EnumProperty<LeaderPattern>( LeaderPattern.Space );
        }
    }
}