using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;

namespace Fonet.Fo
{
    internal sealed class PropertyListBuilder
    {
        private const string Fontsizeattr = "font-size";

        private readonly Hashtable _propertyListTable = new Hashtable();

        internal void AddList( Hashtable list )
        {
            foreach ( object o in list.Keys )
                _propertyListTable.Add( o, list[ o ] );
        }

        internal PropertyList MakeList(
            string ns,
            string elementName,
            Attributes attributes,
            FObj parentFo )
        {
            Debug.Assert( ns != null, "Namespace should never be null." );

            var space = "http://www.w3.org/TR/1999/XSL/Format";
            if ( ns != null )
                space = ns;

            PropertyList parentPropertyList = parentFo != null ? parentFo.Properties : null;
            PropertyList par = null;
            if ( parentPropertyList != null && space.Equals( parentPropertyList.GetNameSpace() ) )
                par = parentPropertyList;

            var p = new PropertyList( par, space, elementName );
            p.SetBuilder( this );

            var propsDone = new StringCollection();

            string fontsizeval = attributes.GetValue( Fontsizeattr );
            if ( fontsizeval != null )
            {
                PropertyMaker propertyMaker = FindMaker( Fontsizeattr );
                if ( propertyMaker != null )
                {
                    try
                    {
                        p.Add( Fontsizeattr,
                            propertyMaker.Make( p, fontsizeval, parentFo ) );
                    }
                    catch ( FonetException )
                    {
                    }
                }
                propsDone.Add( Fontsizeattr );
            }

            for ( var i = 0; i < attributes.GetLength(); i++ )
            {
                string attributeName = attributes.GetQName( i );
                int sepchar = attributeName.IndexOf( '.' );
                string propName = attributeName;
                string subpropName = null;
                if ( sepchar > -1 )
                {
                    propName = attributeName.Substring( 0, sepchar );
                    subpropName = attributeName.Substring( sepchar + 1 );
                }
                else if ( propsDone.Contains( propName ) )
                    continue;

                PropertyMaker propertyMaker = FindMaker( propName );

                if ( propertyMaker != null )
                {
                    try
                    {
                        Property propVal;
                        if ( subpropName != null )
                        {
                            Property baseProp = p.GetExplicitBaseProperty( propName );
                            if ( baseProp == null )
                            {
                                string baseValue = attributes.GetValue( propName );
                                if ( baseValue != null )
                                {
                                    baseProp = propertyMaker.Make( p, baseValue, parentFo );
                                    propsDone.Add( propName );
                                }
                            }
                            propVal = propertyMaker.Make( baseProp, subpropName,
                                p,
                                attributes.GetValue( i ),
                                parentFo );
                        }
                        else
                        {
                            propVal = propertyMaker.Make( p,
                                attributes.GetValue( i ),
                                parentFo );
                        }
                        if ( propVal != null )
                            p[ propName ] = propVal;
                    }
                    catch ( FonetException e )
                    {
                        FonetDriver.ActiveDriver.FireFonetError( e.Message );
                    }
                }
                else
                {
                    if ( !attributeName.StartsWith( "xmlns" ) )
                    {
                        FonetDriver.ActiveDriver.FireFonetWarning(
                            "property " + attributeName + " ignored" );
                    }
                }
            }

            return p;
        }

        internal Property GetSubpropValue(
            string propertyName, Property p, string subpropName )
        {
            PropertyMaker maker = FindMaker( propertyName );
            if ( maker != null )
                return maker.GetSubpropValue( p, subpropName );
            return null;
        }

        internal Property GetShorthand( PropertyList propertyList, string propertyName )
        {
            PropertyMaker propertyMaker = FindMaker( propertyName );
            if ( propertyMaker != null )
                return propertyMaker.GetShorthand( propertyList );
            FonetDriver.ActiveDriver.FireFonetError( "No maker for " + propertyName );
            return null;
        }

        internal Property MakeProperty( PropertyList propertyList, string propertyName )
        {
            Property p = null;
            PropertyMaker propertyMaker = FindMaker( propertyName );
            if ( propertyMaker != null )
                p = propertyMaker.Make( propertyList );
            else
                FonetDriver.ActiveDriver.FireFonetWarning( "property " + propertyName + " ignored" );
            return p;
        }

        internal PropertyMaker FindMaker( string propertyName )
        {
            return (PropertyMaker)_propertyListTable[ propertyName ];
        }
    }
}