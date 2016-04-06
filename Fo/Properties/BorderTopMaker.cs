namespace Fonet.Fo.Properties
{
    internal class BorderTopMaker : ListProperty.Maker
    {
        protected BorderTopMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new BorderTopMaker( propName );
        }


        public override bool IsInherited()
        {
            return false;
        }
    }
}