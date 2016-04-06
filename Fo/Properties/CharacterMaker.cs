namespace Fonet.Fo.Properties
{
    internal class CharacterMaker : CharacterProperty.Maker
    {
        private Property _mDefaultProp;

        protected CharacterMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new CharacterMaker( propName );
        }


        public override bool IsInherited()
        {
            return false;
        }

        public override Property Make( PropertyList propertyList )
        {
            return _mDefaultProp ?? ( _mDefaultProp = Make( propertyList, "none", propertyList.GetParentFObj() ) );
        }
    }
}