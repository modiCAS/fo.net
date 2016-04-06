using Fonet.Fo.Properties;
using Fonet.Layout;

namespace Fonet.Fo.Flow
{
    internal class BlockContainer : FObj
    {
        private AreaContainer areaContainer;
        private int bottom;
        private int height;
        private int left;
        private int position;
        private int right;
        private int span;
        private int top;
        private int width;

        protected BlockContainer( FObj parent, PropertyList propertyList )
            : base( parent, propertyList )
        {
            name = "fo:block-container";
            span = properties.GetProperty( "span" ).GetEnum();
        }

        public new static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        public override Status Layout( Area area )
        {
            if ( marker == MarkerStart )
            {
                AbsolutePositionProps mAbsProps = propMgr.GetAbsolutePositionProps();
                BorderAndPadding bap = propMgr.GetBorderAndPadding();
                BackgroundProps bProps = propMgr.GetBackgroundProps();
                MarginProps mProps = propMgr.GetMarginProps();

                marker = 0;
                position = properties.GetProperty( "position" ).GetEnum();
                top = properties.GetProperty( "top" ).GetLength().MValue();
                bottom = properties.GetProperty( "bottom" ).GetLength().MValue();
                left = properties.GetProperty( "left" ).GetLength().MValue();
                right = properties.GetProperty( "right" ).GetLength().MValue();
                width = properties.GetProperty( "width" ).GetLength().MValue();
                height = properties.GetProperty( "height" ).GetLength().MValue();
                span = properties.GetProperty( "span" ).GetEnum();

                string id = properties.GetProperty( "id" ).GetString();
                area.getIDReferences().InitializeID( id, area );
            }

            var container = (AreaContainer)area;
            if ( width == 0 && height == 0 )
            {
                width = right - left;
                height = bottom - top;
            }

            areaContainer =
                new AreaContainer( propMgr.GetFontState( container.getFontInfo() ),
                    container.getXPosition() + left,
                    container.GetYPosition() - top, width, height,
                    position );

            areaContainer.setPage( area.getPage() );
            areaContainer.setBackground( propMgr.GetBackgroundProps() );
            areaContainer.setBorderAndPadding( propMgr.GetBorderAndPadding() );
            areaContainer.start();

            areaContainer.setAbsoluteHeight( 0 );
            areaContainer.setIDReferences( area.getIDReferences() );

            int numChildren = children.Count;
            for ( int i = marker; i < numChildren; i++ )
            {
                var fo = (FObj)children[ i ];
                Status status = fo.Layout( areaContainer );
            }

            areaContainer.end();
            if ( position == Position.ABSOLUTE )
                areaContainer.SetHeight( height );
            area.addChild( areaContainer );

            return new Status( Status.OK );
        }

        public override int GetContentWidth()
        {
            if ( areaContainer != null )
                return areaContainer.getContentWidth();
            return 0;
        }

        public override bool GeneratesReferenceAreas()
        {
            return true;
        }

        public int GetSpan()
        {
            return span;
        }

        internal new class Maker : FObj.Maker
        {
            public override FObj Make( FObj parent, PropertyList propertyList )
            {
                return new BlockContainer( parent, propertyList );
            }
        }
    }
}