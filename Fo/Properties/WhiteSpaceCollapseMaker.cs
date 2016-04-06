namespace Fonet.Fo.Properties
{
    internal class WhiteSpaceCollapseMaker : GenericBoolean
    {
        private Property _mDefaultProp;

        protected WhiteSpaceCollapseMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new WhiteSpaceCollapseMaker( propName );
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