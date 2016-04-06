using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class LeaderLengthMaker : LengthRangeProperty.Maker
    {
        private static readonly PropertyMaker SMinimumMaker =
            new SpMinimumMaker( "leader-length.minimum" );

        private static readonly PropertyMaker SOptimumMaker =
            new SpOptimumMaker( "leader-length.optimum" );

        private static readonly PropertyMaker SMaximumMaker =
            new SpMaximumMaker( "leader-length.maximum" );


        private readonly PropertyMaker _mShorthandMaker;

        protected LeaderLengthMaker( string name )
            : base( name )
        {
            _mShorthandMaker = GetSubpropMaker( "minimum" );
        }


        public new static PropertyMaker Maker( string propName )
        {
            return new LeaderLengthMaker( propName );
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
            return MakeCompound( propertyList, propertyList.GetParentFObj() );
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
            return "0pt";
        }

        protected virtual string GetDefaultForOptimum()
        {
            return "12.0pt";
        }

        protected virtual string GetDefaultForMaximum()
        {
            return "100%";
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
            return true;
        }

        public override IPercentBase GetPercentBase( FObj fo, PropertyList propertyList )
        {
            return new LengthBase( fo, propertyList, LengthBase.ContainingBox );
        }

        private class SpMinimumMaker : LengthProperty.Maker
        {
            protected internal SpMinimumMaker( string sPropName ) : base( sPropName )
            {
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

            public override IPercentBase GetPercentBase( FObj fo, PropertyList propertyList )
            {
                return new LengthBase( fo, propertyList, LengthBase.ContainingBox );
            }
        }
    }
}