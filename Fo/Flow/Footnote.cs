using System.Collections;
using Fonet.Layout;

namespace Fonet.Fo.Flow
{
    internal class Footnote : FObj
    {
        public Footnote( FObj parent, PropertyList propertyList )
            : base( parent, propertyList )
        {
            name = "fo:footnote";
        }

        public new static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        public override Status Layout( Area area )
        {
            FONode inline = null;
            FONode fbody = null;
            if ( marker == MarkerStart )
                marker = 0;
            int numChildren = children.Count;
            for ( int i = marker; i < numChildren; i++ )
            {
                var fo = (FONode)children[ i ];
                if ( fo is Inline )
                {
                    inline = fo;
                    Status status = fo.Layout( area );
                    if ( status.isIncomplete() )
                        return status;
                }
                else if ( inline != null && fo is FootnoteBody )
                {
                    fbody = fo;
                    if ( area is BlockArea )
                        ( (BlockArea)area ).addFootnote( (FootnoteBody)fbody );
                    else
                    {
                        Page page = area.getPage();
                        LayoutFootnote( page, (FootnoteBody)fbody, area );
                    }
                }
            }
            if ( fbody == null )
            {
                FonetDriver.ActiveDriver.FireFonetWarning(
                    "No footnote-body in footnote" );
            }
            if ( area is BlockArea )
            {
            }
            return new Status( Status.OK );
        }

        public static bool LayoutFootnote( Page p, FootnoteBody fb, Area area )
        {
            try
            {
                BodyAreaContainer bac = p.getBody();
                AreaContainer footArea = bac.getFootnoteReferenceArea();
                footArea.setIDReferences( bac.getIDReferences() );
                int basePos = footArea.GetCurrentYPosition()
                    - footArea.GetHeight();
                int oldHeight = footArea.GetHeight();
                if ( area != null )
                {
                    footArea.setMaxHeight( area.getMaxHeight() - area.GetHeight()
                        + footArea.GetHeight() );
                }
                else
                {
                    footArea.setMaxHeight( bac.getMaxHeight()
                        + footArea.GetHeight() );
                }
                Status status = fb.Layout( footArea );
                if ( status.isIncomplete() )
                    return false;
                if ( area != null )
                {
                    area.setMaxHeight( area.getMaxHeight()
                        - footArea.GetHeight() + oldHeight );
                }
                if ( bac.getFootnoteState() == 0 )
                {
                    Area ar = bac.getMainReferenceArea();
                    DecreaseMaxHeight( ar, footArea.GetHeight() - oldHeight );
                    footArea.setYPosition( basePos + footArea.GetHeight() );
                }
            }
            catch ( FonetException )
            {
                return false;
            }
            return true;
        }

        protected static void DecreaseMaxHeight( Area ar, int change )
        {
            ar.setMaxHeight( ar.getMaxHeight() - change );
            ArrayList childs = ar.getChildren();
            foreach ( object obj in childs )
            {
                if ( obj is Area )
                {
                    var childArea = (Area)obj;
                    DecreaseMaxHeight( childArea, change );
                }
            }
        }

        internal new class Maker : FObj.Maker
        {
            public override FObj Make( FObj parent, PropertyList propertyList )
            {
                return new Footnote( parent, propertyList );
            }
        }
    }
}