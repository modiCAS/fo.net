using Fonet.Fo;

namespace Fonet.DataTypes
{
    internal class LengthBase : IPercentBase
    {
        public const int CustomBase = 0;
        public const int Fontsize = 1;
        public const int InhFontsize = 2;
        public const int ContainingBox = 3;
        public const int ContainingRefarea = 4;
        private readonly int _iBaseType;
        protected readonly FObj ParentFo;
        private readonly PropertyList _propertyList;

        public LengthBase( FObj parentFo, PropertyList plist, int iBaseType )
        {
            this.ParentFo = parentFo;
            _propertyList = plist;
            this._iBaseType = iBaseType;
        }

        public int GetDimension()
        {
            return 1;
        }

        public double GetBaseValue()
        {
            return 1.0;
        }

        public int GetBaseLength()
        {
            switch ( _iBaseType )
            {
            case Fontsize:
                return _propertyList.GetProperty( "font-size" ).GetLength().MValue();
            case InhFontsize:
                return _propertyList.GetInheritedProperty( "font-size" ).GetLength().MValue();
            case ContainingBox:
                return ParentFo.GetContentWidth();
            case ContainingRefarea:
                {
                FObj fo;
                for ( fo = ParentFo;
                    fo != null && !fo.GeneratesReferenceAreas();
                    fo = fo.GetParent() )
                {
                }

                return fo != null ? fo.GetContentWidth() : 0;
                }
            case CustomBase:
                FonetDriver.ActiveDriver.FireFonetError(
                    "LengthBase.getBaseLength() called on CUSTOM_BASE type" );
                return 0;
            default:
                FonetDriver.ActiveDriver.FireFonetError(
                    "Unknown base type for LengthBase" );
                return 0;
            }
        }

        protected FObj GetParentFo()
        {
            return ParentFo;
        }

        protected PropertyList GetPropertyList()
        {
            return _propertyList;
        }
    }
}