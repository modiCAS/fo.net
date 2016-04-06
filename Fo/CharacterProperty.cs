namespace Fonet.Fo
{
    internal class CharacterProperty : Property
    {
        private readonly char _character;

        public CharacterProperty( char character )
        {
            this._character = character;
        }

        public override object GetObject()
        {
            return _character;
        }

        public override char GetCharacter()
        {
            return _character;
        }

        public override string GetString()
        {
            return _character.ToString();
        }

        internal class Maker : PropertyMaker
        {
            public Maker( string propName ) : base( propName )
            {
            }

            public override Property Make(
                PropertyList propertyList, string value, FObj fo )
            {
                char c = value[ 0 ];
                return new CharacterProperty( c );
            }
        }
    }
}