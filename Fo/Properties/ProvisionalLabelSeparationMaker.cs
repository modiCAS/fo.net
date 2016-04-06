namespace Fonet.Fo.Properties
{
    internal class ProvisionalLabelSeparationMaker : LengthProperty.Maker
    {
        private Property _mDefaultProp;

        protected ProvisionalLabelSeparationMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new ProvisionalLabelSeparationMaker( propName );
        }


        public override bool IsInherited()
        {
            return true;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "6pt", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}