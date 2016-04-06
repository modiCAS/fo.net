using System.Text;
using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class BlockProgressionDimensionMaker : LengthRangeProperty.Maker
    {
        private static readonly PropertyMaker s_MinimumMaker =
            new SP_MinimumMaker( "block-progression-dimension.minimum" );

        private static readonly PropertyMaker s_OptimumMaker =
            new SP_OptimumMaker( "block-progression-dimension.optimum" );

        private static readonly PropertyMaker s_MaximumMaker =
            new SP_MaximumMaker( "block-progression-dimension.maximum" );

        private Property m_defaultProp;

        private readonly PropertyMaker m_shorthandMaker;

        protected BlockProgressionDimensionMaker( string name )
            : base( name )
        {
            m_shorthandMaker = GetSubpropMaker( "minimum" );
        }


        public new static PropertyMaker Maker( string propName )
        {
            return new BlockProgressionDimensionMaker( propName );
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

            return base.GetSubpropMaker( subprop );
        }

        protected override Property SetSubprop( Property baseProp, string subpropName, Property subProp )
        {
            LengthRange val = baseProp.GetLengthRange();
            val.SetComponent( subpropName, subProp, false );
            return baseProp;
        }

        public override Property GetSubpropValue( Property baseProp, string subpropName )
        {
            LengthRange val = baseProp.GetLengthRange();
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
            var p = new LengthRange();
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

            return new LengthRangeProperty( p );
        }


        protected virtual string GetDefaultForMinimum()
        {
            return "auto";
        }

        protected virtual string GetDefaultForOptimum()
        {
            return "auto";
        }

        protected virtual string GetDefaultForMaximum()
        {
            return "auto";
        }

        public override Property ConvertProperty( Property p, PropertyList pList, FObj fo )
        {
            if ( p is LengthRangeProperty )
                return p;
            if ( !( p is EnumProperty ) )
                p = m_shorthandMaker.ConvertProperty( p, pList, fo );
            if ( p != null )
            {
                Property prop = MakeCompound( pList, fo );
                LengthRange pval = prop.GetLengthRange();

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

        public override bool IsCorrespondingForced( PropertyList propertyList )
        {
            var sbExpr = new StringBuilder();

            sbExpr.Length = 0;

            sbExpr.Append( propertyList.wmRelToAbs( PropertyList.BLOCKPROGDIM ) );

            if ( propertyList.GetExplicitProperty( sbExpr.ToString() ) != null )
                return true;

            sbExpr.Length = 0;
            sbExpr.Append( "min-" );
            sbExpr.Append( propertyList.wmRelToAbs( PropertyList.BLOCKPROGDIM ) );

            if ( propertyList.GetExplicitProperty( sbExpr.ToString() ) != null )
                return true;

            sbExpr.Length = 0;
            sbExpr.Append( "max-" );
            sbExpr.Append( propertyList.wmRelToAbs( PropertyList.BLOCKPROGDIM ) );

            if ( propertyList.GetExplicitProperty( sbExpr.ToString() ) != null )
                return true;

            return false;
        }

        public override Property Compute( PropertyList propertyList )
        {
            FObj parentFO = propertyList.getParentFObj();
            var sbExpr = new StringBuilder();
            Property p = null;

            sbExpr.Append( propertyList.wmRelToAbs( PropertyList.BLOCKPROGDIM ) );

            p = propertyList.GetExplicitOrShorthandProperty( sbExpr.ToString() );

            if ( p != null )
                p = ConvertProperty( p, propertyList, parentFO );

            else
                p = MakeCompound( propertyList, parentFO );

            Property subprop;

            sbExpr.Length = 0;
            sbExpr.Append( "min-" );
            sbExpr.Append( propertyList.wmRelToAbs( PropertyList.BLOCKPROGDIM ) );

            subprop = propertyList.GetExplicitOrShorthandProperty( sbExpr.ToString() );

            if ( subprop != null )
                SetSubprop( p, "minimum", subprop );

            sbExpr.Length = 0;
            sbExpr.Append( "max-" );
            sbExpr.Append( propertyList.wmRelToAbs( PropertyList.BLOCKPROGDIM ) );

            subprop = propertyList.GetExplicitOrShorthandProperty( sbExpr.ToString() );

            if ( subprop != null )
                SetSubprop( p, "maximum", subprop );

            return p;
        }

        private class SP_MinimumMaker : LengthProperty.Maker
        {
            protected internal SP_MinimumMaker( string sPropName ) : base( sPropName )
            {
            }

            protected override bool IsAutoLengthAllowed()
            {
                return true;
            }

            public override IPercentBase GetPercentBase( FObj fo, PropertyList propertyList )
            {
                return new LengthBase( fo, propertyList, LengthBase.CONTAINING_BOX );
            }
        }

        private class SP_OptimumMaker : LengthProperty.Maker
        {
            protected internal SP_OptimumMaker( string sPropName ) : base( sPropName )
            {
            }

            protected override bool IsAutoLengthAllowed()
            {
                return true;
            }

            public override IPercentBase GetPercentBase( FObj fo, PropertyList propertyList )
            {
                return new LengthBase( fo, propertyList, LengthBase.CONTAINING_BOX );
            }
        }

        private class SP_MaximumMaker : LengthProperty.Maker
        {
            protected internal SP_MaximumMaker( string sPropName ) : base( sPropName )
            {
            }

            protected override bool IsAutoLengthAllowed()
            {
                return true;
            }

            public override IPercentBase GetPercentBase( FObj fo, PropertyList propertyList )
            {
                return new LengthBase( fo, propertyList, LengthBase.CONTAINING_BOX );
            }
        }
    }
}