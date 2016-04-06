using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class HyphenationKeepMaker : ToBeImplementedProperty.Maker
    {
        private Property _mDefaultProp;

        protected HyphenationKeepMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new HyphenationKeepMaker( propName );
        }


        public override bool IsInherited()
        {
            return true;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "auto", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}