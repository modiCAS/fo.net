namespace Fonet.Fo.Properties
{
    internal class BorderRightMaker : ListProperty.Maker
    {
        protected BorderRightMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new BorderRightMaker( propName );
        }


        public override bool IsInherited()
        {
            return false;
        }
    }
}