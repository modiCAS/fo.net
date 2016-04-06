using Fonet.Layout;

namespace Fonet.Fo.Flow
{
    internal class FootnoteBody : FObj
    {
        private readonly int _align = 0;

        private readonly int _alignLast = 0;

        private readonly int _endIndent = 0;

        private readonly int _lineHeight = 0;

        private readonly int _startIndent = 0;

        private readonly int _textIndent = 0;

        public FootnoteBody( FObj parent, PropertyList propertyList )
            : base( parent, propertyList )
        {
            Name = "fo:footnote-body";
            AreaClass = Fonet.Layout.AreaClass.SetAreaClass( Fonet.Layout.AreaClass.XslFootnote );
        }

        public new static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        public override Status Layout( Area area )
        {
            if ( Marker == MarkerStart )
                Marker = 0;
            var blockArea =
                new BlockArea( PropMgr.GetFontState( area.GetFontInfo() ),
                    area.GetAllocationWidth(), area.SpaceLeft(),
                    _startIndent, _endIndent, _textIndent, _align,
                    _alignLast, _lineHeight );
            blockArea.SetGeneratedBy( this );
            blockArea.IsFirst( true );
            blockArea.SetParent( area );
            blockArea.SetPage( area.GetPage() );
            blockArea.Start();

            blockArea.SetAbsoluteHeight( area.GetAbsoluteHeight() );
            blockArea.SetIDReferences( area.GetIDReferences() );

            blockArea.SetTableCellXOffset( area.GetTableCellXOffset() );

            int numChildren = Children.Count;
            for ( int i = Marker; i < numChildren; i++ )
            {
                var fo = (FoNode)Children[ i ];
                Status status;
                if ( ( status = fo.Layout( blockArea ) ).IsIncomplete() )
                {
                    ResetMarker();
                    return status;
                }
            }
            blockArea.End();
            area.AddChild( blockArea );
            area.IncreaseHeight( blockArea.GetHeight() );
            blockArea.IsLast( true );
            return new Status( Status.Ok );
        }

        internal new class Maker : FObj.Maker
        {
            public override FObj Make( FObj parent, PropertyList propertyList )
            {
                return new FootnoteBody( parent, propertyList );
            }
        }
    }
}