using Fonet.DataTypes;

namespace Fonet.Fo.Expr
{
    internal class RgbColorFunction : FunctionBase
    {
        public override int NumArgs
        {
            get { return 3; }
        }

        public override IPercentBase GetPercentBase()
        {
            return new RgbPercentBase();
        }

        public override Property Eval( Property[] args, PropertyInfo pInfo )
        {
            var cfvals = new float[ 3 ];
            for ( var i = 0; i < 3; i++ )
            {
                Number cval = args[ i ].GetNumber();
                if ( cval == null )
                    throw new PropertyException( "Argument to rgb() must be a Number" );
                float colorVal = cval.FloatValue() / 255f;
                if ( colorVal < 0.0 || colorVal > 255.0 )
                {
                    FonetDriver.ActiveDriver.FireFonetWarning(
                        string.Format( "Normalising colour value {0} to 0", cval.FloatValue() ) );

                    colorVal = 0.0f;
                }
                cfvals[ i ] = colorVal;
            }
            return new ColorTypeProperty( new ColorType( cfvals[ 0 ], cfvals[ 1 ],
                cfvals[ 2 ] ) );
        }

        internal class RgbPercentBase : IPercentBase
        {
            public int GetDimension()
            {
                return 0;
            }

            public double GetBaseValue()
            {
                return 255f;
            }

            public int GetBaseLength()
            {
                return 0;
            }
        }
    }
}