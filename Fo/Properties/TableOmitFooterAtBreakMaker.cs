namespace Fonet.Fo.Properties
{
    internal class TableOmitFooterAtBreakMaker : GenericBoolean
    {
        private Property _mDefaultProp;

        protected TableOmitFooterAtBreakMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new TableOmitFooterAtBreakMaker( propName );
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