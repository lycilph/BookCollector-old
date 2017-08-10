using MahApps.Metro.Controls.Dialogs;

namespace Core
{
    public class FileDialogResult
    {
        public MessageDialogResult Result { get; set; }
        public string Filename { get; set; }

        public FileDialogResult(MessageDialogResult result, string filename)
        {
            Result = result;
            Filename = filename;
        }
    }
}
