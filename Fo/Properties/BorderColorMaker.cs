namespace Fonet.Fo.Properties
{
    internal class BorderColorMaker : ListProperty.Maker
    {
        protected BorderColorMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new BorderColorMaker( propName );
        }


        public override bool IsInherited()
        {
            return false;
        }
    }
}