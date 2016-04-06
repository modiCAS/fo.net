using System.Collections;
using System.IO;
using Fonet.DataTypes;
using Fonet.Fo.Pagination;

namespace Fonet.Layout
{
    internal class AreaTree
    {
        private FontInfo _fontInfo;

        private readonly StreamRenderer _streamRenderer;

        public AreaTree( StreamRenderer streamRenderer )
        {
            _streamRenderer = streamRenderer;
        }

        public void SetFontInfo( FontInfo fontInfo )
        {
            _fontInfo = fontInfo;
        }

        public FontInfo GetFontInfo()
        {
            return _fontInfo;
        }

        public void AddPage( Page page )
        {
            try
            {
                _streamRenderer.QueuePage( page );
            }
            catch ( IOException e )
            {
                throw new FonetException( "", e );
            }
        }

        public IDReferences GetIDReferences()
        {
            return _streamRenderer.GetIDReferences();
        }

        public ArrayList GetDocumentMarkers()
        {
            return _streamRenderer.GetDocumentMarkers();
        }

        public PageSequence GetCurrentPageSequence()
        {
            return _streamRenderer.GetCurrentPageSequence();
        }

        public ArrayList GetCurrentPageSequenceMarkers()
        {
            return _streamRenderer.GetCurrentPageSequenceMarkers();
        }
    }
}