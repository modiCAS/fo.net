namespace Fonet.Fo.Properties
{
    internal class BorderWidthMaker : ListProperty.Maker
    {
        protected BorderWidthMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new BorderWidthMaker( propName );
        }


        public override bool IsInherited()
        {
            return false;
        }
    }
}