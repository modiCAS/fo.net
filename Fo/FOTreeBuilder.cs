using System;
using System.Collections;
using System.Xml;
using Fonet.Fo.Pagination;

namespace Fonet.Fo
{
    /// <summary>
    ///     Builds the formatting object tree.
    /// </summary>
    internal sealed class FoTreeBuilder
    {
        /// <summary>
        ///     Current formatting object being handled.
        /// </summary>
        private FObj _currentFObj;

        /// <summary>
        ///     Table mapping element names to the makers of objects
        ///     representing formatting objects.
        /// </summary>
        private readonly Hashtable _fobjTable = new Hashtable();

        private readonly ArrayList _namespaces = new ArrayList();

        /// <summary>
        ///     Class that builds a property list for each formatting object.
        /// </summary>
        private readonly Hashtable _propertylistTable = new Hashtable();

        /// <summary>
        ///     The root of the formatting object tree.
        /// </summary>
        private FObj _rootFObj;

        /// <summary>
        ///     The class that handles formatting and rendering to a stream.
        /// </summary>
        private StreamRenderer _streamRenderer;

        /// <summary>
        ///     Set of names of formatting objects encountered but unknown.
        /// </summary>
        private readonly Hashtable _unknownFOs = new Hashtable();

        /// <summary>
        ///     Sets the stream renderer that will be used as output.
        /// </summary>
        internal void SetStreamRenderer( StreamRenderer streamRenderer )
        {
            this._streamRenderer = streamRenderer;
        }

        /// <summary>
        ///     Add a mapping from element name to maker.
        /// </summary>
        internal void AddElementMapping( string namespaceUri, Hashtable table )
        {
            _fobjTable.Add( namespaceUri, table );
            _namespaces.Add( string.Intern( namespaceUri ) );
        }

        /// <summary>
        ///     Add a mapping from property name to maker.
        /// </summary>
        internal void AddPropertyMapping( string namespaceUri, Hashtable list )
        {
            var plb = (PropertyListBuilder)_propertylistTable[ namespaceUri ];
            if ( plb == null )
            {
                plb = new PropertyListBuilder();
                plb.AddList( list );
                _propertylistTable.Add( namespaceUri, plb );
            }
            else
                plb.AddList( list );
        }

        private FObj.Maker GetFObjMaker( string uri, string localName )
        {
            var table = (Hashtable)_fobjTable[ uri ];
            if ( table != null )
                return (FObj.Maker)table[ localName ];
            return null;
        }

        private void StartElement(
            string uri,
            string localName,
            Attributes attlist )
        {
            FObj.Maker fobjMaker = GetFObjMaker( uri, localName );

            var currentListBuilder =
                (PropertyListBuilder)_propertylistTable[ uri ];

            var foreignXml = false;
            if ( fobjMaker == null )
            {
                string fullName = uri + "^" + localName;
                if ( !_unknownFOs.ContainsKey( fullName ) )
                {
                    _unknownFOs.Add( fullName, "" );
                    FonetDriver.ActiveDriver.FireFonetError( "Unknown formatting object " + fullName );
                }
                if ( _namespaces.Contains( string.Intern( uri ) ) )
                    fobjMaker = new Unknown.Maker();
                else
                {
                    fobjMaker = new UnknownXmlObj.Maker( uri, localName );
                    foreignXml = true;
                }
            }

            PropertyList list;
            if ( currentListBuilder != null )
                list = currentListBuilder.MakeList( uri, localName, attlist, _currentFObj );
            else if ( foreignXml )
                list = null;
            else
            {
                if ( _currentFObj == null )
                    throw new FonetException( "Invalid XML or missing namespace" );
                list = _currentFObj.Properties;
            }

            FObj fobj = fobjMaker.Make( _currentFObj, list );

            if ( _rootFObj == null )
            {
                _rootFObj = fobj;
                if ( !fobj.GetName().Equals( "fo:root" ) )
                    throw new FonetException( "Root element must" + " be root, not " + fobj.GetName() );
            }
            else if ( !( fobj is PageSequence ) )
                _currentFObj.AddChild( fobj );

            _currentFObj = fobj;
        }

        private void EndElement()
        {
            if ( _currentFObj == null ) return;

            _currentFObj.End();

            // If it is a page-sequence, then we can finally render it.
            // This is the biggest performance problem we have, we need
            // to be able to render prior to this point.
            var sequence = _currentFObj as PageSequence;
            if ( sequence != null ) _streamRenderer.Render( sequence );

            _currentFObj = _currentFObj.GetParent();
        }

        internal void Parse( XmlReader reader )
        {
            try
            {
                object nsuri = reader.NameTable.Add( "http://www.w3.org/2000/xmlns/" );

                FonetDriver.ActiveDriver.FireFonetInfo( "Building formatting object tree" );
                _streamRenderer.StartRenderer();

                while ( reader.Read() )
                {
                    switch ( reader.NodeType )
                    {
                    case XmlNodeType.Element:
                        var atts = new Attributes();
                        while ( reader.MoveToNextAttribute() )
                        {
                            if ( reader.NamespaceURI.Equals( nsuri ) ) continue;

                            var newAtt = new SaxAttribute
                            {
                                Name = reader.Name,
                                NamespaceUri = reader.NamespaceURI,
                                Value = reader.Value
                            };
                            atts.AttArray.Add( newAtt );
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
                        if ( _currentFObj != null )
                            _currentFObj.AddCharacters( chars, 0, chars.Length );
                        if ( reader.NodeType == XmlNodeType.Element )
                            goto case XmlNodeType.Element;
                        if ( reader.NodeType == XmlNodeType.EndElement )
                            goto case XmlNodeType.EndElement;
                        break;
                    }
                }
                FonetDriver.ActiveDriver.FireFonetInfo( "Parsing of document complete, stopping renderer" );
                _streamRenderer.StopRenderer();
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
        internal ArrayList AttArray = new ArrayList( 3 );

        // called by property list builder
        internal int GetLength()
        {
            return AttArray.Count;
        }

        // called by property list builder
        internal string GetQName( int index )
        {
            var saxAtt = (SaxAttribute)AttArray[ index ];
            return saxAtt.Name;
        }

        // called by property list builder
        internal string GetValue( int index )
        {
            var saxAtt = (SaxAttribute)AttArray[ index ];
            return saxAtt.Value;
        }

        // called by property list builder
        internal string GetValue( string name )
        {
            foreach ( SaxAttribute att in AttArray )
            {
                if ( att.Name.Equals( name ) )
                    return att.Value;
            }
            return null;
        }

        // only called above
        internal Attributes TrimArray()
        {
            AttArray.TrimToSize();
            return this;
        }
    }

    // Only used by FO tree builder
    internal struct SaxAttribute
    {
        public string Name;
        public string NamespaceUri;
        public string Value;
    }
}