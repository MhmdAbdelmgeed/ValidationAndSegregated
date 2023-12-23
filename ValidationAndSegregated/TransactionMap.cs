using CsvHelper.Configuration;
using System.Globalization;

namespace ValidationAndSegregated
{
    public class TransactionMap : ClassMap<Transaction>
    {
        public TransactionMap()
        {
            AutoMap(CultureInfo.InvariantCulture); 
            Map(m => m.GoodID).Name("GoodID").TypeConverterOption.NullValues(string.Empty);
            Map(m => m.TransactionDate)
                .Name("TransactionDate")
                .TypeConverterOption.Format(
                    new[] { "d/M/yyyy", "dd/MM/yyyy", "d-MM-yyyy", "dd-MM-yyyy" } 
                )
                .TypeConverterOption.CultureInfo(CultureInfo.InvariantCulture); 
        }
    }
}