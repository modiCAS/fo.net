namespace Fonet.Fo.Properties
{
    internal class KeepWithNextMaker : GenericKeep
    {
        private Property _mDefaultProp;

        protected KeepWithNextMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new KeepWithNextMaker( propName );
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