using System;

namespace Fonet.Pdf.Gdi.Font.Tables
{
    /// <summary>
    ///     Class that represents the Horizontal Metrics ('maxp') table.
    /// </summary>
    /// <remarks>
    ///     http://www.microsoft.com/typography/otspec/maxp.htm
    /// </remarks>
    internal class MaximumProfileTable : FontTable
    {
        /// <summary>
        ///     Maximum levels of recursion; 1 for simple components.
        ///     Only set if <i>versionNo</i> is 1.0.
        /// </summary>
        internal ushort MaxComponentDepth;

        /// <summary>
        ///     Maximum number of components referenced at "top level"
        ///     for any composite glyph.   Only set if
        ///     <i>versionNo</i> is 1.0.
        /// </summary>
        internal ushort MaxComponentElements;

        /// <summary>
        ///     Maximum contours in a composite glyph.  Only set if
        ///     <i>versionNo</i> is 1.0.
        /// </summary>
        internal ushort MaxCompositeContours;

        /// <summary>
        ///     Maximum points in a composite glyph.  Only set if
        ///     <i>versionNo</i> is 1.0.
        /// </summary>
        internal ushort MaxCompositePoints;

        /// <summary>
        ///     Maximum contours in a non-composite glyph.  Only set if
        ///     <i>versionNo</i> is 1.0.
        /// </summary>
        internal ushort MaxContours;

        /// <summary>
        ///     Number of FDEFs.   Only set if <i>versionNo</i> is 1.0.
        /// </summary>
        internal ushort MaxFunctionDefs;

        /// <summary>
        ///     Number of IDEFs.   Only set if <i>versionNo</i> is 1.0.
        /// </summary>
        internal ushort MaxInstructionDefs;

        /// <summary>
        ///     Maximum points in a non-composite glyph.
        /// </summary>
        internal ushort MaxPoints;

        /// <summary>
        ///     Maximum byte count for glyph instructions.  Only set
        ///     if <i>versionNo</i> is 1.0.
        /// </summary>
        internal ushort MaxSizeOfInstructions;

        /// <summary>
        ///     Maximum stack depth2.  Only set if <i>versionNo</i> is 1.0.
        /// </summary>
        internal ushort MaxStackElements;

        /// <summary>
        ///     Number of Storage Area locations.  Only set if
        ///     <i>versionNo</i> is 1.0.
        /// </summary>
        internal ushort MaxStorage;

        /// <summary>
        ///     Maximum points used in Z0.   Only set if
        ///     <i>versionNo</i> is 1.0.
        /// </summary>
        internal ushort MaxTwilightPoints;

        /// <summary>
        ///     1 if instructions do not use the twilight zone (Z0), or
        ///     2 if instructions do use Z0; should be set to 2 in most
        ///     cases.  Only set if <i>versionNo</i> is 1.0.
        /// </summary>
        internal ushort MaxZones;

        /// <summary>
        ///     The number of glyphs in the font.
        /// </summary>
        internal ushort NumGlyphs;

        /// <summary>
        ///     Table version number
        /// </summary>
        internal int VersionNo;

        /// <summary>
        ///     Initialises a new instance of the <see cref="MaximumProfileTable" />
        ///     class.
        /// </summary>
        /// <param name="entry"></param>
        public MaximumProfileTable( DirectoryEntry entry ) : base( TableNames.Maxp, entry )
        {
        }

        /// <summary>
        ///     Gets the number of glyphs
        /// </summary>
        public int GlyphCount
        {
            get { return NumGlyphs; }
            set { NumGlyphs = Convert.ToUInt16( value ); }
        }

        /// <summary>
        ///     Reads the contents of the "maxp" table from the supplied stream
        ///     at the current position.
        /// </summary>
        /// <param name="reader"></param>
        protected internal override void Read( FontFileReader reader )
        {
            FontFileStream stream = reader.Stream;

            // These two fields are common to versions 0.5 and 1.0
            VersionNo = stream.ReadFixed();
            NumGlyphs = stream.ReadUShort();

            // Version 1.0 of this table contains more data
            if ( VersionNo == 0x00010000 )
            {
                MaxPoints = stream.ReadUShort();
                MaxContours = stream.ReadUShort();
                MaxCompositePoints = stream.ReadUShort();
                MaxCompositeContours = stream.ReadUShort();
                MaxZones = stream.ReadUShort();
                MaxTwilightPoints = stream.ReadUShort();
                MaxStorage = stream.ReadUShort();
                MaxFunctionDefs = stream.ReadUShort();
                MaxInstructionDefs = stream.ReadUShort();
                MaxStackElements = stream.ReadUShort();
                MaxSizeOfInstructions = stream.ReadUShort();
                MaxComponentElements = stream.ReadUShort();
                MaxComponentDepth = stream.ReadUShort();
            }
        }

        protected internal override void Write( FontFileWriter writer )
        {
            FontFileStream stream = writer.Stream;

            // These two fields are common to versions 0.5 and 1.0
            stream.WriteFixed( VersionNo );
            stream.WriteUShort( NumGlyphs );

            // Version 1.0 of this table contains more data
            if ( VersionNo == 0x00010000 )
            {
                stream.WriteUShort( MaxPoints );
                stream.WriteUShort( MaxContours );
                stream.WriteUShort( MaxCompositePoints );
                stream.WriteUShort( MaxCompositeContours );
                stream.WriteUShort( MaxZones );
                stream.WriteUShort( MaxTwilightPoints );
                stream.WriteUShort( MaxStorage );
                stream.WriteUShort( MaxFunctionDefs );
                stream.WriteUShort( MaxInstructionDefs );
                stream.WriteUShort( MaxStackElements );
                stream.WriteUShort( MaxSizeOfInstructions );
                stream.WriteUShort( MaxComponentElements );
                stream.WriteUShort( MaxComponentDepth );
            }
        }
    }
}