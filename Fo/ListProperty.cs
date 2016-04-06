using System.Collections;

namespace Fonet.Fo
{
    internal class ListProperty : Property
    {
        protected ArrayList List;

        public ListProperty( Property prop )
        {
            List = new ArrayList();
            List.Add( prop );
        }

        public void AddProperty( Property prop )
        {
            List.Add( prop );
        }

        public override ArrayList GetList()
        {
            return List;
        }

        public override object GetObject()
        {
            return List;
        }

        internal class Maker : PropertyMaker
        {
            public Maker( string name ) : base( name )
            {
            }

            public override Property ConvertProperty(
                Property p, PropertyList propertyList, FObj fo )
            {
                if ( p is ListProperty )
                    return p;
                return new ListProperty( p );
            }
        }
    }
}