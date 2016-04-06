namespace Fonet.Fo.Properties
{
    internal class SourceDocumentMaker : StringProperty.Maker
    {
        private Property _mDefaultProp;

        protected SourceDocumentMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new SourceDocumentMaker( propName );
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