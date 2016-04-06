using Fonet.Layout;

namespace Fonet.Fo.Flow
{
    internal class FootnoteBody : FObj
    {
        private readonly int align = 0;

        private readonly int alignLast = 0;

        private readonly int endIndent = 0;

        private readonly int lineHeight = 0;

        private readonly int startIndent = 0;

        private readonly int textIndent = 0;

        public FootnoteBody( FObj parent, PropertyList propertyList )
            : base( parent, propertyList )
        {
            name = "fo:footnote-body";
            areaClass = AreaClass.setAreaClass( AreaClass.XSL_FOOTNOTE );
        }

        public new static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        public override Status Layout( Area area )
        {
            if ( marker == MarkerStart )
                marker = 0;
            var blockArea =
                new BlockArea( propMgr.GetFontState( area.getFontInfo() ),
                    area.getAllocationWidth(), area.spaceLeft(),
                    startIndent, endIndent, textIndent, align,
                    alignLast, lineHeight );
            blockArea.setGeneratedBy( this );
            blockArea.isFirst( true );
            blockArea.setParent( area );
            blockArea.setPage( area.getPage() );
            blockArea.start();

            blockArea.setAbsoluteHeight( area.getAbsoluteHeight() );
            blockArea.setIDReferences( area.getIDReferences() );

            blockArea.setTableCellXOffset( area.getTableCellXOffset() );

            int numChildren = children.Count;
            for ( int i = marker; i < numChildren; i++ )
            {
                var fo = (FONode)children[ i ];
                Status status;
                if ( ( status = fo.Layout( blockArea ) ).isIncomplete() )
                {
                    ResetMarker();
                    return status;
                }
            }
            blockArea.end();
            area.addChild( blockArea );
            area.increaseHeight( blockArea.GetHeight() );
            blockArea.isLast( true );
            return new Status( Status.OK );
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