namespace NetPay.DataProcessor.ExportDtos
{
    public class ExportServiceDto
    {
        public string ServiceName { get; set; } = null!;
        public ExportSupplierDto[] Suppliers { get; set; } = null!;
    }

    public class ExportSupplierDto
    {
        public string SupplierName { get; set; } = null!;
    }
}
