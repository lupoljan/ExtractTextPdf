using CsvHelper.Configuration.Attributes;

namespace ExtractTextPdf.Models
{
    public class Invoice
    {
        [Name("Numer faktury")]
        public string InvoiceNumber { get; set; }
        [Name("Kwota")]
        public int Price { get; set; }
    }

}
