namespace Fonet.Fo.Flow
{
    internal class Wrapper : FObjMixed
    {
        public Wrapper( FObj parent, PropertyList propertyList )
            : base( parent, propertyList )
        {
            Name = "fo:wrapper";
        }

        public new static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        protected internal override void AddCharacters( char[] data, int start, int length )
        {
            var ft = new FoText( data, start, length, this );
            Children.Add( ft );
        }

        internal new class Maker : FObj.Maker
        {
            public override FObj Make( FObj parent, PropertyList propertyList )
            {
                return new Wrapper( parent, propertyList );
            }
        }
    }
}