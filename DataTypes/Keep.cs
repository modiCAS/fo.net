using Fonet.Fo;

namespace Fonet.DataTypes
{
    internal class Keep : ICompoundDatatype
    {
        private Property _withinColumn;
        private Property _withinLine;

        private Property _withinPage;

        public void SetComponent( string sCmpnName, Property cmpnValue,
            bool bIsDefault )
        {
            if ( sCmpnName.Equals( "within-line" ) )
                SetWithinLine( cmpnValue, bIsDefault );
            else if ( sCmpnName.Equals( "within-column" ) )
                SetWithinColumn( cmpnValue, bIsDefault );
            else if ( sCmpnName.Equals( "within-page" ) )
                SetWithinPage( cmpnValue, bIsDefault );
        }

        public Property GetComponent( string sCmpnName )
        {
            if ( sCmpnName.Equals( "within-line" ) )
                return GetWithinLine();
            if ( sCmpnName.Equals( "within-column" ) )
                return GetWithinColumn();
            if ( sCmpnName.Equals( "within-page" ) )
                return GetWithinPage();
            return null;
        }

        public void SetWithinLine( Property withinLine, bool bIsDefault )
        {
            this._withinLine = withinLine;
        }

        protected void SetWithinColumn( Property withinColumn,
            bool bIsDefault )
        {
            this._withinColumn = withinColumn;
        }

        public void SetWithinPage( Property withinPage, bool bIsDefault )
        {
            this._withinPage = withinPage;
        }

        public Property GetWithinLine()
        {
            return _withinLine;
        }

        public Property GetWithinColumn()
        {
            return _withinColumn;
        }

        public Property GetWithinPage()
        {
            return _withinPage;
        }

        public override string ToString()
        {
            return "Keep";
        }
    }
}