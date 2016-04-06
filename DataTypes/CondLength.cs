using Fonet.Fo;
using Fonet.Fo.Properties;

namespace Fonet.DataTypes
{
    internal class CondLength : ICompoundDatatype
    {
        private Property _conditionality;
        private Property _length;

        public void SetComponent( string sCmpnName, Property cmpnValue, bool bIsDefault )
        {
            if ( sCmpnName.Equals( "length" ) )
                _length = cmpnValue;
            else if ( sCmpnName.Equals( "conditionality" ) )
                _conditionality = cmpnValue;
        }

        public Property GetComponent( string sCmpnName )
        {
            if ( sCmpnName.Equals( "length" ) )
                return _length;
            if ( sCmpnName.Equals( "conditionality" ) )
                return _conditionality;
            return null;
        }

        public Property GetConditionality()
        {
            return _conditionality;
        }

        public Property GetLength()
        {
            return _length;
        }

        public bool IsDiscard()
        {
            return _conditionality.GetEnum() == Constants.Discard;
        }

        public int MValue()
        {
            return _length.GetLength().MValue();
        }
    }
}