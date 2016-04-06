namespace Fonet.Fo.Properties
{
    internal class GroupingSizeMaker : NumberProperty.Maker
    {
        private Property _mDefaultProp;

        protected GroupingSizeMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new GroupingSizeMaker( propName );
        }


        public override bool IsInherited()
        {
            return false;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "0", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}