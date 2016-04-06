using Fonet.Layout;
using Fonet.Layout.Inline;

namespace Fonet.Fo
{
    internal class XmlElement : XmlObj
    {
        private readonly string _nmspace = string.Empty;

        public XmlElement( FObj parent, PropertyList propertyList, string tag )
            : base( parent, propertyList, tag )
        {
            Init();
        }

        public static FObj.Maker GetMaker( string tag )
        {
            return new Maker( tag );
        }

        public override Status Layout( Area area )
        {
            if ( !( area is ForeignObjectArea ) )
                throw new FonetException( "XML not in fo:instream-foreign-object" );

            return new Status( Status.Ok );
        }

        private void Init()
        {
            CreateBasicDocument();
        }

        public override string GetNameSpace()
        {
            return _nmspace;
        }

        internal new class Maker : FObj.Maker
        {
            private readonly string _tag;

            internal Maker( string t )
            {
                _tag = t;
            }

            public override FObj Make( FObj parent, PropertyList propertyList )
            {
                return new XmlElement( parent, propertyList, _tag );
            }
        }
    }
}