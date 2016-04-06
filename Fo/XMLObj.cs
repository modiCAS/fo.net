using System;
using System.Collections;
using System.Xml;
using Fonet.DataTypes;
using Fonet.Layout;

namespace Fonet.Fo
{
    internal abstract class XmlObj : FObj
    {
        private const string Ns = "http://www.codeplex.com/fonet";

        protected static Hashtable NsTable = new Hashtable();

        protected XmlDocument Doc;

        private XmlNode _element;
        private string _tagName;

        protected XmlObj( FObj parent, PropertyList propertyList, string tag )
            : base( parent, propertyList )
        {
            _tagName = tag;
        }

        public abstract string GetNameSpace();

        public void AddGraphic( XmlDocument doc, XmlNode parent )
        {
            this.Doc = doc;
        }

        public void BuildTopLevel( XmlDocument doc, XmlNode svgRoot )
        {
        }

        public XmlDocument CreateBasicDocument()
        {
            try
            {
                Doc = new XmlDocument();
                Doc.AppendChild( Doc.CreateElement( "graph", Ns ) );
                _element = Doc.DocumentElement;
                BuildTopLevel( Doc, _element );
            }
            catch ( Exception e )
            {
                FonetDriver.ActiveDriver.FireFonetError( e.ToString() );
            }
            return Doc;
        }

        protected internal override void AddChild( FoNode child )
        {
            var xmlChild = child as XmlObj;
            if ( xmlChild != null ) xmlChild.AddGraphic( Doc, _element );
        }

        protected internal override void AddCharacters( char[] data, int start, int length )
        {
            var str = new string( data, start, length - start );
            Doc.DocumentElement.AppendChild( Doc.CreateTextNode( str ) );
        }

        public override Status Layout( Area area )
        {
            FonetDriver.ActiveDriver.FireFonetError(
                Name + " outside foreign xml" );

            return new Status( Status.Ok );
        }

        public override void RemoveID( IDReferences idReferences )
        {
        }

        public override void SetIsInTableCell()
        {
        }

        public override void ForceStartOffset( int offset )
        {
        }

        public override void ForceWidth( int width )
        {
        }

        public override void ResetMarker()
        {
        }

        public override void SetLinkSet( LinkSet linkSet )
        {
        }

        public override ArrayList GetMarkerSnapshot( ArrayList snapshot )
        {
            return snapshot;
        }

        public override void Rollback( ArrayList snapshot )
        {
        }

        protected override void SetWritingMode()
        {
        }
    }
}