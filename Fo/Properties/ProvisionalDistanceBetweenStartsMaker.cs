namespace Fonet.Fo.Properties
{
    internal class ProvisionalDistanceBetweenStartsMaker : LengthProperty.Maker
    {
        private Property _mDefaultProp;

        protected ProvisionalDistanceBetweenStartsMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new ProvisionalDistanceBetweenStartsMaker( propName );
        }


        public override bool IsInherited()
        {
            return true;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "24pt", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}