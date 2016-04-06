using Fonet.Layout;

namespace Fonet.Fo
{
    internal class FObjMixed : FObj
    {
        protected TextState ts;

        protected FObjMixed( FObj parent, PropertyList propertyList )
            : base( parent, propertyList )
        {
        }

        public new static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        public TextState getTextState()
        {
            return ts;
        }

        protected internal override void AddCharacters( char[] data, int start, int length )
        {
            var ft = new FOText( data, start, length, this );
            ft.setUnderlined( ts.getUnderlined() );
            ft.setOverlined( ts.getOverlined() );
            ft.setLineThrough( ts.getLineThrough() );
            AddChild( ft );
        }

        public override Status Layout( Area area )
        {
            if ( properties != null )
            {
                Property prop = properties.GetProperty( "id" );
                if ( prop != null )
                {
                    string id = prop.GetString();

                    if ( marker == MarkerStart )
                    {
                        if ( area.getIDReferences() != null )
                            area.getIDReferences().CreateID( id );
                        marker = 0;
                    }

                    if ( marker == 0 )
                    {
                        if ( area.getIDReferences() != null )
                            area.getIDReferences().ConfigureID( id, area );
                    }
                }
            }

            int numChildren = children.Count;
            for ( int i = marker; i < numChildren; i++ )
            {
                var fo = (FONode)children[ i ];
                Status status;
                if ( ( status = fo.Layout( area ) ).isIncomplete() )
                {
                    marker = i;
                    return status;
                }
            }
            return new Status( Status.OK );
        }

        internal new class Maker : FObj.Maker
        {
            public override FObj Make( FObj parent, PropertyList propertyList )
            {
                return new FObjMixed( parent, propertyList );
            }
        }
    }
}