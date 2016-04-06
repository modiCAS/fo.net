using System.Collections;
using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class GenericCondBorderWidth : CondLengthProperty.Maker
    {
        private static readonly PropertyMaker SLengthMaker =
            new SpLengthMaker( "border-cond-width-template.length" );

        private static readonly PropertyMaker SConditionalityMaker =
            new SpConditionalityMaker( "border-cond-width-template.conditionality" );

        private static Hashtable _sHtKeywords;

        private Property _mDefaultProp;


        private readonly PropertyMaker _mShorthandMaker;

        protected GenericCondBorderWidth( string name )
            : base( name )
        {
            _mShorthandMaker = GetSubpropMaker( "length" );
        }


        public new static PropertyMaker Maker( string propName )
        {
            return new GenericCondBorderWidth( propName );
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
            if ( subprop.Equals( "length" ) )
                return SLengthMaker;

            if ( subprop.Equals( "conditionality" ) )
                return SConditionalityMaker;

            return base.GetSubpropMaker( subprop );
        }

        protected override Property SetSubprop( Property baseProp, string subpropName, Property subProp )
        {
            CondLength val = baseProp.GetCondLength();
            val.SetComponent( subpropName, subProp, false );
            return baseProp;
        }

        public override Property GetSubpropValue( Property baseProp, string subpropName )
        {
            CondLength val = baseProp.GetCondLength();
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
            var p = new CondLength();
            Property subProp;

            subProp = GetSubpropMaker( "length" ).Make( pList,
                GetDefaultForLength(), fo );
            p.SetComponent( "length", subProp, true );

            subProp = GetSubpropMaker( "conditionality" ).Make( pList,
                GetDefaultForConditionality(), fo );
            p.SetComponent( "conditionality", subProp, true );

            return new CondLengthProperty( p );
        }


        protected virtual string GetDefaultForLength()
        {
            return "medium";
        }

        protected virtual string GetDefaultForConditionality()
        {
            return "";
        }

        public override Property ConvertProperty( Property p, PropertyList pList, FObj fo )
        {
            if ( p is CondLengthProperty )
                return p;
            if ( !( p is EnumProperty ) )
                p = _mShorthandMaker.ConvertProperty( p, pList, fo );
            if ( p != null )
            {
                Property prop = MakeCompound( pList, fo );
                CondLength pval = prop.GetCondLength();

                pval.SetComponent( "length", p, false );
                return prop;
            }
            return null;
        }

        public override bool IsInherited()
        {
            return false;
        }

        private static void InitKeywords()
        {
            _sHtKeywords = new Hashtable( 3 );

            _sHtKeywords.Add( "thin", "0.5pt" );

            _sHtKeywords.Add( "medium", "1pt" );

            _sHtKeywords.Add( "thick", "2pt" );
        }

        protected override string CheckValueKeywords( string keyword )
        {
            if ( _sHtKeywords == null )
                InitKeywords();
            var value = (string)_sHtKeywords[ keyword ];
            if ( value == null )
                return base.CheckValueKeywords( keyword );
            return value;
        }

        internal class Enums
        {
            internal class Conditionality
            {
                public const int Discard = Constants.Discard;

                public const int Retain = Constants.Retain;
            }
        }

        private class SpLengthMaker : LengthProperty.Maker
        {
            private static Hashtable _sHtKeywords;

            protected internal SpLengthMaker( string sPropName ) : base( sPropName )
            {
            }

            private static void InitKeywords()
            {
                _sHtKeywords = new Hashtable( 3 );

                _sHtKeywords.Add( "thin", "0.5pt" );

                _sHtKeywords.Add( "medium", "1pt" );

                _sHtKeywords.Add( "thick", "2pt" );
            }

            protected override string CheckValueKeywords( string keyword )
            {
                if ( _sHtKeywords == null )
                    InitKeywords();
                var value = (string)_sHtKeywords[ keyword ];
                if ( value == null )
                    return base.CheckValueKeywords( keyword );
                return value;
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