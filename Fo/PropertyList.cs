using System.Collections;
using Fonet.Fo.Properties;

namespace Fonet.Fo
{
    internal class PropertyList : Hashtable
    {
        public const int Left = 0;

        public const int Right = 1;

        public const int Top = 2;

        public const int Bottom = 3;

        public const int Height = 4;

        public const int Width = 5;

        public const int Start = 0;

        public const int End = 1;

        public const int Before = 2;

        public const int After = 3;

        public const int Blockprogdim = 4;

        public const int Inlineprogdim = 5;

        private static readonly string[] SAbsNames =
        {
            "left", "right", "top", "bottom", "height", "width"
        };

        private static readonly string[] SRelNames =
        {
            "start", "end", "before", "after", "block-progression-dimension",
            "inline-progression-dimension"
        };

        private static readonly Hashtable Wmtables = new Hashtable( 4 );

        private PropertyListBuilder _builder;

        private readonly string _element = "";

        private readonly string _nmspace = "";

        private readonly PropertyList _parentPropertyList;
        private byte[] _wmtable;

        static PropertyList()
        {
            Wmtables.Add(
                WritingMode.LrTb, /* lr-tb */
                new byte[]
                {
                    Start, End, Before, After, Blockprogdim, Inlineprogdim
                } );
            Wmtables.Add(
                WritingMode.RlTb, /* rl-tb */
                new byte[]
                {
                    End, Start, Before, After, Blockprogdim, Inlineprogdim
                } );
            Wmtables.Add(
                WritingMode.TbRl, /* tb-rl */
                new byte[]
                {
                    After, Before, Start, End, Inlineprogdim, Blockprogdim
                } );
        }

        public PropertyList(
            PropertyList parentPropertyList, string space, string el )
        {
            FObj = null;
            this._parentPropertyList = parentPropertyList;
            _nmspace = space;
            _element = el;
        }

        public FObj FObj { get; set; }

        public FObj GetParentFObj()
        {
            if ( _parentPropertyList != null )
                return _parentPropertyList.FObj;
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
                p = _builder.GetShorthand( this, baseName );
            if ( p != null && sepchar > -1 )
            {
                return _builder.GetSubpropValue( baseName, p,
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
                    return _builder.GetSubpropValue(
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
            if ( _builder != null )
            {
                if ( _parentPropertyList != null && IsInherited( propertyName ) )
                    return _parentPropertyList.GetProperty( propertyName );
                try
                {
                    return _builder.MakeProperty( this, propertyName );
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
            PropertyMaker propertyMaker = _builder.FindMaker( propertyName );
            if ( propertyMaker != null )
                return propertyMaker.IsInherited();
            FonetDriver.ActiveDriver.FireFonetError( "Unknown property : " + propertyName );
            return true;
        }

        private Property FindProperty( string propertyName, bool bTryInherit )
        {
            PropertyMaker maker = _builder.FindMaker( propertyName );

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
                    if ( _parentPropertyList != null && maker.IsInherited() )
                        p = _parentPropertyList.FindProperty( propertyName, true );
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

        public T GetProperty<T>( string propertyName )
        {
            var property = GetProperty( propertyName ) as EnumProperty<T>;
            return property != null ? property.Value : default(T);
        }

        public Property GetProperty( string propertyName )
        {
            return GetProperty( propertyName, true, true );
        }

        private Property GetProperty( string propertyName, bool bTryInherit, bool bTryDefault )
        {
            if ( _builder == null )
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
                    p = _builder.MakeProperty( this, propertyName );
                }
                catch ( FonetException e )
                {
                    FonetDriver.ActiveDriver.FireFonetError( e.ToString() );
                }
            }

            if ( subpropName != null && p != null )
                return _builder.GetSubpropValue( propertyName, p, subpropName );
            return p;
        }

        public void SetBuilder( PropertyListBuilder builder )
        {
            this._builder = builder;
        }

        public string GetNameSpace()
        {
            return _nmspace;
        }

        public string GetElement()
        {
            return _element;
        }

        public Property GetNearestSpecifiedProperty( string propertyName )
        {
            Property p = null;
            for ( PropertyList plist = this;
                p == null && plist != null;
                plist = plist._parentPropertyList )
                p = plist.GetExplicitProperty( propertyName );
            if ( p == null )
            {
                try
                {
                    p = _builder.MakeProperty( this, propertyName );
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
            if ( _parentPropertyList != null )
                return _parentPropertyList.GetProperty( propertyName );
            if ( _builder != null )
            {
                try
                {
                    return _builder.MakeProperty( this, propertyName );
                }
                catch ( FonetException e )
                {
                    FonetDriver.ActiveDriver.FireFonetError(
                        "Exception in getFromParent(): property=" + propertyName + " : " + e );
                }
            }
            return null;
        }

        public string WmAbsToRel( int absdir )
        {
            if ( _wmtable != null )
                return SRelNames[ _wmtable[ absdir ] ];
            return string.Empty;
        }

        public string WmRelToAbs( int reldir )
        {
            if ( _wmtable != null )
            {
                for ( var i = 0; i < _wmtable.Length; i++ )
                {
                    if ( _wmtable[ i ] == reldir )
                        return SAbsNames[ i ];
                }
            }
            return string.Empty;
        }

        public void SetWritingMode( int writingMode )
        {
            _wmtable = (byte[])Wmtables[ writingMode ];
        }
    }
}