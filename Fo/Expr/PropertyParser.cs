using System.Collections;
using System.Globalization;
using Fonet.DataTypes;

namespace Fonet.Fo.Expr
{
    internal class PropertyParser : PropertyTokenizer
    {
        private const string Relunit = "em";
        private static readonly Numeric NegOne = new Numeric( (decimal)-1.0 );
        private static readonly Hashtable FunctionTable = new Hashtable();
        private readonly PropertyInfo _propInfo;

        static PropertyParser()
        {
            FunctionTable.Add( "ceiling", new CeilingFunction() );
            FunctionTable.Add( "floor", new FloorFunction() );
            FunctionTable.Add( "round", new RoundFunction() );
            FunctionTable.Add( "min", new MinFunction() );
            FunctionTable.Add( "max", new MaxFunction() );
            FunctionTable.Add( "abs", new AbsFunction() );
            FunctionTable.Add( "rgb", new RgbColorFunction() );
            FunctionTable.Add( "from-table-column", new FromTableColumnFunction() );
            FunctionTable.Add( "inherited-property-value",
                new InheritedPropFunction() );
            FunctionTable.Add( "from-parent", new FromParentFunction() );
            FunctionTable.Add( "from-nearest-specified-value",
                new NearestSpecPropFunction() );
            FunctionTable.Add( "proportional-column-width",
                new PpColWidthFunction() );
            FunctionTable.Add( "label-end", new LabelEndFunction() );
            FunctionTable.Add( "body-start", new BodyStartFunction() );
            FunctionTable.Add( "_fop-property-value", new FonetPropValFunction() );
        }

        private PropertyParser( string propExpr, PropertyInfo pInfo )
            : base( propExpr )
        {
            _propInfo = pInfo;
        }

        public static Property Parse( string expr, PropertyInfo propInfo )
        {
            return new PropertyParser( expr, propInfo ).ParseProperty();
        }

        private Property ParseProperty()
        {
            Next();
            if ( CurrentToken == TokEof )
                return new StringProperty( "" );
            ListProperty propList = null;
            while ( true )
            {
                Property prop = ParseAdditiveExpr();
                if ( CurrentToken == TokEof )
                {
                    if ( propList != null )
                    {
                        propList.AddProperty( prop );
                        return propList;
                    }
                    return prop;
                }
                if ( propList == null )
                    propList = new ListProperty( prop );
                else
                    propList.AddProperty( prop );
            }
        }

        private Property ParseAdditiveExpr()
        {
            Property prop = ParseMultiplicativeExpr();
            var cont = true;
            while ( cont )
            {
                switch ( CurrentToken )
                {
                case TokPlus:
                    Next();
                    prop = EvalAddition( prop.GetNumeric(),
                        ParseMultiplicativeExpr().GetNumeric() );
                    break;
                case TokMinus:
                    Next();
                    prop =
                        EvalSubtraction( prop.GetNumeric(),
                            ParseMultiplicativeExpr().GetNumeric() );
                    break;
                default:
                    cont = false;
                    break;
                }
            }
            return prop;
        }

        private Property ParseMultiplicativeExpr()
        {
            Property prop = ParseUnaryExpr();
            var cont = true;
            while ( cont )
            {
                switch ( CurrentToken )
                {
                case TokDiv:
                    Next();
                    prop = EvalDivide( prop.GetNumeric(),
                        ParseUnaryExpr().GetNumeric() );
                    break;
                case TokMod:
                    Next();
                    prop = EvalModulo( prop.GetNumber(),
                        ParseUnaryExpr().GetNumber() );
                    break;
                case TokMultiply:
                    Next();
                    prop = EvalMultiply( prop.GetNumeric(),
                        ParseUnaryExpr().GetNumeric() );
                    break;
                default:
                    cont = false;
                    break;
                }
            }
            return prop;
        }

        private Property ParseUnaryExpr()
        {
            if ( CurrentToken == TokMinus )
            {
                Next();
                return EvalNegate( ParseUnaryExpr().GetNumeric() );
            }
            return ParsePrimaryExpr();
        }

        private void ExpectRpar()
        {
            if ( CurrentToken != TokRpar )
                throw new PropertyException( "expected )" );
            Next();
        }

        private Property ParsePrimaryExpr()
        {
            Property prop;
            switch ( CurrentToken )
            {
            case TokLpar:
                Next();
                prop = ParseAdditiveExpr();
                ExpectRpar();
                return prop;

            case TokLiteral:
                prop = new StringProperty( CurrentTokenValue );
                break;

            case TokNcname:
                prop = new NCnameProperty( CurrentTokenValue );
                break;

            case TokFloat:
                prop = new NumberProperty( ParseDouble( CurrentTokenValue ) );
                break;

            case TokInteger:
                prop = new NumberProperty( int.Parse( CurrentTokenValue ) );
                break;

            case TokPercent:
                double pcval = ParseDouble(
                    CurrentTokenValue.Substring( 0, CurrentTokenValue.Length - 1 ) ) / 100.0;
                IPercentBase pcBase = _propInfo.GetPercentBase();
                if ( pcBase != null )
                {
                    if ( pcBase.GetDimension() == 0 )
                        prop = new NumberProperty( pcval * pcBase.GetBaseValue() );
                    else if ( pcBase.GetDimension() == 1 )
                    {
                        prop = new LengthProperty( new PercentLength( pcval,
                            pcBase ) );
                    }
                    else
                        throw new PropertyException( "Illegal percent dimension value" );
                }
                else
                    prop = new NumberProperty( pcval );
                break;

            case TokNumeric:
                int numLen = CurrentTokenValue.Length - CurrentUnitLength;
                string unitPart = CurrentTokenValue.Substring( numLen );
                double numPart = ParseDouble( CurrentTokenValue.Substring( 0, numLen ) );
                Length length = null;
                if ( unitPart.Equals( Relunit ) )
                    length = new FixedLength( numPart, _propInfo.CurrentFontSize() );
                else
                    length = new FixedLength( numPart, unitPart );
                if ( length == null )
                    throw new PropertyException( "unrecognized unit name: " + CurrentTokenValue );
                prop = new LengthProperty( length );
                break;

            case TokColorspec:
                prop = new ColorTypeProperty( new ColorType( CurrentTokenValue ) );
                break;

            case TokFunctionLpar:
                {
                var function =
                    (IFunction)FunctionTable[ CurrentTokenValue ];
                if ( function == null )
                {
                    throw new PropertyException( "no such function: "
                        + CurrentTokenValue );
                }
                Next();
                _propInfo.PushFunction( function );
                prop = function.Eval( ParseArgs( function.NumArgs ), _propInfo );
                _propInfo.PopFunction();
                return prop;
                }
            default:
                throw new PropertyException( "syntax error" );
            }
            Next();
            return prop;
        }

        private Property[] ParseArgs( int nbArgs )
        {
            var args = new Property[ nbArgs ];
            Property prop;
            var i = 0;
            if ( CurrentToken == TokRpar )
                Next();
            else
            {
                while ( true )
                {
                    prop = ParseAdditiveExpr();
                    if ( i < nbArgs )
                        args[ i++ ] = prop;
                    if ( CurrentToken != TokComma )
                        break;
                    Next();
                }
                ExpectRpar();
            }
            if ( nbArgs != i )
                throw new PropertyException( "Wrong number of args for function" );
            return args;
        }

        private Property EvalAddition( Numeric op1, Numeric op2 )
        {
            if ( op1 == null || op2 == null )
                throw new PropertyException( "Non numeric operand in addition" );
            return new NumericProperty( op1.Add( op2 ) );
        }

        private Property EvalSubtraction( Numeric op1, Numeric op2 )
        {
            if ( op1 == null || op2 == null )
                throw new PropertyException( "Non numeric operand in subtraction" );
            return new NumericProperty( op1.Subtract( op2 ) );
        }

        private Property EvalNegate( Numeric op )
        {
            if ( op == null )
                throw new PropertyException( "Non numeric operand to unary minus" );
            return new NumericProperty( op.Multiply( NegOne ) );
        }

        private Property EvalMultiply( Numeric op1, Numeric op2 )
        {
            if ( op1 == null || op2 == null )
                throw new PropertyException( "Non numeric operand in multiplication" );
            return new NumericProperty( op1.Multiply( op2 ) );
        }

        private Property EvalDivide( Numeric op1, Numeric op2 )
        {
            if ( op1 == null || op2 == null )
                throw new PropertyException( "Non numeric operand in division" );
            return new NumericProperty( op1.Divide( op2 ) );
        }

        private Property EvalModulo( Number op1, Number op2 )
        {
            if ( op1 == null || op2 == null )
                throw new PropertyException( "Non number operand to modulo" );
            return new NumberProperty( op1.DoubleValue() % op2.DoubleValue() );
        }

        private double ParseDouble( string s )
        {
            return double.Parse( s, CultureInfo.InvariantCulture.NumberFormat );
        }
    }
}