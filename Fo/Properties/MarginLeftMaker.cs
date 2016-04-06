namespace Fonet.Fo.Properties
{
    internal class MarginLeftMaker : LengthProperty.Maker
    {
        private Property _mDefaultProp;

        protected MarginLeftMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new MarginLeftMaker( propName );
        }


        public override bool IsInherited()
        {
            return false;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "0pt", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}