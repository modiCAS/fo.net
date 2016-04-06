using Fonet.Fo;

namespace Fonet.DataTypes
{
    internal class Keep : ICompoundDatatype
    {
        private Property withinColumn;
        private Property withinLine;

        private Property withinPage;

        public void SetComponent( string sCmpnName, Property cmpnValue,
            bool bIsDefault )
        {
            if ( sCmpnName.Equals( "within-line" ) )
                setWithinLine( cmpnValue, bIsDefault );
            else if ( sCmpnName.Equals( "within-column" ) )
                setWithinColumn( cmpnValue, bIsDefault );
            else if ( sCmpnName.Equals( "within-page" ) )
                setWithinPage( cmpnValue, bIsDefault );
        }

        public Property GetComponent( string sCmpnName )
        {
            if ( sCmpnName.Equals( "within-line" ) )
                return getWithinLine();
            if ( sCmpnName.Equals( "within-column" ) )
                return getWithinColumn();
            if ( sCmpnName.Equals( "within-page" ) )
                return getWithinPage();
            return null;
        }

        public void setWithinLine( Property withinLine, bool bIsDefault )
        {
            this.withinLine = withinLine;
        }

        protected void setWithinColumn( Property withinColumn,
            bool bIsDefault )
        {
            this.withinColumn = withinColumn;
        }

        public void setWithinPage( Property withinPage, bool bIsDefault )
        {
            this.withinPage = withinPage;
        }

        public Property getWithinLine()
        {
            return withinLine;
        }

        public Property getWithinColumn()
        {
            return withinColumn;
        }

        public Property getWithinPage()
        {
            return withinPage;
        }

        public override string ToString()
        {
            return "Keep";
        }
    }
}