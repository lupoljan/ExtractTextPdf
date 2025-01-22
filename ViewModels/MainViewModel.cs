using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CsvHelper.Configuration;
using CsvHelper;
using ExtractTextPdf.Models;
using Microsoft.Win32;
using Syncfusion.Pdf.Interactive;
using Syncfusion.Windows.PdfViewer;
using System.Globalization;
using System.IO;
using System.Windows;
using Syncfusion.Pdf;
using System.Text.RegularExpressions;

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

        private List<Invoice> _csvData;
        public List<Invoice> CsvData
        {
            get => _csvData;
            set
            {
                SetProperty(ref _csvData, value);
                OnPropertyChanged(nameof(HasCsvData));
            }
        }

        public bool HasCsvData => CsvData != null && CsvData.Any();

        private string _searchResultMessage;
        public string SearchResultMessage
        {
            get => _searchResultMessage;
            set => SetProperty(ref _searchResultMessage, value);
        }
        public IRelayCommand LoadPdfCommand { get; }
        public IRelayCommand ClearPdfListCommand { get; }
        public IRelayCommand LoadCsvCommand { get; }
        public IRelayCommand SearchCommand { get; }

        public MainViewModel()
        {
            PdfFiles = new List<string>();
            CsvData = new List<Invoice>();

            LoadPdfCommand = new RelayCommand(LoadPdf);
            ClearPdfListCommand = new RelayCommand(ClearPdfList);
            LoadCsvCommand = new RelayCommand(LoadCsv);
            SearchCommand = new RelayCommand(Search);
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
            SearchResultMessage = "";
        }

        private void LoadCsv()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string csvPath = openFileDialog.FileName;

                try
                {
                    using (var reader = new StreamReader(csvPath))
                    using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
                    {
                        CsvData = csv.GetRecords<Invoice>().ToList();
                    }

                    MessageBox.Show($"Loaded {CsvData.Count} invoices from CSV file.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error reading CSV file: {ex.Message}");
                }
            }
        }

        private void Search()
        {
            if (!PdfFiles.Any() || !CsvData.Any())
            {
                MessageBox.Show("Please load both PDF files and a CSV file first.");
                return;
            }

            var matchingFiles = new List<string>();

            foreach (var pdfFile in PdfFiles)
            {
                // Extract text from the current PDF file
                string pdfText = ExtractTextFromPdf(pdfFile);

                // Check if any Invoice object matches the extracted text
                if (CsvData.Any(invoice =>
                        Regex.IsMatch(pdfText, Regex.Escape(invoice.InvoiceNumber), RegexOptions.IgnoreCase) &&
                        Regex.IsMatch(pdfText, Regex.Escape(invoice.Price.ToString()), RegexOptions.IgnoreCase)))
                {
                    matchingFiles.Add(pdfFile);
                }
            }

            SearchResultMessage = matchingFiles.Any()
                ? $"Found {matchingFiles.Count} matching PDF files:\n{string.Join("\n", matchingFiles)}"
                : "No matching PDF files found.";
        }

        private string ExtractTextFromPdf(string path)
        {
            try
            {
                PdfDocumentView pdfDocumentView = new PdfDocumentView();
                //Load the PDF file.
                pdfDocumentView.Load(path);

                //Extract text from the file.
                TextLines textLines = new TextLines();
                string extractedText = string.Empty;
                for (int i = 0; i < pdfDocumentView.PageCount; i++)
                {
                    extractedText += pdfDocumentView.ExtractText(i, out textLines);
                }

                return extractedText;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading PDF file {path}: {ex.Message}");
                return string.Empty;
            }
        }
    }
}
