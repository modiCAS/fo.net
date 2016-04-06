namespace Fonet.Fo
{
    internal struct Status
    {
        private readonly int _code;

        public const int Ok = 1;
        public const int AreaFullNone = 2;
        public const int AreaFullSome = 3;
        public const int ForcePageBreak = 4;
        public const int ForcePageBreakEven = 5;
        public const int ForcePageBreakOdd = 6;
        public const int ForceColumnBreak = 7;
        public const int KeepWithNext = 8;

        public Status( int code )
        {
            this._code = code;
        }

        public int GetCode()
        {
            return _code;
        }

        public bool IsIncomplete()
        {
            return _code != Ok && _code != KeepWithNext;
        }

        public bool LaidOutNone()
        {
            return _code == AreaFullNone;
        }

        public bool IsPageBreak()
        {
            return _code == ForcePageBreak
                || _code == ForcePageBreakEven
                || _code == ForcePageBreakOdd;
        }
    }
}