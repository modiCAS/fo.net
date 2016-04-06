namespace Fonet.Fo.Properties
{
    internal class GenericCondPadding : GenericCondLength
    {
        protected GenericCondPadding( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new GenericCondPadding( propName );
        }


        public override bool IsInherited()
        {
            return false;
        }

        protected override string GetDefaultForLength()
        {
            return "0pt";
        }
    }
}