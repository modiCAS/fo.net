namespace Fonet.Fo.Properties
{
    internal class TableOmitHeaderAtBreakMaker : GenericBoolean
    {
        private Property _mDefaultProp;

        protected TableOmitHeaderAtBreakMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new TableOmitHeaderAtBreakMaker( propName );
        }

        public override bool IsInherited()
        {
            return false;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "false", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}