using Fonet.Fo;

namespace Fonet.DataTypes
{
    internal interface ICompoundDatatype
    {
        void SetComponent( string componentName, Property componentValue, bool isDefault );

        Property GetComponent( string componentName );
    }
}