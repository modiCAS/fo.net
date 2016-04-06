using Fonet.Fo;

namespace Fonet.DataTypes
{
    internal class LengthPair : ICompoundDatatype
    {
        private Property bpd;
        private Property ipd;

        public void SetComponent( string sCmpnName, Property cmpnValue,
            bool bIsDefault )
        {
            if ( sCmpnName.Equals( "block-progression-direction" ) )
                bpd = cmpnValue;
            else if ( sCmpnName.Equals( "inline-progression-direction" ) )
                ipd = cmpnValue;
        }

        public Property GetComponent( string sCmpnName )
        {
            if ( sCmpnName.Equals( "block-progression-direction" ) )
                return GetBPD();
            if ( sCmpnName.Equals( "inline-progression-direction" ) )
                return GetIPD();
            return null;
        }

        public Property GetIPD()
        {
            return ipd;
        }

        public Property GetBPD()
        {
            return bpd;
        }
    }
}