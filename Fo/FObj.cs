using System.Collections;
using Fonet.DataTypes;
using Fonet.Layout;

namespace Fonet.Fo
{
    internal class FObj : FONode
    {
        private Hashtable markerClassNames;

        protected string name;

        public PropertyList properties;

        protected PropertyManager propMgr;

        protected FObj( FObj parent, PropertyList propertyList )
            : base( parent )
        {
            properties = propertyList;
            propertyList.FObj = this;
            propMgr = MakePropertyManager( propertyList );
            name = "default FO";
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
            return new Status( Status.OK );
        }

        public string GetName()
        {
            return name;
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
            return properties.GetProperty( name );
        }

        public virtual int GetContentWidth()
        {
            return 0;
        }

        public virtual void RemoveID( IDReferences idReferences )
        {
            if ( properties.GetProperty( "id" ) == null
                || properties.GetProperty( "id" ).GetString() == null )
                return;
            idReferences.RemoveID( properties.GetProperty( "id" ).GetString() );
            int numChildren = children.Count;
            for ( var i = 0; i < numChildren; i++ )
            {
                var child = (FONode)children[ i ];
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
                !p.GeneratesReferenceAreas() && ( parent = p.getParent() ) != null;
                p = parent )
                ;
            properties.SetWritingMode( p.GetProperty( "writing-mode" ).GetEnum() );
        }

        public void AddMarker( string markerClassName )
        {
            if ( children != null )
            {
                for ( var i = 0; i < children.Count; i++ )
                {
                    var child = (FONode)children[ i ];
                    if ( !child.MayPrecedeMarker() )
                    {
                        throw new FonetException(
                            string.Format( "A fo:marker must be an initial child of '{0}'", GetName() ) );
                    }
                }
            }
            if ( markerClassNames == null )
            {
                markerClassNames = new Hashtable();
                markerClassNames.Add( markerClassName, string.Empty );
            }
            else if ( !markerClassNames.ContainsKey( markerClassName ) )
                markerClassNames.Add( markerClassName, string.Empty );
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