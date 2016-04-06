namespace Fonet.Fo.Properties
{
    internal class InitialPageNumberMaker : StringProperty.Maker
    {
        private Property _mDefaultProp;

        protected InitialPageNumberMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new InitialPageNumberMaker( propName );
        }


        public override bool IsInherited()
        {
            return false;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "auto", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}