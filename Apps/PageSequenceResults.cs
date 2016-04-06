namespace Fonet.Apps
{
    internal class PageSequenceResults
    {
        private readonly string _id;
        private readonly int _pageCount;

        internal PageSequenceResults( string id, int pageCount )
        {
            this._id = id;
            this._pageCount = pageCount;
        }

        internal string GetID()
        {
            return _id;
        }

        internal int GetPageCount()
        {
            return _pageCount;
        }
    }
}