using Fonet.Layout;

namespace Fonet.Fo
{
    internal class FObjMixed : FObj
    {
        protected TextState Ts;

        protected FObjMixed( FObj parent, PropertyList propertyList )
            : base( parent, propertyList )
        {
        }

        public new static FObj.Maker GetMaker()
        {
            return new Maker();
        }

        public TextState GetTextState()
        {
            return Ts;
        }

        protected internal override void AddCharacters( char[] data, int start, int length )
        {
            var ft = new FoText( data, start, length, this );
            ft.SetUnderlined( Ts.getUnderlined() );
            ft.SetOverlined( Ts.getOverlined() );
            ft.SetLineThrough( Ts.getLineThrough() );
            AddChild( ft );
        }

        public override Status Layout( Area area )
        {
            if ( Properties != null )
            {
                Property prop = Properties.GetProperty( "id" );
                if ( prop != null )
                {
                    string id = prop.GetString();

                    if ( Marker == MarkerStart )
                    {
                        if ( area.getIDReferences() != null )
                            area.getIDReferences().CreateID( id );
                        Marker = 0;
                    }

                    if ( Marker == 0 )
                    {
                        if ( area.getIDReferences() != null )
                            area.getIDReferences().ConfigureID( id, area );
                    }
                }
            }

            int numChildren = Children.Count;
            for ( int i = Marker; i < numChildren; i++ )
            {
                var fo = (FoNode)Children[ i ];
                Status status;
                if ( ( status = fo.Layout( area ) ).IsIncomplete() )
                {
                    Marker = i;
                    return status;
                }
            }
            return new Status( Status.Ok );
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