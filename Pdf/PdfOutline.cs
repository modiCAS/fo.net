using System.Collections;

namespace Fonet.Pdf
{
    /// <summary>
    ///     This represents a single Outline object in a PDF, including the root Outlines
    ///     object. Outlines provide the bookmark bar, usually rendered to the right of
    ///     a PDF document in user agents such as Acrobat Reader
    /// </summary>
    public class PdfOutline : PdfObject
    {
        private readonly PdfObjectReference _actionRef;

        private int _count;

        private PdfOutline _first;
        private PdfOutline _last;
        private PdfOutline _next;

        /// <summary>
        ///     Parent outline object. Root Outlines parent is null
        /// </summary>
        private PdfOutline _parent;

        private PdfOutline _prev;

        /// <summary>
        ///     List of sub-entries (outline objects)
        /// </summary>
        private readonly ArrayList _subentries;

        /// <summary>
        ///     Title to display for the bookmark entry
        /// </summary>
        private string _title;

        /// <summary>
        ///     Class constructor.
        /// </summary>
        /// <param name="objectId">The object id number</param>
        /// <param name="title">The title of the outline entry (can only be null for root Outlines obj)</param>
        /// <param name="action">The page which this outline refers to.</param>
        public PdfOutline( PdfObjectId objectId, string title, PdfObjectReference action )
            : base( objectId )
        {
            _subentries = new ArrayList();
            _count = 0;
            _parent = null;
            _prev = null;
            _next = null;
            _first = null;
            _last = null;
            this._title = title;
            _actionRef = action;
        }

        public void SetTitle( string title )
        {
            this._title = title;
        }

        /// <summary>
        ///     Add a sub element to this outline
        /// </summary>
        /// <param name="outline"></param>
        public void AddOutline( PdfOutline outline )
        {
            if ( _subentries.Count > 0 )
            {
                outline._prev = (PdfOutline)_subentries[ _subentries.Count - 1 ];
                outline._prev._next = outline;
            }
            else
                _first = outline;

            _subentries.Add( outline );
            outline._parent = this;

            IncrementCount(); // note: count is not just the immediate children

            _last = outline;
        }

        private void IncrementCount()
        {
            // count is a total of our immediate subentries and all descendent subentries
            _count++;
            if ( _parent != null )
                _parent.IncrementCount();
        }

        protected internal override void Write( PdfWriter writer )
        {
            var dict = new PdfDictionary();

            if ( _parent == null )
            {
                // root Outlines object
                if ( _first != null && _last != null )
                {
                    dict.Add( PdfName.Names.First, _first.GetReference() );
                    dict.Add( PdfName.Names.Last, _last.GetReference() );
                }
            }
            else
            {
                dict.Add( PdfName.Names.Title, new PdfString( _title ) );
                dict.Add( PdfName.Names.Parent, _parent.GetReference() );

                if ( _first != null && _last != null )
                {
                    dict.Add( PdfName.Names.First, _first.GetReference() );
                    dict.Add( PdfName.Names.Last, _last.GetReference() );
                }
                if ( _prev != null )
                    dict.Add( PdfName.Names.Prev, _prev.GetReference() );
                if ( _next != null )
                    dict.Add( PdfName.Names.Next, _next.GetReference() );
                if ( _count > 0 )
                    dict.Add( PdfName.Names.Count, new PdfNumeric( _count ) );

                if ( _actionRef != null )
                    dict.Add( PdfName.Names.A, _actionRef );
            }

            writer.Write( dict );
        }
    }
}