using Fonet.Layout;

namespace Fonet.Fo.Flow
{
    internal class ListBlock : FObj
    {
        private int align;
        private int alignLast;
        private int endIndent;
        private int lineHeight;
        private int spaceAfter;
        private int spaceBefore;
        private int startIndent;

        public ListBlock( FObj parent, PropertyList propertyList )
            : base( parent, propertyList )
        {
            name = "fo:list-block";
        }

        public new static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        public override Status Layout( Area area )
        {
            if ( marker == MarkerStart )
            {
                AccessibilityProps mAccProps = propMgr.GetAccessibilityProps();
                AuralProps mAurProps = propMgr.GetAuralProps();
                BorderAndPadding bap = propMgr.GetBorderAndPadding();
                BackgroundProps bProps = propMgr.GetBackgroundProps();
                MarginProps mProps = propMgr.GetMarginProps();
                RelativePositionProps mRelProps = propMgr.GetRelativePositionProps();

                align = properties.GetProperty( "text-align" ).GetEnum();
                alignLast = properties.GetProperty( "text-align-last" ).GetEnum();
                lineHeight =
                    properties.GetProperty( "line-height" ).GetLength().MValue();
                startIndent =
                    properties.GetProperty( "start-indent" ).GetLength().MValue();
                endIndent =
                    properties.GetProperty( "end-indent" ).GetLength().MValue();
                spaceBefore =
                    properties.GetProperty( "space-before.optimum" ).GetLength().MValue();
                spaceAfter =
                    properties.GetProperty( "space-after.optimum" ).GetLength().MValue();

                marker = 0;

                if ( area is BlockArea )
                    area.end();

                if ( spaceBefore != 0 )
                    area.addDisplaySpace( spaceBefore );

                if ( isInTableCell )
                {
                    startIndent += forcedStartOffset;
                    endIndent += area.getAllocationWidth() - forcedWidth
                        - forcedStartOffset;
                }

                string id = properties.GetProperty( "id" ).GetString();
                area.getIDReferences().InitializeID( id, area );
            }

            var blockArea =
                new BlockArea( propMgr.GetFontState( area.getFontInfo() ),
                    area.getAllocationWidth(), area.spaceLeft(),
                    startIndent, endIndent, 0, align, alignLast,
                    lineHeight );
            blockArea.setTableCellXOffset( area.getTableCellXOffset() );
            blockArea.setGeneratedBy( this );
            areasGenerated++;
            if ( areasGenerated == 1 )
                blockArea.isFirst( true );
            blockArea.addLineagePair( this, areasGenerated );

            blockArea.setParent( area );
            blockArea.setPage( area.getPage() );
            blockArea.setBackground( propMgr.GetBackgroundProps() );
            blockArea.start();

            blockArea.setAbsoluteHeight( area.getAbsoluteHeight() );
            blockArea.setIDReferences( area.getIDReferences() );

            int numChildren = children.Count;
            for ( int i = marker; i < numChildren; i++ )
            {
                if ( !( children[ i ] is ListItem ) )
                {
                    FonetDriver.ActiveDriver.FireFonetError(
                        "Children of list-blocks must be list-items" );
                    return new Status( Status.OK );
                }
                var listItem = (ListItem)children[ i ];
                Status status;
                if ( ( status = listItem.Layout( blockArea ) ).isIncomplete() )
                {
                    if ( status.getCode() == Status.AREA_FULL_NONE && i > 0 )
                        status = new Status( Status.AREA_FULL_SOME );
                    marker = i;
                    blockArea.end();
                    area.addChild( blockArea );
                    area.increaseHeight( blockArea.GetHeight() );
                    return status;
                }
            }

            blockArea.end();
            area.addChild( blockArea );
            area.increaseHeight( blockArea.GetHeight() );

            if ( spaceAfter != 0 )
                area.addDisplaySpace( spaceAfter );

            if ( area is BlockArea )
                area.start();

            blockArea.isLast( true );
            return new Status( Status.OK );
        }

        internal new class Maker : FObj.Maker
        {
            public override FObj Make( FObj parent, PropertyList propertyList )
            {
                return new ListBlock( parent, propertyList );
            }
        }
    }
}