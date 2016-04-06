using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class GenericSpace : SpaceProperty.Maker
    {
        private static readonly PropertyMaker s_MinimumMaker = new LengthProperty.Maker( "generic-space.minimum" );

        private static readonly PropertyMaker s_OptimumMaker = new LengthProperty.Maker( "generic-space.optimum" );

        private static readonly PropertyMaker s_MaximumMaker = new LengthProperty.Maker( "generic-space.maximum" );

        private static readonly PropertyMaker s_PrecedenceMaker =
            new SP_PrecedenceMaker( "generic-space.precedence" );

        private static readonly PropertyMaker s_ConditionalityMaker =
            new SP_ConditionalityMaker( "generic-space.conditionality" );

        private Property m_defaultProp;


        private readonly PropertyMaker m_shorthandMaker;

        protected GenericSpace( string name )
            : base( name )
        {
            m_shorthandMaker = GetSubpropMaker( "minimum" );
        }


        public new static PropertyMaker Maker( string propName )
        {
            return new GenericSpace( propName );
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
            if ( subprop.Equals( "minimum" ) )
                return s_MinimumMaker;

            if ( subprop.Equals( "optimum" ) )
                return s_OptimumMaker;

            if ( subprop.Equals( "maximum" ) )
                return s_MaximumMaker;

            if ( subprop.Equals( "precedence" ) )
                return s_PrecedenceMaker;

            if ( subprop.Equals( "conditionality" ) )
                return s_ConditionalityMaker;

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
            if ( m_defaultProp == null )
                m_defaultProp = MakeCompound( propertyList, propertyList.getParentFObj() );
            return m_defaultProp;
        }


        protected override Property MakeCompound( PropertyList pList, FObj fo )
        {
            var p = new Space();
            Property subProp;

            subProp = GetSubpropMaker( "minimum" ).Make( pList,
                GetDefaultForMinimum(), fo );
            p.SetComponent( "minimum", subProp, true );

            subProp = GetSubpropMaker( "optimum" ).Make( pList,
                GetDefaultForOptimum(), fo );
            p.SetComponent( "optimum", subProp, true );

            subProp = GetSubpropMaker( "maximum" ).Make( pList,
                GetDefaultForMaximum(), fo );
            p.SetComponent( "maximum", subProp, true );

            subProp = GetSubpropMaker( "precedence" ).Make( pList,
                getDefaultForPrecedence(), fo );
            p.SetComponent( "precedence", subProp, true );

            subProp = GetSubpropMaker( "conditionality" ).Make( pList,
                getDefaultForConditionality(), fo );
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

        protected virtual string getDefaultForPrecedence()
        {
            return "0";
        }

        protected virtual string getDefaultForConditionality()
        {
            return "discard";
        }

        public override Property ConvertProperty( Property p, PropertyList pList, FObj fo )
        {
            if ( p is SpaceProperty )
                return p;
            if ( !( p is EnumProperty ) )
                p = m_shorthandMaker.ConvertProperty( p, pList, fo );
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
                public const int FORCE = Constants.FORCE;
            }

            internal class Conditionality
            {
                public const int DISCARD = Constants.DISCARD;

                public const int RETAIN = Constants.RETAIN;
            }
        }

        private class SP_PrecedenceMaker : NumberProperty.Maker
        {
            protected internal static readonly EnumProperty s_propFORCE = new EnumProperty( Enums.Precedence.FORCE );

            protected internal SP_PrecedenceMaker( string sPropName ) : base( sPropName )
            {
            }

            public override Property CheckEnumValues( string value )
            {
                if ( value.Equals( "force" ) )
                    return s_propFORCE;

                return base.CheckEnumValues( value );
            }
        }

        private class SP_ConditionalityMaker : EnumProperty.Maker
        {
            protected internal static readonly EnumProperty s_propDISCARD =
                new EnumProperty( Enums.Conditionality.DISCARD );

            protected internal static readonly EnumProperty s_propRETAIN = new EnumProperty( Enums.Conditionality.RETAIN );

            protected internal SP_ConditionalityMaker( string sPropName ) : base( sPropName )
            {
            }

            public override Property CheckEnumValues( string value )
            {
                if ( value.Equals( "discard" ) )
                    return s_propDISCARD;

                if ( value.Equals( "retain" ) )
                    return s_propRETAIN;

                return base.CheckEnumValues( value );
            }
        }
    }
}