namespace Fonet.Fo.Properties
{
    internal class BorderMaker : ListProperty.Maker
    {
        protected BorderMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new BorderMaker( propName );
        }


        public override bool IsInherited()
        {
            return false;
        }
    }
}