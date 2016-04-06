namespace Fonet.Pdf
{
    public class PdfInternalLink : IPdfAction
    {
        private readonly PdfObjectReference _goToReference;

        public PdfInternalLink( PdfObjectReference goToReference )
        {
            this._goToReference = goToReference;
        }

        public PdfObject GetAction()
        {
            return _goToReference;
        }
    }
}