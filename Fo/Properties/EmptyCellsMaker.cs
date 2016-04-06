using Fonet.DataTypes;

namespace Fonet.Fo.Properties
{
    internal class EmptyCellsMaker : ToBeImplementedProperty.Maker
    {
        private Property _mDefaultProp;

        protected EmptyCellsMaker( string name ) : base( name )
        {
        }

        public new static PropertyMaker Maker( string propName )
        {
            return new EmptyCellsMaker( propName );
        }


        public override bool IsInherited()
        {
            return true;
        }

        public override Property Make( PropertyList propertyList )
        {
            if ( _mDefaultProp == null )
                _mDefaultProp = Make( propertyList, "show", propertyList.GetParentFObj() );
            return _mDefaultProp;
        }
    }
}