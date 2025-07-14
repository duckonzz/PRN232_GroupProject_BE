namespace GenderHealthCare.ModelViews.ReportModels
{
    public class ExportFileResult
    {
        public byte[] FileBytes { get; set; }
        public string ContentType { get; set; } // "text/csv" or "application/pdf"
        public string FileName { get; set; }
    }

}
