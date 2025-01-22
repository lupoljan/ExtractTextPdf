using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExtractTextPdf.Models;
using Microsoft.Win32;
using Syncfusion.Pdf.Interactive;
using Syncfusion.Windows.PdfViewer;
using System.Windows;

namespace ExtractTextPdf.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private List<string> _pdfFiles;
        public List<string> PdfFiles
        {
            get => _pdfFiles;
            set
            {
                SetProperty(ref _pdfFiles, value);
                OnPropertyChanged(nameof(HasPdfFiles));
            }
        }

        private string _selectedPdfFile;
        public string SelectedPdfFile
        {
            get => _selectedPdfFile;
            set
            {
                SetProperty(ref _selectedPdfFile, value);

                if (!string.IsNullOrEmpty(value))
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        var pdfViewer = Application.Current.MainWindow.FindName("PdfViewer") as PdfViewerControl;
                        pdfViewer?.Load(value);
                    });
                }
            }
        }
        public bool HasPdfFiles => PdfFiles != null && PdfFiles.Any();

        public IRelayCommand LoadPdfCommand { get; }
        public IRelayCommand ClearPdfListCommand { get; }

        public MainViewModel()
        {
            PdfFiles = new List<string>();

            LoadPdfCommand = new RelayCommand(LoadPdf);
            ClearPdfListCommand = new RelayCommand(ClearPdfList);
        }

        private void LoadPdf()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "PDF files (*.pdf)|*.pdf"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                PdfFiles = openFileDialog.FileNames.ToList();
                MessageBox.Show($"Loaded {PdfFiles.Count} PDF files.");
            }
        }
        private void ClearPdfList()
        {
            PdfFiles.Clear();
            PdfFiles = new List<string>();
        }

    }
}
