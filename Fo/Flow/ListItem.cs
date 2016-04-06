using Fonet.Layout;

namespace Fonet.Fo.Flow
{
    internal class ListItem : FObj
    {
        private int align;
        private int alignLast;
        private BlockArea blockArea;
        private string id;
        private int lineHeight;
        private int spaceAfter;
        private int spaceBefore;

        public ListItem( FObj parent, PropertyList propertyList )
            : base( parent, propertyList )
        {
            name = "fo:list-item";
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
                spaceBefore =
                    properties.GetProperty( "space-before.optimum" ).GetLength().MValue();
                spaceAfter =
                    properties.GetProperty( "space-after.optimum" ).GetLength().MValue();
                id = properties.GetProperty( "id" ).GetString();

                area.getIDReferences().CreateID( id );

                marker = 0;
            }

            if ( area is BlockArea )
                area.end();

            if ( spaceBefore != 0 )
                area.addDisplaySpace( spaceBefore );

            blockArea =
                new BlockArea( propMgr.GetFontState( area.getFontInfo() ),
                    area.getAllocationWidth(), area.spaceLeft(), 0, 0,
                    0, align, alignLast, lineHeight );
            blockArea.setTableCellXOffset( area.getTableCellXOffset() );
            blockArea.setGeneratedBy( this );
            areasGenerated++;
            if ( areasGenerated == 1 )
                blockArea.isFirst( true );
            blockArea.addLineagePair( this, areasGenerated );

            blockArea.setParent( area );
            blockArea.setPage( area.getPage() );
            blockArea.start();

            blockArea.setAbsoluteHeight( area.getAbsoluteHeight() );
            blockArea.setIDReferences( area.getIDReferences() );

            int numChildren = children.Count;
            if ( numChildren != 2 )
                throw new FonetException( "list-item must have exactly two children" );
            var label = (ListItemLabel)children[ 0 ];
            var body = (ListItemBody)children[ 1 ];

            Status status;

            if ( marker == 0 )
            {
                area.getIDReferences().ConfigureID( id, area );

                status = label.Layout( blockArea );
                if ( status.isIncomplete() )
                    return status;
            }

            status = body.Layout( blockArea );
            if ( status.isIncomplete() )
            {
                blockArea.end();
                area.addChild( blockArea );
                area.increaseHeight( blockArea.GetHeight() );
                marker = 1;
                return status;
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

        public override int GetContentWidth()
        {
            if ( blockArea != null )
                return blockArea.getContentWidth();
            return 0;
        }

        internal new class Maker : FObj.Maker
        {
            public override FObj Make( FObj parent, PropertyList propertyList )
            {
                return new ListItem( parent, propertyList );
            }
        }
    }
}