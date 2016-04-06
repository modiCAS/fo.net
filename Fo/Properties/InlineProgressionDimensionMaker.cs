using System.Text;
using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class InlineProgressionDimensionMaker : LengthRangeProperty.Maker
    {
        private static readonly PropertyMaker SMinimumMaker =
            new SpMinimumMaker( "inline-progression-dimension.minimum" );

        private static readonly PropertyMaker SOptimumMaker =
            new SpOptimumMaker( "inline-progression-dimension.optimum" );

        private static readonly PropertyMaker SMaximumMaker =
            new SpMaximumMaker( "inline-progression-dimension.maximum" );

        private Property _mDefaultProp;


        private readonly PropertyMaker _mShorthandMaker;

        protected InlineProgressionDimensionMaker( string name )
            : base( name )
        {
            _mShorthandMaker = GetSubpropMaker( "minimum" );
        }


        public new static PropertyMaker Maker( string propName )
        {
            return new InlineProgressionDimensionMaker( propName );
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
            if ( _mDefaultProp == null )
                _mDefaultProp = MakeCompound( propertyList, propertyList.GetParentFObj() );
            return _mDefaultProp;
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
                p = _mShorthandMaker.ConvertProperty( p, pList, fo );
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

            sbExpr.Append( propertyList.WmRelToAbs( PropertyList.Inlineprogdim ) );

            if ( propertyList.GetExplicitProperty( sbExpr.ToString() ) != null )
                return true;

            sbExpr.Length = 0;
            sbExpr.Append( "min-" );
            sbExpr.Append( propertyList.WmRelToAbs( PropertyList.Inlineprogdim ) );

            if ( propertyList.GetExplicitProperty( sbExpr.ToString() ) != null )
                return true;

            sbExpr.Length = 0;
            sbExpr.Append( "max-" );
            sbExpr.Append( propertyList.WmRelToAbs( PropertyList.Inlineprogdim ) );

            if ( propertyList.GetExplicitProperty( sbExpr.ToString() ) != null )
                return true;

            return false;
        }


        public override Property Compute( PropertyList propertyList )
        {
            FObj parentFo = propertyList.GetParentFObj();
            var sbExpr = new StringBuilder();
            Property p = null;

            sbExpr.Append( propertyList.WmRelToAbs( PropertyList.Inlineprogdim ) );

            p = propertyList.GetExplicitOrShorthandProperty( sbExpr.ToString() );

            if ( p != null )
                p = ConvertProperty( p, propertyList, parentFo );

            else
                p = MakeCompound( propertyList, parentFo );

            Property subprop;

            sbExpr.Length = 0;
            sbExpr.Append( "min-" );
            sbExpr.Append( propertyList.WmRelToAbs( PropertyList.Inlineprogdim ) );

            subprop = propertyList.GetExplicitOrShorthandProperty( sbExpr.ToString() );

            if ( subprop != null )
                SetSubprop( p, "minimum", subprop );

            sbExpr.Length = 0;
            sbExpr.Append( "max-" );
            sbExpr.Append( propertyList.WmRelToAbs( PropertyList.Inlineprogdim ) );

            subprop = propertyList.GetExplicitOrShorthandProperty( sbExpr.ToString() );

            if ( subprop != null )
                SetSubprop( p, "maximum", subprop );

            return p;
        }

        private class SpMinimumMaker : LengthProperty.Maker
        {
            protected internal SpMinimumMaker( string sPropName ) : base( sPropName )
            {
            }

            protected override bool IsAutoLengthAllowed()
            {
                return true;
            }

            public override IPercentBase GetPercentBase( FObj fo, PropertyList propertyList )
            {
                return new LengthBase( fo, propertyList, LengthBase.ContainingBox );
            }
        }

        private class SpOptimumMaker : LengthProperty.Maker
        {
            protected internal SpOptimumMaker( string sPropName ) : base( sPropName )
            {
            }

            protected override bool IsAutoLengthAllowed()
            {
                return true;
            }

            public override IPercentBase GetPercentBase( FObj fo, PropertyList propertyList )
            {
                return new LengthBase( fo, propertyList, LengthBase.ContainingBox );
            }
        }

        private class SpMaximumMaker : LengthProperty.Maker
        {
            protected internal SpMaximumMaker( string sPropName ) : base( sPropName )
            {
            }

            protected override bool IsAutoLengthAllowed()
            {
                return true;
            }

            public override IPercentBase GetPercentBase( FObj fo, PropertyList propertyList )
            {
                return new LengthBase( fo, propertyList, LengthBase.ContainingBox );
            }
        }
    }
}