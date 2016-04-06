namespace Fonet.Fo.Properties
{
    internal class BottomMaker : LengthProperty.Maker
    {
        private Property _mDefaultProp;

        protected BottomMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new BottomMaker( propName );
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
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "auto", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}