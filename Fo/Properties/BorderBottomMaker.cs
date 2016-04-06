namespace Fonet.Fo.Properties
{
    internal class BorderBottomMaker : ListProperty.Maker
    {
        protected BorderBottomMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new BorderBottomMaker( propName );
        }


        public override bool IsInherited()
        {
            return false;
        }
    }
}