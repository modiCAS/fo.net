using Fonet.Fo;

namespace Fonet.DataTypes
{
    internal class LengthPair : ICompoundDatatype
    {
        private Property _bpd;
        private Property _ipd;

        public void SetComponent( string sCmpnName, Property cmpnValue,
            bool bIsDefault )
        {
            if ( sCmpnName.Equals( "block-progression-direction" ) )
                _bpd = cmpnValue;
            else if ( sCmpnName.Equals( "inline-progression-direction" ) )
                _ipd = cmpnValue;
        }

        public Property GetComponent( string sCmpnName )
        {
            if ( sCmpnName.Equals( "block-progression-direction" ) )
                return GetBpd();
            if ( sCmpnName.Equals( "inline-progression-direction" ) )
                return GetIpd();
            return null;
        }

        public Property GetIpd()
        {
            return _ipd;
        }

        public Property GetBpd()
        {
            return _bpd;
        }
    }
}