namespace Fonet.Fo.Properties
{
    internal class MaximumRepeatsMaker : StringProperty.Maker
    {
        private Property _mDefaultProp;

        protected MaximumRepeatsMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new MaximumRepeatsMaker( propName );
        }


        public override bool IsInherited()
        {
            return false;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "no-limit", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}