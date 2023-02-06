using ReactiveUI;
using System.Windows.Controls;

namespace Gratti.App.Marking.Views.Models
{
    public class ContentViewModel : LogViewModel
    {
        private ContentControl content;
        public ContentControl Content
        {
            get { return content; }
            set { this.RaiseAndSetIfChanged(ref content, value); }
        }

    }
}
