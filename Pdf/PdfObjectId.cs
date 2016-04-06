namespace Fonet.Pdf
{
    public struct PdfObjectId
    {
        private readonly uint _objectNumber;

        private readonly ushort _generationNumber;

        public PdfObjectId( uint objectNumber )
        {
            this._objectNumber = objectNumber;
            _generationNumber = 0;
        }

        public PdfObjectId( uint objectNumber, ushort generationNumber )
        {
            this._objectNumber = objectNumber;
            this._generationNumber = generationNumber;
        }

        public uint ObjectNumber
        {
            get { return _objectNumber; }
        }

        public ushort GenerationNumber
        {
            get { return _generationNumber; }
        }

        // TODO: implement equals/hashcode etc.
    }
}