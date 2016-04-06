using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class GenericSpace : SpaceProperty.Maker
    {
        private static readonly PropertyMaker SMinimumMaker = new LengthProperty.Maker( "generic-space.minimum" );

        private static readonly PropertyMaker SOptimumMaker = new LengthProperty.Maker( "generic-space.optimum" );

        private static readonly PropertyMaker SMaximumMaker = new LengthProperty.Maker( "generic-space.maximum" );

        private static readonly PropertyMaker SPrecedenceMaker =
            new SpPrecedenceMaker( "generic-space.precedence" );

        private static readonly PropertyMaker SConditionalityMaker =
            new SpConditionalityMaker( "generic-space.conditionality" );

        private Property _mDefaultProp;


        private readonly PropertyMaker _mShorthandMaker;

        protected GenericSpace( string name )
            : base( name )
        {
            _mShorthandMaker = GetSubpropMaker( "minimum" );
        }


        public new static PropertyMaker Maker( string propName )
        {
            return new GenericSpace( propName );
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
            if ( subprop.Equals( "minimum" ) )
                return SMinimumMaker;

            if ( subprop.Equals( "optimum" ) )
                return SOptimumMaker;

            if ( subprop.Equals( "maximum" ) )
                return SMaximumMaker;

            if ( subprop.Equals( "precedence" ) )
                return SPrecedenceMaker;

            if ( subprop.Equals( "conditionality" ) )
                return SConditionalityMaker;

            return base.GetSubpropMaker( subprop );
        }

        protected override Property SetSubprop( Property baseProp, string subpropName, Property subProp )
        {
            Space val = baseProp.GetSpace();
            val.SetComponent( subpropName, subProp, false );
            return baseProp;
        }

        public override Property GetSubpropValue( Property baseProp, string subpropName )
        {
            Space val = baseProp.GetSpace();
            return val.GetComponent( subpropName );
        }

        public override Property Make( PropertyList propertyList )
        {
            return _mDefaultProp ?? ( _mDefaultProp = MakeCompound( propertyList, propertyList.GetParentFObj() ) );
        }


        protected override Property MakeCompound( PropertyList pList, FObj fo )
        {
            var p = new Space();

            Property subProp = GetSubpropMaker( "minimum" ).Make( pList,
                GetDefaultForMinimum(), fo );
            p.SetComponent( "minimum", subProp, true );

            subProp = GetSubpropMaker( "optimum" ).Make( pList,
                GetDefaultForOptimum(), fo );
            p.SetComponent( "optimum", subProp, true );

            subProp = GetSubpropMaker( "maximum" ).Make( pList,
                GetDefaultForMaximum(), fo );
            p.SetComponent( "maximum", subProp, true );

            subProp = GetSubpropMaker( "precedence" ).Make( pList,
                GetDefaultForPrecedence(), fo );
            p.SetComponent( "precedence", subProp, true );

            subProp = GetSubpropMaker( "conditionality" ).Make( pList,
                GetDefaultForConditionality(), fo );
            p.SetComponent( "conditionality", subProp, true );

            return new SpaceProperty( p );
        }

        protected virtual string GetDefaultForMinimum()
        {
            return "0pt";
        }

        protected virtual string GetDefaultForOptimum()
        {
            return "0pt";
        }

        protected virtual string GetDefaultForMaximum()
        {
            return "0pt";
        }

        protected virtual string GetDefaultForPrecedence()
        {
            return "0";
        }

        protected virtual string GetDefaultForConditionality()
        {
            return "discard";
        }

        public override Property ConvertProperty( Property p, PropertyList pList, FObj fo )
        {
            if ( p is SpaceProperty )
                return p;
            if ( !( p is EnumProperty ) )
                p = _mShorthandMaker.ConvertProperty( p, pList, fo );
            if ( p != null )
            {
                Property prop = MakeCompound( pList, fo );
                Space pval = prop.GetSpace();

                pval.SetComponent( "minimum", p, false );
                pval.SetComponent( "optimum", p, false );
                pval.SetComponent( "maximum", p, false );
                return prop;
            }
            return null;
        }

        public override bool IsInherited()
        {
            return false;
        }

        internal class Enums
        {
            internal class Precedence
            {
                public const int Force = Constants.Force;
            }

            internal class Conditionality
            {
                public const int Discard = Constants.Discard;

                public const int Retain = Constants.Retain;
            }
        }

        private class SpPrecedenceMaker : NumberProperty.Maker
        {
            protected internal static readonly EnumProperty SPropForce = new EnumProperty( Enums.Precedence.Force );

            protected internal SpPrecedenceMaker( string sPropName ) : base( sPropName )
            {
            }

            public override Property CheckEnumValues( string value )
            {
                if ( value.Equals( "force" ) )
                    return SPropForce;

                return base.CheckEnumValues( value );
            }
        }

        private class SpConditionalityMaker : EnumProperty.Maker
        {
            protected internal static readonly EnumProperty SPropDiscard =
                new EnumProperty( Enums.Conditionality.Discard );

            protected internal static readonly EnumProperty SPropRetain = new EnumProperty( Enums.Conditionality.Retain );

            protected internal SpConditionalityMaker( string sPropName ) : base( sPropName )
            {
            }

            public override Property CheckEnumValues( string value )
            {
                if ( value.Equals( "discard" ) )
                    return SPropDiscard;

                if ( value.Equals( "retain" ) )
                    return SPropRetain;

                return base.CheckEnumValues( value );
            }
        }
    }
}