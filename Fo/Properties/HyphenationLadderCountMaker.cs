using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class HyphenationLadderCountMaker : ToBeImplementedProperty.Maker
    {
        private Property _mDefaultProp;

        protected HyphenationLadderCountMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new HyphenationLadderCountMaker( propName );
        }


        public override bool IsInherited()
        {
            return true;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "no-limit", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}