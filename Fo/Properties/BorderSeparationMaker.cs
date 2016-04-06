using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class BorderSeparationMaker : LengthPairProperty.Maker
    {
        private static readonly PropertyMaker SBlockProgressionDirectionMaker =
            new LengthProperty.Maker( "border-separation.block-progression-direction" );

        private static readonly PropertyMaker SInlineProgressionDirectionMaker =
            new LengthProperty.Maker( "border-separation.inline-progression-direction" );

        private Property _mDefaultProp;


        private readonly PropertyMaker _mShorthandMaker;

        protected BorderSeparationMaker( string name )
            : base( name )
        {
            _mShorthandMaker = GetSubpropMaker( "block-progression-direction" );
        }


        public new static PropertyMaker Maker( string propName )
        {
            return new BorderSeparationMaker( propName );
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
            if ( subprop.Equals( "block-progression-direction" ) )
                return SBlockProgressionDirectionMaker;

            if ( subprop.Equals( "inline-progression-direction" ) )
                return SInlineProgressionDirectionMaker;

            return base.GetSubpropMaker( subprop );
        }

        protected override Property SetSubprop( Property baseProp, string subpropName, Property subProp )
        {
            LengthPair val = baseProp.GetLengthPair();
            val.SetComponent( subpropName, subProp, false );
            return baseProp;
        }

        public override Property GetSubpropValue( Property baseProp, string subpropName )
        {
            LengthPair val = baseProp.GetLengthPair();
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
            var p = new LengthPair();
            Property subProp;

            subProp = GetSubpropMaker( "block-progression-direction" ).Make( pList,
                GetDefaultForBlockProgressionDirection(), fo );
            p.SetComponent( "block-progression-direction", subProp, true );

            subProp = GetSubpropMaker( "inline-progression-direction" ).Make( pList,
                GetDefaultForInlineProgressionDirection(), fo );
            p.SetComponent( "inline-progression-direction", subProp, true );

            return new LengthPairProperty( p );
        }


        protected virtual string GetDefaultForBlockProgressionDirection()
        {
            return "0pt";
        }

        protected virtual string GetDefaultForInlineProgressionDirection()
        {
            return "0pt";
        }

        public override Property ConvertProperty( Property p, PropertyList pList, FObj fo )
        {
            if ( p is LengthPairProperty )
                return p;
            if ( !( p is EnumProperty ) )
                p = _mShorthandMaker.ConvertProperty( p, pList, fo );
            if ( p != null )
            {
                Property prop = MakeCompound( pList, fo );
                LengthPair pval = prop.GetLengthPair();

                pval.SetComponent( "block-progression-direction", p, false );
                pval.SetComponent( "inline-progression-direction", p, false );
                return prop;
            }
            return null;
        }

        public override bool IsInherited()
        {
            return true;
        }
    }
}