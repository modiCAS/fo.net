namespace Fonet.Pdf
{
    public struct PdfObjectId
    {
        private readonly uint objectNumber;

        private readonly ushort generationNumber;

        public PdfObjectId( uint objectNumber )
        {
            this.objectNumber = objectNumber;
            generationNumber = 0;
        }

        public PdfObjectId( uint objectNumber, ushort generationNumber )
        {
            this.objectNumber = objectNumber;
            this.generationNumber = generationNumber;
        }

        public uint ObjectNumber
        {
            get { return objectNumber; }
        }

        public ushort GenerationNumber
        {
            get { return generationNumber; }
        }

        // TODO: implement equals/hashcode etc.
    }
}