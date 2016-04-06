using System.Collections;
using Fonet.DataTypes;

namespace Fonet.Fo.Expr
{
    internal class PropertyInfo
    {
        private readonly FObj _fo;
        private readonly PropertyMaker _maker;
        private readonly PropertyList _plist;
        private Stack _stkFunction;

        public PropertyInfo( PropertyMaker maker, PropertyList plist, FObj fo )
        {
            _maker = maker;
            _plist = plist;
            _fo = fo;
        }

        public bool InheritsSpecified()
        {
            return _maker.InheritsSpecified();
        }

        public IPercentBase GetPercentBase()
        {
            IPercentBase pcbase = GetFunctionPercentBase();
            return pcbase ?? _maker.GetPercentBase( _fo, _plist );
        }

        public int CurrentFontSize()
        {
            return _plist.GetProperty( "font-size" ).GetLength().MValue();
        }

        public FObj GetFo()
        {
            return _fo;
        }

        public PropertyList GetPropertyList()
        {
            return _plist;
        }

        public void PushFunction( IFunction func )
        {
            if ( _stkFunction == null )
                _stkFunction = new Stack();
            _stkFunction.Push( func );
        }

        public void PopFunction()
        {
            if ( _stkFunction != null )
                _stkFunction.Pop();
        }

        private IPercentBase GetFunctionPercentBase()
        {
            if ( _stkFunction != null )
            {
                var f = (IFunction)_stkFunction.Peek();
                if ( f != null )
                    return f.GetPercentBase();
            }
            return null;
        }
    }
}