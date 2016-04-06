using System.Collections;
using Fonet.Fo.Properties;

namespace Fonet.Fo
{
    internal class PropertyList : Hashtable
    {
        public const int LEFT = 0;

        public const int RIGHT = 1;

        public const int TOP = 2;

        public const int BOTTOM = 3;

        public const int HEIGHT = 4;

        public const int WIDTH = 5;

        public const int START = 0;

        public const int END = 1;

        public const int BEFORE = 2;

        public const int AFTER = 3;

        public const int BLOCKPROGDIM = 4;

        public const int INLINEPROGDIM = 5;

        private static readonly string[] sAbsNames =
        {
            "left", "right", "top", "bottom", "height", "width"
        };

        private static readonly string[] sRelNames =
        {
            "start", "end", "before", "after", "block-progression-dimension",
            "inline-progression-dimension"
        };

        private static readonly Hashtable wmtables = new Hashtable( 4 );

        private PropertyListBuilder builder;

        private readonly string element = "";

        private readonly string nmspace = "";

        private readonly PropertyList parentPropertyList;
        private byte[] wmtable;

        static PropertyList()
        {
            wmtables.Add(
                WritingMode.LR_TB, /* lr-tb */
                new byte[]
                {
                    START, END, BEFORE, AFTER, BLOCKPROGDIM, INLINEPROGDIM
                } );
            wmtables.Add(
                WritingMode.RL_TB, /* rl-tb */
                new byte[]
                {
                    END, START, BEFORE, AFTER, BLOCKPROGDIM, INLINEPROGDIM
                } );
            wmtables.Add(
                WritingMode.TB_RL, /* tb-rl */
                new byte[]
                {
                    AFTER, BEFORE, START, END, INLINEPROGDIM, BLOCKPROGDIM
                } );
        }

        public PropertyList(
            PropertyList parentPropertyList, string space, string el )
        {
            FObj = null;
            this.parentPropertyList = parentPropertyList;
            nmspace = space;
            element = el;
        }

        public FObj FObj { get; set; }

        public FObj getParentFObj()
        {
            if ( parentPropertyList != null )
                return parentPropertyList.FObj;
            return null;
        }

        public Property GetExplicitOrShorthandProperty( string propertyName )
        {
            int sepchar = propertyName.IndexOf( '.' );
            string baseName;
            if ( sepchar > -1 )
                baseName = propertyName.Substring( 0, sepchar );
            else
                baseName = propertyName;
            Property p = GetExplicitBaseProperty( baseName );
            if ( p == null )
                p = builder.GetShorthand( this, baseName );
            if ( p != null && sepchar > -1 )
            {
                return builder.GetSubpropValue( baseName, p,
                    propertyName.Substring( sepchar
                        + 1 ) );
            }
            return p;
        }

        public Property GetExplicitProperty( string propertyName )
        {
            int sepchar = propertyName.IndexOf( '.' );
            if ( sepchar > -1 )
            {
                string baseName = propertyName.Substring( 0, sepchar );
                Property p = GetExplicitBaseProperty( baseName );
                if ( p != null )
                {
                    return builder.GetSubpropValue(
                        baseName, p,
                        propertyName.Substring( sepchar
                            + 1 ) );
                }
                return null;
            }
            return (Property)this[ propertyName ];
        }

        public Property GetExplicitBaseProperty( string propertyName )
        {
            return (Property)this[ propertyName ];
        }

        public Property GetInheritedProperty( string propertyName )
        {
            if ( builder != null )
            {
                if ( parentPropertyList != null && IsInherited( propertyName ) )
                    return parentPropertyList.GetProperty( propertyName );
                try
                {
                    return builder.MakeProperty( this, propertyName );
                }
                catch ( FonetException e )
                {
                    FonetDriver.ActiveDriver.FireFonetError(
                        "Exception in getInherited(): property=" + propertyName + " : " + e );
                }
            }
            return null;
        }

        private bool IsInherited( string propertyName )
        {
            PropertyMaker propertyMaker = builder.FindMaker( propertyName );
            if ( propertyMaker != null )
                return propertyMaker.IsInherited();
            FonetDriver.ActiveDriver.FireFonetError( "Unknown property : " + propertyName );
            return true;
        }

        private Property FindProperty( string propertyName, bool bTryInherit )
        {
            PropertyMaker maker = builder.FindMaker( propertyName );

            Property p = null;
            if ( maker.IsCorrespondingForced( this ) )
                p = ComputeProperty( this, maker );
            else
            {
                p = GetExplicitBaseProperty( propertyName );

                if ( p == null )
                    p = ComputeProperty( this, maker );

                if ( p == null )
                    p = maker.GetShorthand( this );

                if ( p == null && bTryInherit )
                {
                    if ( parentPropertyList != null && maker.IsInherited() )
                        p = parentPropertyList.FindProperty( propertyName, true );
                }
            }
            return p;
        }


        private Property ComputeProperty(
            PropertyList propertyList, PropertyMaker propertyMaker )
        {
            Property p = null;
            try
            {
                p = propertyMaker.Compute( propertyList );
            }
            catch ( FonetException e )
            {
                FonetDriver.ActiveDriver.FireFonetError( e.Message );
            }
            return p;
        }

        public Property GetSpecifiedProperty( string propertyName )
        {
            return GetProperty( propertyName, false, false );
        }

        public Property GetProperty( string propertyName )
        {
            return GetProperty( propertyName, true, true );
        }

        private Property GetProperty( string propertyName, bool bTryInherit, bool bTryDefault )
        {
            if ( builder == null )
                FonetDriver.ActiveDriver.FireFonetError( "builder not set in PropertyList" );

            int sepchar = propertyName.IndexOf( '.' );
            string subpropName = null;
            if ( sepchar > -1 )
            {
                subpropName = propertyName.Substring( sepchar + 1 );
                propertyName = propertyName.Substring( 0, sepchar );
            }

            Property p = FindProperty( propertyName, bTryInherit );
            if ( p == null && bTryDefault )
            {
                try
                {
                    p = builder.MakeProperty( this, propertyName );
                }
                catch ( FonetException e )
                {
                    FonetDriver.ActiveDriver.FireFonetError( e.ToString() );
                }
            }

            if ( subpropName != null && p != null )
                return builder.GetSubpropValue( propertyName, p, subpropName );
            return p;
        }

        public void SetBuilder( PropertyListBuilder builder )
        {
            this.builder = builder;
        }

        public string GetNameSpace()
        {
            return nmspace;
        }

        public string GetElement()
        {
            return element;
        }

        public Property GetNearestSpecifiedProperty( string propertyName )
        {
            Property p = null;
            for ( PropertyList plist = this;
                p == null && plist != null;
                plist = plist.parentPropertyList )
                p = plist.GetExplicitProperty( propertyName );
            if ( p == null )
            {
                try
                {
                    p = builder.MakeProperty( this, propertyName );
                }
                catch ( FonetException e )
                {
                    FonetDriver.ActiveDriver.FireFonetError(
                        "Exception in getNearestSpecified(): property=" + propertyName + " : " + e );
                }
            }
            return p;
        }

        public Property GetFromParentProperty( string propertyName )
        {
            if ( parentPropertyList != null )
                return parentPropertyList.GetProperty( propertyName );
            if ( builder != null )
            {
                try
                {
                    return builder.MakeProperty( this, propertyName );
                }
                catch ( FonetException e )
                {
                    FonetDriver.ActiveDriver.FireFonetError(
                        "Exception in getFromParent(): property=" + propertyName + " : " + e );
                }
            }
            return null;
        }

        public string wmAbsToRel( int absdir )
        {
            if ( wmtable != null )
                return sRelNames[ wmtable[ absdir ] ];
            return string.Empty;
        }

        public string wmRelToAbs( int reldir )
        {
            if ( wmtable != null )
            {
                for ( var i = 0; i < wmtable.Length; i++ )
                {
                    if ( wmtable[ i ] == reldir )
                        return sAbsNames[ i ];
                }
            }
            return string.Empty;
        }

        public void SetWritingMode( int writingMode )
        {
            wmtable = (byte[])wmtables[ writingMode ];
        }
    }
}