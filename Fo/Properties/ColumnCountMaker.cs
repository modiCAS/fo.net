namespace Fonet.Fo.Properties
{
    internal class ColumnCountMaker : StringProperty.Maker
    {
        private Property _mDefaultProp;

        protected ColumnCountMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new ColumnCountMaker( propName );
        }


        public override bool IsInherited()
        {
            return false;
        }

        public override Property Make( PropertyList propertyList )
        {
            return _mDefaultProp ?? ( _mDefaultProp = Make( propertyList, "1", propertyList.GetParentFObj() ) );
        }
    }
}