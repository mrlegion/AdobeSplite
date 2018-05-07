using PDFSplitter.Model;
using Prism.Mvvm;

namespace PDFSplitter.ViewModels
{
    public class ProgressWindowViewModel : BindableBase
    {
        private readonly PdfSplitModel _model;

        public int Progress => _model.Progress;

        public bool InBusy => _model.InProcessing;

        public ProgressWindowViewModel()
        {
            _model = new PdfSplitModel();
            _model.PropertyChanged += (sender, args) => { RaisePropertyChanged(args.PropertyName); };
        }

        public void Start(string file)
        {
            _model.StartProccess(file);
        }
    }
}
