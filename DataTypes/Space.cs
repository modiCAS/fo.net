using Fonet.Fo;

namespace Fonet.DataTypes
{
    internal class Space : LengthRange
    {
        public Property Conditionality { get; set; }

        public Property Precedence { get; set; }

        public override void SetComponent( string componentName, Property componentValue, bool isDefault )
        {
            if ( componentName.Equals( "precedence" ) )
                Precedence = componentValue;
            else if ( componentName.Equals( "conditionality" ) )
                Conditionality = componentValue;
            else
                base.SetComponent( componentName, componentValue, isDefault );
        }

        public override Property GetComponent( string componentName )
        {
            if ( componentName.Equals( "precedence" ) )
                return Precedence;
            if ( componentName.Equals( "conditionality" ) )
                return Conditionality;
            return base.GetComponent( componentName );
        }
    }
}