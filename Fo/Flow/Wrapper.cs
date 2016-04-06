namespace Fonet.Fo.Flow
{
    internal class Wrapper : FObjMixed
    {
        public Wrapper( FObj parent, PropertyList propertyList )
            : base( parent, propertyList )
        {
            name = "fo:wrapper";
        }

        public new static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        protected internal override void AddCharacters( char[] data, int start, int length )
        {
            var ft = new FOText( data, start, length, this );
            children.Add( ft );
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