using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class RenderingIntentMaker : ToBeImplementedProperty.Maker
    {
        private Property _mDefaultProp;

        protected RenderingIntentMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new RenderingIntentMaker( propName );
        }


        public override bool IsInherited()
        {
            return false;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "auto", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}