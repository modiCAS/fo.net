namespace Fonet.Fo.Pagination
{
    internal interface ISubSequenceSpecifier
    {
        string GetNextPageMaster( int currentPageNumber, bool thisIsFirstPage, bool isEmptyPage );

        void Reset();
    }
}