using System.Collections;
using System.Linq;
using Fonet.DataTypes;
using Fonet.Layout;

namespace Fonet.Fo
{
    internal class FObj : FoNode
    {
        private Hashtable _markerClassNames;

        protected string Name;

        public readonly PropertyList Properties;

        protected readonly PropertyManager PropMgr;

        protected FObj( FObj parent, PropertyList propertyList )
            : base( parent )
        {
            Properties = propertyList;
            propertyList.FObj = this;
            PropMgr = MakePropertyManager( propertyList );
            Name = "default FO";
            SetWritingMode();
        }

        public static Maker GetMaker()
        {
            return new Maker();
        }

        protected PropertyManager MakePropertyManager( PropertyList propertyList )
        {
            return new PropertyManager( propertyList );
        }

        protected internal virtual void AddCharacters( char[] data, int start, int length )
        {
            // ignore
        }

        public override Status Layout( Area area )
        {
            return new Status( Status.Ok );
        }

        public string GetName()
        {
            return Name;
        }

        protected internal virtual void Start()
        {
            // do nothing by default
        }

        protected internal virtual void End()
        {
            // do nothing by default
        }

        public override Property GetProperty( string name )
        {
            return Properties.GetProperty( name );
        }

        public virtual int GetContentWidth()
        {
            return 0;
        }

        public virtual void RemoveID( IDReferences idReferences )
        {
            if ( Properties.GetProperty( "id" ) == null
                || Properties.GetProperty( "id" ).GetString() == null )
                return;
            idReferences.RemoveID( Properties.GetProperty( "id" ).GetString() );
            int numChildren = Children.Count;
            for ( var i = 0; i < numChildren; i++ )
            {
                var child = (FoNode)Children[ i ];
                if ( child is FObj )
                    ( (FObj)child ).RemoveID( idReferences );
            }
        }

        public virtual bool GeneratesReferenceAreas()
        {
            return false;
        }

        protected virtual void SetWritingMode()
        {
            FObj p;
            FObj parent;
            for ( p = this;
                !p.GeneratesReferenceAreas() && ( parent = p.GetParent() ) != null;
                p = parent )
            {
            }

            Properties.SetWritingMode( p.GetProperty( "writing-mode" ).GetEnum() );
        }

        public void AddMarker( string markerClassName )
        {
            if ( Children != null )
            {
                if ( Children.Cast<FoNode>().Any( child => !child.MayPrecedeMarker() ) )
                {
                    throw new FonetException(
                        string.Format( "A fo:marker must be an initial child of '{0}'", GetName() ) );
                }
            }
            if ( _markerClassNames == null )
            {
                _markerClassNames = new Hashtable { { markerClassName, string.Empty } };
            }
            else if ( !_markerClassNames.ContainsKey( markerClassName ) )
                _markerClassNames.Add( markerClassName, string.Empty );
            else
            {
                throw new FonetException(
                    string.Format( "marker-class-name '{0}' already exists for this parent",
                        markerClassName ) );
            }
        }

        internal class Maker
        {
            public virtual FObj Make( FObj parent, PropertyList propertyList )
            {
                return new FObj( parent, propertyList );
            }
        }
    }
}