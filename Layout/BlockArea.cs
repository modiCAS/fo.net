using System.Collections;
using Fonet.Fo.Flow;
using Fonet.Render.Pdf;

namespace Fonet.Layout
{
    internal class BlockArea : Area
    {
        protected int align;
        protected int alignLastLine;
        protected LineArea currentLineArea;
        protected LinkSet currentLinkSet;
        protected int endIndent;
        protected int halfLeading;
        protected bool hasLines;
        protected HyphenationProps hyphProps;
        protected int lineHeight;
        protected ArrayList pendingFootnotes;
        protected int startIndent;
        protected int textIndent;

        public BlockArea( FontState fontState, int allocationWidth, int maxHeight,
            int startIndent, int endIndent, int textIndent,
            int align, int alignLastLine, int lineHeight )
            : base( fontState, allocationWidth, maxHeight )
        {
            this.startIndent = startIndent;
            this.endIndent = endIndent;
            this.textIndent = textIndent;
            contentRectangleWidth = allocationWidth - startIndent
                - endIndent;
            this.align = align;
            this.alignLastLine = alignLastLine;
            this.lineHeight = lineHeight;

            if ( fontState != null )
                halfLeading = ( lineHeight - fontState.FontSize ) / 2;
        }

        public override void render( PdfRenderer renderer )
        {
            renderer.RenderBlockArea( this );
        }

        protected void addLineArea( LineArea la )
        {
            if ( !la.isEmpty() )
            {
                la.verticalAlign();
                addDisplaySpace( halfLeading );
                int size = la.GetHeight();
                addChild( la );
                increaseHeight( size );
                addDisplaySpace( halfLeading );
            }
            if ( pendingFootnotes != null )
            {
                foreach ( FootnoteBody fb in pendingFootnotes )
                {
                    Page page = getPage();
                    if ( !Footnote.LayoutFootnote( page, fb, this ) )
                        page.addPendingFootnote( fb );
                }
                pendingFootnotes = null;
            }
        }

        public LineArea getCurrentLineArea()
        {
            if ( currentHeight + lineHeight > maxHeight )
                return null;
            currentLineArea.changeHyphenation( hyphProps );
            hasLines = true;
            return currentLineArea;
        }

        public LineArea createNextLineArea()
        {
            if ( hasLines )
            {
                currentLineArea.align( align );
                addLineArea( currentLineArea );
            }
            currentLineArea = new LineArea( fontState, lineHeight,
                halfLeading, allocationWidth,
                startIndent, endIndent,
                currentLineArea );
            currentLineArea.changeHyphenation( hyphProps );
            if ( currentHeight + lineHeight > maxHeight )
                return null;
            return currentLineArea;
        }

        public void setupLinkSet( LinkSet ls )
        {
            if ( ls != null )
            {
                currentLinkSet = ls;
                ls.setYOffset( currentHeight );
            }
        }

        public override void end()
        {
            if ( hasLines )
            {
                currentLineArea.addPending();
                currentLineArea.align( alignLastLine );
                addLineArea( currentLineArea );
            }
        }

        public override void start()
        {
            currentLineArea = new LineArea( fontState, lineHeight, halfLeading,
                allocationWidth,
                startIndent + textIndent, endIndent,
                null );
        }

        public int getEndIndent()
        {
            return endIndent;
        }

        public int getStartIndent()
        {
            return startIndent;
        }

        public void setIndents( int startIndent, int endIndent )
        {
            this.startIndent = startIndent;
            this.endIndent = endIndent;
            contentRectangleWidth = allocationWidth - startIndent
                - endIndent;
        }

        public override int spaceLeft()
        {
            return maxHeight - currentHeight -
                ( getPaddingTop() + getPaddingBottom()
                    + getBorderTopWidth() + getBorderBottomWidth() );
        }

        public int getHalfLeading()
        {
            return halfLeading;
        }

        public void setHyphenation( HyphenationProps hyphProps )
        {
            this.hyphProps = hyphProps;
        }

        public void addFootnote( FootnoteBody fb )
        {
            if ( pendingFootnotes == null )
                pendingFootnotes = new ArrayList();
            pendingFootnotes.Add( fb );
        }
    }
}