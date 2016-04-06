namespace Fonet.DataTypes
{
    internal class ColorSpace
    {
        public const int DeviceUnknown = -1;
        public const int DeviceGray = 1;
        public const int DeviceRgb = 2;
        public const int DeviceCmyk = 3;

        protected int currentColorSpace = -1;

        private bool hasICCProfile;
        private byte[] iccProfile;
        private int numComponents;

        public ColorSpace( int theColorSpace )
        {
            currentColorSpace = theColorSpace;
            hasICCProfile = false;
            numComponents = CalculateNumComponents();
        }

        public void SetColorSpace( int theColorSpace )
        {
            currentColorSpace = theColorSpace;
            numComponents = CalculateNumComponents();
        }

        public bool HasICCProfile()
        {
            return hasICCProfile;
        }

        public byte[] GetICCProfile()
        {
            if ( hasICCProfile )
                return iccProfile;
            return new byte[ 0 ];
        }

        public void SetICCProfile( byte[] iccProfile )
        {
            this.iccProfile = iccProfile;
            hasICCProfile = true;
        }

        public int GetColorSpace()
        {
            return currentColorSpace;
        }

        public int GetNumComponents()
        {
            return numComponents;
        }

        public string GetColorSpacePDFString()
        {
            if ( currentColorSpace == DeviceRgb )
                return "DeviceRGB";
            if ( currentColorSpace == DeviceCmyk )
                return "DeviceCMYK";
            if ( currentColorSpace == DeviceGray )
                return "DeviceGray";
            return "DeviceRGB";
        }

        private int CalculateNumComponents()
        {
            if ( currentColorSpace == DeviceGray )
                return 1;
            if ( currentColorSpace == DeviceRgb )
                return 3;
            if ( currentColorSpace == DeviceCmyk )
                return 4;
            return 0;
        }
    }
}