namespace Fonet.Fo.Properties
{
    internal class ColorMaker : GenericColor
    {
        private Property _mDefaultProp;

        protected ColorMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new ColorMaker( propName );
        }


        public override bool IsInherited()
        {
            return true;
        }

        public override Property Make( PropertyList propertyList )
        {
            return _mDefaultProp ?? ( _mDefaultProp = Make( propertyList, "black", propertyList.GetParentFObj() ) );
        }
    }
}