namespace Fonet.Fo.Properties
{
    internal class BorderStyleMaker : ListProperty.Maker
    {
        protected BorderStyleMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new BorderStyleMaker( propName );
        }


        public override bool IsInherited()
        {
            return false;
        }
    }
}