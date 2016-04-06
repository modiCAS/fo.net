using Fonet.Layout;

namespace Fonet.Fo
{
    internal class UnknownXmlObj : XmlObj
    {
        private readonly string _nmspace;

        protected UnknownXmlObj( FObj parent, PropertyList propertyList, string space, string tag )
            : base( parent, propertyList, tag )
        {
            _nmspace = space;
            if ( !"".Equals( space ) )
                Name = _nmspace + ":" + tag;
            else
                Name = "(none):" + tag;
        }

        public static FObj.Maker GetMaker( string space, string tag )
        {
            return new Maker( space, tag );
        }

        public override string GetNameSpace()
        {
            return _nmspace;
        }

        protected internal override void AddChild( FoNode child )
        {
            if ( Doc == null )
                CreateBasicDocument();
            base.AddChild( child );
        }

        protected internal override void AddCharacters( char[] data, int start, int length )
        {
            if ( Doc == null )
                CreateBasicDocument();
            base.AddCharacters( data, start, length );
        }

        public override Status Layout( Area area )
        {
            return new Status( Status.Ok );
        }

        internal new class Maker : FObj.Maker
        {
            private readonly string _space;
            private readonly string _tag;

            internal Maker( string sp, string t )
            {
                _space = sp;
                _tag = t;
            }

            public override FObj Make( FObj parent,
                PropertyList propertyList )
            {
                return new UnknownXmlObj( parent, propertyList, _space, _tag );
            }
        }
    }
}