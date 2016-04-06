namespace Fonet.Fo.Properties
{
    internal class PaddingMaker : ListProperty.Maker
    {
        protected PaddingMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new PaddingMaker( propName );
        }


        public override bool IsInherited()
        {
            return false;
        }
    }
}