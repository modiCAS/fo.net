using Fonet.Fo;
using Fonet.Fo.Properties;

namespace Fonet.DataTypes
{
    internal class CondLength : ICompoundDatatype
    {
        private Property conditionality;
        private Property length;

        public void SetComponent( string sCmpnName, Property cmpnValue, bool bIsDefault )
        {
            if ( sCmpnName.Equals( "length" ) )
                length = cmpnValue;
            else if ( sCmpnName.Equals( "conditionality" ) )
                conditionality = cmpnValue;
        }

        public Property GetComponent( string sCmpnName )
        {
            if ( sCmpnName.Equals( "length" ) )
                return length;
            if ( sCmpnName.Equals( "conditionality" ) )
                return conditionality;
            return null;
        }

        public Property GetConditionality()
        {
            return conditionality;
        }

        public Property GetLength()
        {
            return length;
        }

        public bool IsDiscard()
        {
            return conditionality.GetEnum() == Constants.DISCARD;
        }

        public int MValue()
        {
            return length.GetLength().MValue();
        }
    }
}