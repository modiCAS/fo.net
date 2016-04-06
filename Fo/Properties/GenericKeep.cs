using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class GenericKeep : KeepProperty.Maker
    {
        private static readonly PropertyMaker s_WithinPageMaker =
            new SP_WithinPageMaker( "generic-keep.within-page" );

        private static readonly PropertyMaker s_WithinLineMaker =
            new SP_WithinLineMaker( "generic-keep.within-line" );

        private static readonly PropertyMaker s_WithinColumnMaker =
            new SP_WithinColumnMaker( "generic-keep.within-column" );

        private Property m_defaultProp;


        private readonly PropertyMaker m_shorthandMaker;

        protected GenericKeep( string name )
            : base( name )
        {
            m_shorthandMaker = GetSubpropMaker( "within-page" );
        }


        public new static PropertyMaker Maker( string propName )
        {
            return new GenericKeep( propName );
        }

        public override Property CheckEnumValues( string value )
        {
            return m_shorthandMaker.CheckEnumValues( value );
        }

        protected override bool IsCompoundMaker()
        {
            return true;
        }

        protected override PropertyMaker GetSubpropMaker( string subprop )
        {
            if ( subprop.Equals( "within-page" ) )
                return s_WithinPageMaker;

            if ( subprop.Equals( "within-line" ) )
                return s_WithinLineMaker;

            if ( subprop.Equals( "within-column" ) )
                return s_WithinColumnMaker;

            return base.GetSubpropMaker( subprop );
        }

        protected override Property SetSubprop( Property baseProp, string subpropName, Property subProp )
        {
            Keep val = baseProp.GetKeep();
            val.SetComponent( subpropName, subProp, false );
            return baseProp;
        }

        public override Property GetSubpropValue( Property baseProp, string subpropName )
        {
            Keep val = baseProp.GetKeep();
            return val.GetComponent( subpropName );
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( m_defaultProp == null )
                m_defaultProp = MakeCompound( propertyList, propertyList.getParentFObj() );
            return m_defaultProp;
        }


        protected override Property MakeCompound( PropertyList pList, FObj fo )
        {
            var p = new Keep();
            Property subProp;

            subProp = GetSubpropMaker( "within-page" ).Make( pList,
                getDefaultForWithinPage(), fo );
            p.SetComponent( "within-page", subProp, true );

            subProp = GetSubpropMaker( "within-line" ).Make( pList,
                getDefaultForWithinLine(), fo );
            p.SetComponent( "within-line", subProp, true );

            subProp = GetSubpropMaker( "within-column" ).Make( pList,
                getDefaultForWithinColumn(), fo );
            p.SetComponent( "within-column", subProp, true );

            return new KeepProperty( p );
        }

        protected virtual string getDefaultForWithinPage()
        {
            return "auto";
        }

        protected virtual string getDefaultForWithinLine()
        {
            return "auto";
        }

        protected virtual string getDefaultForWithinColumn()
        {
            return "auto";
        }

        public override Property ConvertProperty( Property p, PropertyList pList, FObj fo )
        {
            if ( p is KeepProperty )
                return p;
            if ( !( p is EnumProperty ) )
                p = m_shorthandMaker.ConvertProperty( p, pList, fo );
            if ( p != null )
            {
                Property prop = MakeCompound( pList, fo );
                Keep pval = prop.GetKeep();

                pval.SetComponent( "within-page", p, false );
                pval.SetComponent( "within-line", p, false );
                pval.SetComponent( "within-column", p, false );
                return prop;
            }
            return null;
        }

        internal class Enums
        {
            internal class WithinPage
            {
                public const int AUTO = Constants.AUTO;

                public const int ALWAYS = Constants.ALWAYS;
            }

            internal class WithinLine
            {
                public const int AUTO = Constants.AUTO;

                public const int ALWAYS = Constants.ALWAYS;
            }

            internal class WithinColumn
            {
                public const int AUTO = Constants.AUTO;

                public const int ALWAYS = Constants.ALWAYS;
            }
        }

        private class SP_WithinPageMaker : NumberProperty.Maker
        {
            protected internal static readonly EnumProperty s_propAUTO = new EnumProperty( Enums.WithinPage.AUTO );

            protected internal static readonly EnumProperty s_propALWAYS = new EnumProperty( Enums.WithinPage.ALWAYS );

            protected internal SP_WithinPageMaker( string sPropName ) : base( sPropName )
            {
            }

            public override Property CheckEnumValues( string value )
            {
                if ( value.Equals( "auto" ) )
                    return s_propAUTO;

                if ( value.Equals( "always" ) )
                    return s_propALWAYS;

                return base.CheckEnumValues( value );
            }
        }

        private class SP_WithinLineMaker : NumberProperty.Maker
        {
            protected internal static readonly EnumProperty s_propAUTO = new EnumProperty( Enums.WithinLine.AUTO );

            protected internal static readonly EnumProperty s_propALWAYS = new EnumProperty( Enums.WithinLine.ALWAYS );

            protected internal SP_WithinLineMaker( string sPropName ) : base( sPropName )
            {
            }

            public override Property CheckEnumValues( string value )
            {
                if ( value.Equals( "auto" ) )
                    return s_propAUTO;

                if ( value.Equals( "always" ) )
                    return s_propALWAYS;

                return base.CheckEnumValues( value );
            }
        }

        private class SP_WithinColumnMaker : NumberProperty.Maker
        {
            protected internal static readonly EnumProperty s_propAUTO = new EnumProperty( Enums.WithinColumn.AUTO );

            protected internal static readonly EnumProperty s_propALWAYS = new EnumProperty( Enums.WithinColumn.ALWAYS );

            protected internal SP_WithinColumnMaker( string sPropName ) : base( sPropName )
            {
            }

            public override Property CheckEnumValues( string value )
            {
                if ( value.Equals( "auto" ) )
                    return s_propAUTO;

                if ( value.Equals( "always" ) )
                    return s_propALWAYS;

                return base.CheckEnumValues( value );
            }
        }
    }
}