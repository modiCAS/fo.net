namespace Fonet.Fo.Properties
{
    internal class RuleThicknessMaker : LengthProperty.Maker
    {
        private Property _mDefaultProp;

        protected RuleThicknessMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new RuleThicknessMaker( propName );
        }


        public override bool IsInherited()
        {
            return true;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "1.0pt", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}