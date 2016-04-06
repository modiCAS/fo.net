namespace Fonet.DataTypes
{
    internal class ColorSpace
    {
        public const int DeviceUnknown = -1;
        public const int DeviceGray = 1;
        public const int DeviceRgb = 2;
        public const int DeviceCmyk = 3;

        protected int CurrentColorSpace = -1;

        private bool _hasIccProfile;
        private byte[] _iccProfile;
        private int _numComponents;

        public ColorSpace( int theColorSpace )
        {
            CurrentColorSpace = theColorSpace;
            _hasIccProfile = false;
            _numComponents = CalculateNumComponents();
        }

        public void SetColorSpace( int theColorSpace )
        {
            CurrentColorSpace = theColorSpace;
            _numComponents = CalculateNumComponents();
        }

        public bool HasIccProfile()
        {
            return _hasIccProfile;
        }

        public byte[] GetIccProfile()
        {
            if ( _hasIccProfile )
                return _iccProfile;
            return new byte[ 0 ];
        }

        public void SetIccProfile( byte[] iccProfile )
        {
            this._iccProfile = iccProfile;
            _hasIccProfile = true;
        }

        public int GetColorSpace()
        {
            return CurrentColorSpace;
        }

        public int GetNumComponents()
        {
            return _numComponents;
        }

        public string GetColorSpacePdfString()
        {
            if ( CurrentColorSpace == DeviceRgb )
                return "DeviceRGB";
            if ( CurrentColorSpace == DeviceCmyk )
                return "DeviceCMYK";
            if ( CurrentColorSpace == DeviceGray )
                return "DeviceGray";
            return "DeviceRGB";
        }

        private int CalculateNumComponents()
        {
            if ( CurrentColorSpace == DeviceGray )
                return 1;
            if ( CurrentColorSpace == DeviceRgb )
                return 3;
            if ( CurrentColorSpace == DeviceCmyk )
                return 4;
            return 0;
        }
    }
}