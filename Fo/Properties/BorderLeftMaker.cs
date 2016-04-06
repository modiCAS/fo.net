namespace Fonet.Fo.Properties
{
    internal class BorderLeftMaker : ListProperty.Maker
    {
        protected BorderLeftMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new BorderLeftMaker( propName );
        }


        public override bool IsInherited()
        {
            return false;
        }
    }
}