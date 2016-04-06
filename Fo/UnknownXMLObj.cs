using Fonet.Layout;

namespace Fonet.Fo
{
    internal class UnknownXMLObj : XMLObj
    {
        private readonly string nmspace;

        protected UnknownXMLObj( FObj parent, PropertyList propertyList, string space, string tag )
            : base( parent, propertyList, tag )
        {
            nmspace = space;
            if ( !"".Equals( space ) )
                name = nmspace + ":" + tag;
            else
                name = "(none):" + tag;
        }

        public static FObj.Maker GetMaker( string space, string tag )
        {
            return new Maker( space, tag );
        }

        public override string GetNameSpace()
        {
            return nmspace;
        }

        protected internal override void AddChild( FONode child )
        {
            if ( doc == null )
                CreateBasicDocument();
            base.AddChild( child );
        }

        protected internal override void AddCharacters( char[] data, int start, int length )
        {
            if ( doc == null )
                CreateBasicDocument();
            base.AddCharacters( data, start, length );
        }

        public override Status Layout( Area area )
        {
            return new Status( Status.OK );
        }

        internal new class Maker : FObj.Maker
        {
            private readonly string space;
            private readonly string tag;

            internal Maker( string sp, string t )
            {
                space = sp;
                tag = t;
            }

            public override FObj Make( FObj parent,
                PropertyList propertyList )
            {
                return new UnknownXMLObj( parent, propertyList, space, tag );
            }
        }
    }
}