using System.Collections;
using Fonet.Fo.Flow;
using Fonet.Render.Pdf;

namespace Fonet.Layout
{
    internal class BlockArea : Area
    {
        protected int Align;
        protected int AlignLastLine;
        protected LineArea CurrentLineArea;
        protected LinkSet CurrentLinkSet;
        protected int EndIndent;
        protected int HalfLeading;
        protected bool HasLines;
        protected HyphenationProps HyphProps;
        protected int LineHeight;
        protected ArrayList PendingFootnotes;
        protected int StartIndent;
        protected int TextIndent;

        public BlockArea( FontState fontState, int allocationWidth, int maxHeight,
            int startIndent, int endIndent, int textIndent,
            int align, int alignLastLine, int lineHeight )
            : base( fontState, allocationWidth, maxHeight )
        {
            this.StartIndent = startIndent;
            this.EndIndent = endIndent;
            this.TextIndent = textIndent;
            ContentRectangleWidth = allocationWidth - startIndent
                - endIndent;
            this.Align = align;
            this.AlignLastLine = alignLastLine;
            this.LineHeight = lineHeight;

            if ( fontState != null )
                HalfLeading = ( lineHeight - fontState.FontSize ) / 2;
        }

        public override void Render( PdfRenderer renderer )
        {
            renderer.RenderBlockArea( this );
        }

        protected void AddLineArea( LineArea la )
        {
            if ( !la.IsEmpty() )
            {
                la.VerticalAlign();
                AddDisplaySpace( HalfLeading );
                int size = la.GetHeight();
                AddChild( la );
                IncreaseHeight( size );
                AddDisplaySpace( HalfLeading );
            }
            if ( PendingFootnotes != null )
            {
                foreach ( FootnoteBody fb in PendingFootnotes )
                {
                    Page page = GetPage();
                    if ( !Footnote.LayoutFootnote( page, fb, this ) )
                        page.AddPendingFootnote( fb );
                }
                PendingFootnotes = null;
            }
        }

        public LineArea GetCurrentLineArea()
        {
            if ( CurrentHeight + LineHeight > MaxHeight )
                return null;
            CurrentLineArea.ChangeHyphenation( HyphProps );
            HasLines = true;
            return CurrentLineArea;
        }

        public LineArea CreateNextLineArea()
        {
            if ( HasLines )
            {
                CurrentLineArea.Align( Align );
                AddLineArea( CurrentLineArea );
            }
            CurrentLineArea = new LineArea( FontState, LineHeight,
                HalfLeading, AllocationWidth,
                StartIndent, EndIndent,
                CurrentLineArea );
            CurrentLineArea.ChangeHyphenation( HyphProps );
            if ( CurrentHeight + LineHeight > MaxHeight )
                return null;
            return CurrentLineArea;
        }

        public void SetupLinkSet( LinkSet ls )
        {
            if ( ls != null )
            {
                CurrentLinkSet = ls;
                ls.SetYOffset( CurrentHeight );
            }
        }

        public override void End()
        {
            if ( HasLines )
            {
                CurrentLineArea.AddPending();
                CurrentLineArea.Align( AlignLastLine );
                AddLineArea( CurrentLineArea );
            }
        }

        public override void Start()
        {
            CurrentLineArea = new LineArea( FontState, LineHeight, HalfLeading,
                AllocationWidth,
                StartIndent + TextIndent, EndIndent,
                null );
        }

        public int GetEndIndent()
        {
            return EndIndent;
        }

        public int GetStartIndent()
        {
            return StartIndent;
        }

        public void SetIndents( int startIndent, int endIndent )
        {
            this.StartIndent = startIndent;
            this.EndIndent = endIndent;
            ContentRectangleWidth = AllocationWidth - startIndent
                - endIndent;
        }

        public override int SpaceLeft()
        {
            return MaxHeight - CurrentHeight -
                ( GetPaddingTop() + GetPaddingBottom()
                    + GetBorderTopWidth() + GetBorderBottomWidth() );
        }

        public int GetHalfLeading()
        {
            return HalfLeading;
        }

        public void SetHyphenation( HyphenationProps hyphProps )
        {
            this.HyphProps = hyphProps;
        }

        public void AddFootnote( FootnoteBody fb )
        {
            if ( PendingFootnotes == null )
                PendingFootnotes = new ArrayList();
            PendingFootnotes.Add( fb );
        }
    }
}