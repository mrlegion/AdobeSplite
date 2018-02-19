
using System.ComponentModel;
using System.Timers;

namespace PDFSplitter.Model
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Prism.Mvvm;

    #endregion
    
    /// <summary>
    /// Business model for project PDFSplitter
    /// </summary>
    public class PdfSplitModel : BindableBase
    {
        private BackgroundWorker worker;

        private int progress = 20;

        /// <summary>
        /// Gets or sets progress work
        /// </summary>
        public int Progress
        {
            get => progress;
            set => SetProperty(ref progress, value);
        }
    }
}
