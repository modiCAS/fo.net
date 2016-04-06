using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class ScoreSpacesMaker : ToBeImplementedProperty.Maker
    {
        private Property _mDefaultProp;

        protected ScoreSpacesMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new ScoreSpacesMaker( propName );
        }


        public override bool IsInherited()
        {
            return true;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "true", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}