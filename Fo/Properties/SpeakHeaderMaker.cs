using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class SpeakHeaderMaker : ToBeImplementedProperty.Maker
    {
        private Property _mDefaultProp;

        protected SpeakHeaderMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new SpeakHeaderMaker( propName );
        }


        public override bool IsInherited()
        {
            return true;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "once", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}