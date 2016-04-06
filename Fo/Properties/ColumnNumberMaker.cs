namespace Fonet.Fo.Properties
{
    internal class ColumnNumberMaker : NumberProperty.Maker
    {
        private Property _mDefaultProp;

        protected ColumnNumberMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new ColumnNumberMaker( propName );
        }


        public override bool IsInherited()
        {
            return false;
        }

        public override Property Make( PropertyList propertyList )
        {
            return _mDefaultProp ?? ( _mDefaultProp = Make( propertyList, "0", propertyList.GetParentFObj() ) );
        }
    }
}