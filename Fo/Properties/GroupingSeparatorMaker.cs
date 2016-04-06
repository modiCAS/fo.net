namespace Fonet.Fo.Properties
{
    internal class GroupingSeparatorMaker : CharacterProperty.Maker
    {
        private Property _mDefaultProp;

        protected GroupingSeparatorMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new GroupingSeparatorMaker( propName );
        }


        public override bool IsInherited()
        {
            return false;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "none", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}