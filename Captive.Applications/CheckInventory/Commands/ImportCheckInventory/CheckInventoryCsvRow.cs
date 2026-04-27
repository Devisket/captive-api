namespace Captive.Applications.CheckInventory.Commands.ImportCheckInventory
{
    /// <summary>
    /// Represents one row in the CSV import file.
    /// BRSTNCodes, ProductCodes, FormCheckTypes use semicolons to separate multiple values.
    /// Empty BRSTNCodes/ProductCodes/FormCheckTypes = applies to all (wildcard).
    /// BRSTNCodes must match BRSTNCode in bank_branches table.
    /// ProductCodes must match FileName in product_configurations table.
    /// FormCheckTypes accepts: Personal, Commercial.
    /// Empty EndingSeries = long.MaxValue.
    /// Empty WarningSeries = 90% of range.
    /// </summary>
    public class CheckInventoryCsvRow
    {
        public string SeriesPattern { get; set; } = string.Empty;
        public int NumberOfPadding { get; set; } = 4;
        public long StartingSeries { get; set; }
        public long? EndingSeries { get; set; }
        public long? WarningSeries { get; set; }
        public bool IsRepeating { get; set; } = false;

        // Semicolon-separated BRSTN codes; empty = all branches
        public string? BRSTNCodes { get; set; }

        // Semicolon-separated ProductConfiguration.FileName values (e.g. MCX;MDT); empty = all products
        public string? ProductCodes { get; set; }

        // Semicolon-separated: Personal;Commercial; empty = all
        public string? FormCheckTypes { get; set; }

        // Optional: account number for account-specific mapping
        public string? AccountNumber { get; set; }
    }
}
