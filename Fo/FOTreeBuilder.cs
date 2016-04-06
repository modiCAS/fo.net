using System;
using System.Collections;
using System.Xml;
using Fonet.Fo.Pagination;

namespace Fonet.Fo
{
    /// <summary>
    ///     Builds the formatting object tree.
    /// </summary>
    internal sealed class FOTreeBuilder
    {
        /// <summary>
        ///     Current formatting object being handled.
        /// </summary>
        private FObj currentFObj;

        /// <summary>
        ///     Table mapping element names to the makers of objects
        ///     representing formatting objects.
        /// </summary>
        private readonly Hashtable fobjTable = new Hashtable();

        private readonly ArrayList namespaces = new ArrayList();

        /// <summary>
        ///     Class that builds a property list for each formatting object.
        /// </summary>
        private readonly Hashtable propertylistTable = new Hashtable();

        /// <summary>
        ///     The root of the formatting object tree.
        /// </summary>
        private FObj rootFObj;

        /// <summary>
        ///     The class that handles formatting and rendering to a stream.
        /// </summary>
        private StreamRenderer streamRenderer;

        /// <summary>
        ///     Set of names of formatting objects encountered but unknown.
        /// </summary>
        private readonly Hashtable unknownFOs = new Hashtable();

        /// <summary>
        ///     Sets the stream renderer that will be used as output.
        /// </summary>
        internal void SetStreamRenderer( StreamRenderer streamRenderer )
        {
            this.streamRenderer = streamRenderer;
        }

        /// <summary>
        ///     Add a mapping from element name to maker.
        /// </summary>
        internal void AddElementMapping( string namespaceURI, Hashtable table )
        {
            fobjTable.Add( namespaceURI, table );
            namespaces.Add( string.Intern( namespaceURI ) );
        }

        /// <summary>
        ///     Add a mapping from property name to maker.
        /// </summary>
        internal void AddPropertyMapping( string namespaceURI, Hashtable list )
        {
            PropertyListBuilder plb;
            plb = (PropertyListBuilder)propertylistTable[ namespaceURI ];
            if ( plb == null )
            {
                plb = new PropertyListBuilder();
                plb.AddList( list );
                propertylistTable.Add( namespaceURI, plb );
            }
            else
                plb.AddList( list );
        }

        private FObj.Maker GetFObjMaker( string uri, string localName )
        {
            var table = (Hashtable)fobjTable[ uri ];
            if ( table != null )
                return (FObj.Maker)table[ localName ];
            return null;
        }

        private void StartElement(
            string uri,
            string localName,
            Attributes attlist )
        {
            FObj fobj;

            FObj.Maker fobjMaker = GetFObjMaker( uri, localName );

            var currentListBuilder =
                (PropertyListBuilder)propertylistTable[ uri ];

            var foreignXML = false;
            if ( fobjMaker == null )
            {
                string fullName = uri + "^" + localName;
                if ( !unknownFOs.ContainsKey( fullName ) )
                {
                    unknownFOs.Add( fullName, "" );
                    FonetDriver.ActiveDriver.FireFonetError( "Unknown formatting object " + fullName );
                }
                if ( namespaces.Contains( string.Intern( uri ) ) )
                    fobjMaker = new Unknown.Maker();
                else
                {
                    fobjMaker = new UnknownXMLObj.Maker( uri, localName );
                    foreignXML = true;
                }
            }

            PropertyList list = null;
            if ( currentListBuilder != null )
                list = currentListBuilder.MakeList( uri, localName, attlist, currentFObj );
            else if ( foreignXML )
                list = null;
            else
            {
                if ( currentFObj == null )
                    throw new FonetException( "Invalid XML or missing namespace" );
                list = currentFObj.properties;
            }
            fobj = fobjMaker.Make( currentFObj, list );

            if ( rootFObj == null )
            {
                rootFObj = fobj;
                if ( !fobj.GetName().Equals( "fo:root" ) )
                    throw new FonetException( "Root element must" + " be root, not " + fobj.GetName() );
            }
            else if ( !( fobj is PageSequence ) )
                currentFObj.AddChild( fobj );

            currentFObj = fobj;
        }

        private void EndElement()
        {
            if ( currentFObj != null )
            {
                currentFObj.End();

                // If it is a page-sequence, then we can finally render it.
                // This is the biggest performance problem we have, we need
                // to be able to render prior to this point.
                if ( currentFObj is PageSequence )
                    streamRenderer.Render( (PageSequence)currentFObj );

                currentFObj = currentFObj.getParent();
            }
        }

        internal void Parse( XmlReader reader )
        {
            var buflen = 500;
            var buffer = new char[ buflen ];
            try
            {
                object nsuri = reader.NameTable.Add( "http://www.w3.org/2000/xmlns/" );

                FonetDriver.ActiveDriver.FireFonetInfo( "Building formatting object tree" );
                streamRenderer.StartRenderer();

                while ( reader.Read() )
                {
                    switch ( reader.NodeType )
                    {
                    case XmlNodeType.Element:
                        var atts = new Attributes();
                        while ( reader.MoveToNextAttribute() )
                        {
                            if ( !reader.NamespaceURI.Equals( nsuri ) )
                            {
                                var newAtt = new SaxAttribute();
                                newAtt.Name = reader.Name;
                                newAtt.NamespaceURI = reader.NamespaceURI;
                                newAtt.Value = reader.Value;
                                atts.attArray.Add( newAtt );
                            }
                        }
                        reader.MoveToElement();
                        StartElement( reader.NamespaceURI, reader.LocalName, atts.TrimArray() );
                        if ( reader.IsEmptyElement )
                            EndElement();
                        break;
                    case XmlNodeType.EndElement:
                        EndElement();
                        break;
                    case XmlNodeType.Text:
                        char[] chars = reader.ReadString().ToCharArray();
                        if ( currentFObj != null )
                            currentFObj.AddCharacters( chars, 0, chars.Length );
                        if ( reader.NodeType == XmlNodeType.Element )
                            goto case XmlNodeType.Element;
                        if ( reader.NodeType == XmlNodeType.EndElement )
                            goto case XmlNodeType.EndElement;
                        break;
                    default:
                        break;
                    }
                }
                FonetDriver.ActiveDriver.FireFonetInfo( "Parsing of document complete, stopping renderer" );
                streamRenderer.StopRenderer();
            }
            catch ( Exception exception )
            {
                FonetDriver.ActiveDriver.FireFonetError( exception.ToString() );
            }
            finally
            {
                if ( reader.ReadState != ReadState.Closed )
                    reader.Close();
            }
        }
    }

    internal class Attributes
    {
        internal ArrayList attArray = new ArrayList( 3 );

        // called by property list builder
        internal int getLength()
        {
            return attArray.Count;
        }

        // called by property list builder
        internal string getQName( int index )
        {
            var saxAtt = (SaxAttribute)attArray[ index ];
            return saxAtt.Name;
        }

        // called by property list builder
        internal string getValue( int index )
        {
            var saxAtt = (SaxAttribute)attArray[ index ];
            return saxAtt.Value;
        }

        // called by property list builder
        internal string getValue( string name )
        {
            foreach ( SaxAttribute att in attArray )
            {
                if ( att.Name.Equals( name ) )
                    return att.Value;
            }
            return null;
        }

        // only called above
        internal Attributes TrimArray()
        {
            attArray.TrimToSize();
            return this;
        }
    }

    // Only used by FO tree builder
    internal struct SaxAttribute
    {
        public string Name;
        public string NamespaceURI;
        public string Value;
    }
}