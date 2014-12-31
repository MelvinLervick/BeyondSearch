namespace WebPageWidget
{
    public interface IWebWidget
    {
        void ExtractWidgetContent(string content);
        bool ReadWidgetFile( string fileName );
        bool WriteWidgetFile( string fileName );
    }
}