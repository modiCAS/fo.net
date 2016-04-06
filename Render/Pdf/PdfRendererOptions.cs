using System;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Text;
using Fonet.Pdf.Gdi;

namespace Fonet.Render.Pdf
{
    /// <summary>
    ///     This class can be used to control various properties of PDF files
    ///     created by FO.NET.
    /// </summary>
    /// <remarks>
    ///     Can be used to control certain values in the generated PDF's information
    ///     dictionary.  These values are typically displayed in a document summary
    ///     dialog of PDF viewer applications.
    ///     This class also allows security settings to be specified that will
    ///     cause generated PDF files to be encrypted and optionally password protected.
    /// </remarks>
    public sealed class PdfRendererOptions
    {
        private FontType _fontType = FontType.Link;

        private StringCollection _keywords;

        /// <remarks>
        ///     The given initial value zero's out first two bits.
        ///     The PDF specification dictates that these entries must be 0.
        /// </remarks>
        private BitVector32 _permissions = new BitVector32( -4 );

        private readonly GdiPrivateFontCollection _privateFonts = new GdiPrivateFontCollection();

        public PdfRendererOptions()
        {
            Kerning = false;
        }

        /// <summary>
        ///     Specifies the Title of the PDF document.
        /// </summary>
        /// <value>
        ///     The default value is null.
        /// </value>
        /// <remarks>
        ///     This value will be embedded in the PDF information dictionary.
        /// </remarks>
        public string Title { get; set; }

        /// <summary>
        ///     Specifies the Subject of the PDF document.
        /// </summary>
        /// <value>
        ///     The default value is null.
        /// </value>
        /// <remarks>
        ///     This value will be embedded in the PDF information dictionary.
        /// </remarks>
        public string Subject { get; set; }

        /// <summary>
        ///     Specifies the Author of the PDF document.
        /// </summary>
        /// <value>
        ///     The default value is null.
        /// </value>
        /// <remarks>
        ///     This value will be embedded in the PDF information dictionary.
        /// </remarks>
        public string Author { get; set; }

        /// <summary>
        ///     Returns the Creator of the PDF document.
        /// </summary>
        /// <value>
        ///     This method will always return "XSL-FO http://www.w3.org/1999/XSL/Format".
        /// </value>
        internal string Creator
        {
            get { return "XSL-FO http://www.w3.org/1999/XSL/Format"; }
        }

        /// <summary>
        ///     Returns the Producer of the PDF document.
        /// </summary>
        /// <value>
        ///     This method will return the assembly name and version of FO.NET.
        /// </value>
        internal string Producer
        {
            get
            {
                AssemblyName assemName = Assembly.GetExecutingAssembly().GetName();
                return assemName.FullName + ", " + assemName.Version;
            }
        }

        /// <summary>
        ///     Returns a list of keywords as a comma-separated string
        /// </summary>
        /// <value>
        ///     If no keywords exist the empty string <see cref="string.Empty" /> is returned
        /// </value>
        internal string Keywords
        {
            get
            {
                var sb = new StringBuilder();
                if ( _keywords != null )
                {
                    for ( int i = 0, j = _keywords.Count; i < j; i++ )
                    {
                        sb.Append( _keywords[ i ] );
                        if ( i != j - 1 )
                            sb.Append( ", " );
                    }
                }
                return sb.ToString();
            }
        }

        /// <summary>
        ///     Specifies the owner password that will protect full access to any generated PDF documents.
        /// </summary>
        /// <remarks>
        ///     If either the owner or the user password is specified,
        ///     then the document will be encrypted.
        /// </remarks>
        /// <value>
        ///     The default value is null.
        /// </value>
        public string OwnerPassword { get; set; }

        /// <summary>
        ///     Specifies the user password that will protect access to any generated PDF documents.
        /// </summary>
        /// <remarks>
        ///     If either the owner or the user password is specified,
        ///     then the document will be encrypted.
        /// </remarks>
        /// <value>
        ///     The default value is null.
        /// </value>
        public string UserPassword { get; set; }

        /// <summary>
        ///     Returns true if any permissions have been set.
        /// </summary>
        internal bool HasPermissions
        {
            get { return _permissions.Data != -4; }
        }

        /// <summary>
        ///     Returns the PDF permissions encoded as an 32-bit integer.
        /// </summary>
        internal int Permissions
        {
            get { return _permissions.Data; }
        }

        /// <summary>
        ///     Enables or disables printing.
        /// </summary>
        /// <value>
        ///     The default value is true.
        /// </value>
        public bool EnablePrinting
        {
            get { return _permissions[ 4 ]; }
            set { _permissions[ 4 ] = value; }
        }

        /// <summary>
        ///     Enables or disables modifying document contents (other than text annotations and
        ///     interactive form fields).
        /// </summary>
        /// <value>
        ///     The default value is true.
        /// </value>
        public bool EnableModify
        {
            get { return _permissions[ 8 ]; }
            set { _permissions[ 8 ] = value; }
        }

        /// <summary>
        ///     Enables or disables copying of text and graphics.
        /// </summary>
        /// <value>
        ///     The default value is true.
        /// </value>
        public bool EnableCopy
        {
            get { return _permissions[ 16 ]; }
            set { _permissions[ 16 ] = value; }
        }

        /// <summary>
        ///     Enables or disables adding or modifying text annotations and interactive
        ///     form fields.
        /// </summary>
        /// <value>
        ///     The default value is true.
        /// </value>
        public bool EnableAdd
        {
            get { return _permissions[ 32 ]; }
            set { _permissions[ 32 ] = value; }
        }

        /// <summary>
        ///     Specifies how FO.NET should treat fonts.
        /// </summary>
        /// <value>
        ///     The default value is FontType.Link
        /// </value>
        public FontType FontType
        {
            get { return _fontType; }
            set { _fontType = value; }
        }

        /// <summary>
        ///     Gets or sets a value that indicates whether to enable kerning.
        /// </summary>
        /// <value>
        ///     The default value is <b>false</b>
        /// </value>
        public bool Kerning { get; set; }

        /// <summary>
        ///     Adds a keyword to the PDF document.
        /// </summary>
        /// <remarks>
        ///     Keywords are embedded in the PDF information dictionary.
        /// </remarks>
        /// <param name="keyword">The keyword to be added.</param>
        public void AddKeyword( string keyword )
        {
            if ( _keywords == null )
                _keywords = new StringCollection();
            _keywords.Add( keyword );
        }

        /// <summary>
        ///     Adds <i>fileInfo</i> to the private font collection.
        /// </summary>
        /// <param name="fileInfo">
        ///     Absolute path to a TrueType font or collection.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     If <i>fileInfo</i> is null.
        /// </exception>
        /// <exception cref="FileNotFoundException">
        ///     If <i>fileInfo</i> does not exist.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     If <i>fileInfo</i> has already been added.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     If <i>fileInfo</i> cannot be added to the system font collection.
        /// </exception>
        public void AddPrivateFont( FileInfo fileInfo )
        {
            _privateFonts.AddFontFile( fileInfo );
        }
    }
}