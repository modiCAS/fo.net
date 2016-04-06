namespace Fonet.Fo.Properties
{
    internal class LeftMaker : LengthProperty.Maker
    {
        private Property _mDefaultProp;

        protected LeftMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new LeftMaker( propName );
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