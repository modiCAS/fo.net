namespace Fonet.Fo.Properties
{
    internal class ColumnGapMaker : LengthProperty.Maker
    {
        private Property _mDefaultProp;

        protected ColumnGapMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new ColumnGapMaker( propName );
        }


        public override bool IsInherited()
        {
            return false;
        }

        protected override bool IsAutoLengthAllowed()
        {
            return true;
        }

        public override Property Make( PropertyList propertyList )
        {
            return _mDefaultProp ?? ( _mDefaultProp = Make( propertyList, "0.25in", propertyList.GetParentFObj() ) );
        }
    }
}