using System;
using Fonet.Layout;
using Fonet.Pdf;
using Fonet.Pdf.Gdi;

namespace Fonet.Render.Pdf.Fonts
{
    /// <summary>
    ///     A proxy object that delegates all operations to a concrete
    ///     subclass of the Font class.
    /// </summary>
    internal class ProxyFont : Font, IFontDescriptor
    {
        /// <summary>
        ///     Flag that indicates whether the underlying font has been loaded.
        /// </summary>
        private bool _fontLoaded;

        /// <summary>
        ///     Determines what type of "real" font to instantiate.
        /// </summary>
        private readonly FontType _fontType;

        /// <summary>
        ///     Font details such as face name, bold and italic flags
        /// </summary>
        private readonly FontProperties _properties;

        /// <summary>
        ///     The font that does all the work.
        /// </summary>
        private Font _realFont;

        /// <summary>
        ///     Class constructor.
        /// </summary>
        /// <param name="properties"></param>
        /// <param name="fontType"></param>
        public ProxyFont( FontProperties properties, FontType fontType )
        {
            this._properties = properties;
            this._fontType = fontType;
        }

        /// <summary>
        ///     Gets the underlying font.
        /// </summary>
        public Font RealFont
        {
            get
            {
                LoadIfNecessary();
                return _realFont;
            }
        }

        /// <summary>
        ///     Loads the underlying font.
        /// </summary>
        private void LoadIfNecessary()
        {
            if ( !_fontLoaded )
            {
                switch ( _fontType )
                {
                case FontType.Link:
                    _realFont = new TrueTypeFont( _properties );
                    break;
                case FontType.Embed:
                case FontType.Subset:
                    _realFont = LoadCidFont();
                    break;
                default:
                    throw new Exception( "Unknown font type: " + _fontType );
                }
                _fontLoaded = true;
            }
        }

        private Font LoadCidFont()
        {
            switch ( _fontType )
            {
            case FontType.Embed:
                _realFont = new Type2CidFont( _properties );
                break;
            case FontType.Subset:
                _realFont = new Type2CidSubsetFont( _properties );
                break;
            }

            // Flag that indicates whether the CID font should be replaced by a 
            // base 14 font due to a license violation
            var replaceFont = false;

            IFontDescriptor descriptor = _realFont.Descriptor;
            if ( !descriptor.IsEmbeddable )
            {
                FonetDriver.ActiveDriver.FireFonetWarning(
                    string.Format(
                        "Unable to embed font '{0}' because the license states embedding is not allowed.  Will default to Helvetica.",
                        _realFont.FontName ) );

                replaceFont = true;
            }

            // TODO: Do not permit subsetting if license does not allow it
            if ( _realFont is Type2CidSubsetFont && !descriptor.IsSubsettable )
            {
                FonetDriver.ActiveDriver.FireFonetWarning(
                    string.Format(
                        "Unable to subset font '{0}' because the license states subsetting is not allowed..  Will default to Helvetica.",
                        _realFont.FontName ) );

                replaceFont = true;
            }

            if ( replaceFont )
            {
                if ( _properties.IsBoldItalic )
                    _realFont = Base14Font.HelveticaBoldItalic;
                else if ( _properties.IsBold )
                    _realFont = Base14Font.HelveticaBold;
                else if ( _properties.IsItalic )
                    _realFont = Base14Font.HelveticaItalic;
                else
                    _realFont = Base14Font.Helvetica;
            }

            return _realFont;
        }

        #region Implementation of Font members

        public override PdfFontSubTypeEnum SubType
        {
            get
            {
                LoadIfNecessary();
                return _realFont.SubType;
            }
        }

        public override string FontName
        {
            get
            {
                LoadIfNecessary();
                return _realFont.FontName;
            }
        }

        public override PdfFontTypeEnum Type
        {
            get
            {
                LoadIfNecessary();
                return _realFont.Type;
            }
        }

        public override string Encoding
        {
            get
            {
                LoadIfNecessary();
                return _realFont.Encoding;
            }
        }

        public override IFontDescriptor Descriptor
        {
            get
            {
                LoadIfNecessary();
                return _realFont.Descriptor;
            }
        }

        public override bool MultiByteFont
        {
            get
            {
                LoadIfNecessary();
                return _realFont.MultiByteFont;
            }
        }

        public override ushort MapCharacter( char c )
        {
            LoadIfNecessary();
            return _realFont.MapCharacter( c );
        }

        public override int Ascender
        {
            get
            {
                LoadIfNecessary();
                return _realFont.Ascender;
            }
        }

        public override int Descender
        {
            get
            {
                LoadIfNecessary();
                return _realFont.Descender;
            }
        }

        public override int CapHeight
        {
            get
            {
                LoadIfNecessary();
                return _realFont.CapHeight;
            }
        }

        public override int FirstChar
        {
            get
            {
                LoadIfNecessary();
                return _realFont.FirstChar;
            }
        }

        public override int LastChar
        {
            get
            {
                LoadIfNecessary();
                return _realFont.LastChar;
            }
        }

        public override int GetWidth( ushort charIndex )
        {
            LoadIfNecessary();
            return _realFont.GetWidth( charIndex );
        }

        public override int[] Widths
        {
            get
            {
                LoadIfNecessary();
                return _realFont.Widths;
            }
        }

        #endregion

        #region Implementation of IFontDescriptior interface

        public int Flags
        {
            get
            {
                LoadIfNecessary();
                return _realFont.Descriptor.Flags;
            }
        }

        public int[] FontBBox
        {
            get
            {
                LoadIfNecessary();
                return _realFont.Descriptor.FontBBox;
            }
        }

        public int ItalicAngle
        {
            get
            {
                LoadIfNecessary();
                return _realFont.Descriptor.ItalicAngle;
            }
        }

        public int StemV
        {
            get
            {
                LoadIfNecessary();
                return _realFont.Descriptor.StemV;
            }
        }

        public bool HasKerningInfo
        {
            get
            {
                LoadIfNecessary();
                return _realFont.Descriptor.HasKerningInfo;
            }
        }

        public bool IsEmbeddable
        {
            get
            {
                LoadIfNecessary();
                return _realFont.Descriptor.IsEmbeddable;
            }
        }

        public bool IsSubsettable
        {
            get
            {
                LoadIfNecessary();
                return _realFont.Descriptor.IsSubsettable;
            }
        }

        public byte[] FontData
        {
            get
            {
                LoadIfNecessary();
                return _realFont.Descriptor.FontData;
            }
        }

        public GdiKerningPairs KerningInfo
        {
            get
            {
                LoadIfNecessary();
                return _realFont.Descriptor.KerningInfo;
            }
        }

        #endregion
    }
}