using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class GenericKeep : KeepProperty.Maker
    {
        private static readonly PropertyMaker SWithinPageMaker =
            new SpWithinPageMaker( "generic-keep.within-page" );

        private static readonly PropertyMaker SWithinLineMaker =
            new SpWithinLineMaker( "generic-keep.within-line" );

        private static readonly PropertyMaker SWithinColumnMaker =
            new SpWithinColumnMaker( "generic-keep.within-column" );

        private Property _mDefaultProp;


        private readonly PropertyMaker _mShorthandMaker;

        protected GenericKeep( string name )
            : base( name )
        {
            _mShorthandMaker = GetSubpropMaker( "within-page" );
        }


        public new static PropertyMaker Maker( string propName )
        {
            return new GenericKeep( propName );
        }

        public override Property CheckEnumValues( string value )
        {
            return _mShorthandMaker.CheckEnumValues( value );
        }

        protected override bool IsCompoundMaker()
        {
            return true;
        }

        protected override PropertyMaker GetSubpropMaker( string subprop )
        {
            if ( subprop.Equals( "within-page" ) )
                return SWithinPageMaker;

            if ( subprop.Equals( "within-line" ) )
                return SWithinLineMaker;

            if ( subprop.Equals( "within-column" ) )
                return SWithinColumnMaker;

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
            if ( _mDefaultProp == null )
                _mDefaultProp = MakeCompound( propertyList, propertyList.GetParentFObj() );
            return _mDefaultProp;
        }


        protected override Property MakeCompound( PropertyList pList, FObj fo )
        {
            var p = new Keep();
            Property subProp;

            subProp = GetSubpropMaker( "within-page" ).Make( pList,
                GetDefaultForWithinPage(), fo );
            p.SetComponent( "within-page", subProp, true );

            subProp = GetSubpropMaker( "within-line" ).Make( pList,
                GetDefaultForWithinLine(), fo );
            p.SetComponent( "within-line", subProp, true );

            subProp = GetSubpropMaker( "within-column" ).Make( pList,
                GetDefaultForWithinColumn(), fo );
            p.SetComponent( "within-column", subProp, true );

            return new KeepProperty( p );
        }

        protected virtual string GetDefaultForWithinPage()
        {
            return "auto";
        }

        protected virtual string GetDefaultForWithinLine()
        {
            return "auto";
        }

        protected virtual string GetDefaultForWithinColumn()
        {
            return "auto";
        }

        public override Property ConvertProperty( Property p, PropertyList pList, FObj fo )
        {
            if ( p is KeepProperty )
                return p;
            if ( !( p is EnumProperty ) )
                p = _mShorthandMaker.ConvertProperty( p, pList, fo );
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
                public const int Auto = Constants.Auto;

                public const int Always = Constants.Always;
            }

            internal class WithinLine
            {
                public const int Auto = Constants.Auto;

                public const int Always = Constants.Always;
            }

            internal class WithinColumn
            {
                public const int Auto = Constants.Auto;

                public const int Always = Constants.Always;
            }
        }

        private class SpWithinPageMaker : NumberProperty.Maker
        {
            protected internal static readonly EnumProperty SPropAuto = new EnumProperty( Enums.WithinPage.Auto );

            protected internal static readonly EnumProperty SPropAlways = new EnumProperty( Enums.WithinPage.Always );

            protected internal SpWithinPageMaker( string sPropName ) : base( sPropName )
            {
            }

            public override Property CheckEnumValues( string value )
            {
                if ( value.Equals( "auto" ) )
                    return SPropAuto;

                if ( value.Equals( "always" ) )
                    return SPropAlways;

                return base.CheckEnumValues( value );
            }
        }

        private class SpWithinLineMaker : NumberProperty.Maker
        {
            protected internal static readonly EnumProperty SPropAuto = new EnumProperty( Enums.WithinLine.Auto );

            protected internal static readonly EnumProperty SPropAlways = new EnumProperty( Enums.WithinLine.Always );

            protected internal SpWithinLineMaker( string sPropName ) : base( sPropName )
            {
            }

            public override Property CheckEnumValues( string value )
            {
                if ( value.Equals( "auto" ) )
                    return SPropAuto;

                if ( value.Equals( "always" ) )
                    return SPropAlways;

                return base.CheckEnumValues( value );
            }
        }

        private class SpWithinColumnMaker : NumberProperty.Maker
        {
            protected internal static readonly EnumProperty SPropAuto = new EnumProperty( Enums.WithinColumn.Auto );

            protected internal static readonly EnumProperty SPropAlways = new EnumProperty( Enums.WithinColumn.Always );

            protected internal SpWithinColumnMaker( string sPropName ) : base( sPropName )
            {
            }

            public override Property CheckEnumValues( string value )
            {
                if ( value.Equals( "auto" ) )
                    return SPropAuto;

                if ( value.Equals( "always" ) )
                    return SPropAlways;

                return base.CheckEnumValues( value );
            }
        }
    }
}