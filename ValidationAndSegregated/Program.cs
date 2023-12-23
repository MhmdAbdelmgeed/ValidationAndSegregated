using CsvHelper;
using System.Globalization;

namespace ValidationAndSegregated
{

    public class Program
    {

        static void Main(string[] args)
        {
            string filePath = "G:\\NetCore\\Pioneers Technology\\StoreData.csv";

            if (!File.Exists(filePath))
            {
                Console.WriteLine($"File not found: {filePath}");
                return;
            }

            string directory = Path.GetDirectoryName(filePath)!;
            if (directory == null)
            {
                Console.WriteLine($"Invalid directory for the file path: {filePath}");
                return;
            }

            Console.WriteLine($"File Directory: {directory}");

            List<Transaction> validRecords = new List<Transaction>();
            List<string> errorRecords = new List<string>();

            try
            {
                using (var reader = new StreamReader(filePath))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    csv.Context.RegisterClassMap<TransactionMap>();

                    while (csv.Read())
                    {
                        try
                        {
                            var record = csv.GetRecord<Transaction>();
                            validRecords.Add(record);
                        }
                        catch (CsvHelper.TypeConversion.TypeConverterException ex)
                        {
                            string errorMessage = $"Error converting row {csv.Context.Parser.Row} - Raw Data: {csv.Context.Parser.RawRecord}";
                            Console.WriteLine(errorMessage);
                            errorRecords.Add(errorMessage);
                        }
                    }

                    var segregatedData = validRecords
                        .Where(x => x.GoodID.HasValue)
                        .GroupBy(x => x.GoodID.Value);

                    ExportErrorsToLog(errorRecords, directory);

                    foreach (var group in segregatedData)
                    {
                        string fileName = Path.Combine(directory, $"output_{group.Key}.csv");
                        SaveToFile(group.ToList(), fileName);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected exception occurred: {ex.Message}");
            }
        }

        private static void ExportErrorsToLog(List<string> errorRecords, string directory)
        {
            string errorFilePath = Path.Combine(directory, "errors.log");
            using (StreamWriter writer = new StreamWriter(errorFilePath))
            {
                foreach (var errorRecord in errorRecords)
                {
                    writer.WriteLine(errorRecord);
                }
            }
        }
        static bool HasNullValues(Transaction transaction)
        {
            var type = transaction.GetType();
            return type.GetProperties()
                .Where(prop => prop.Name != "Comments" &&
                               prop.PropertyType != typeof(string) &&
                               prop.GetValue(transaction) == null)
                .Any();
        }

        static void ExportErrorsToLog(List<Transaction> errorRecords, string directory)
        {
            string errorFilePath = Path.Combine(directory, "errors.log");
            using (StreamWriter writer = new StreamWriter(errorFilePath))
            {
                foreach (var errorRecord in errorRecords)
                {
                    writer.WriteLine($"Error in TransactionID: {errorRecord.TransactionID}");
                }
            }
        }

        static void SaveToFile(List<Transaction> transactions, string fileName)
        {
            using (var writer = new StreamWriter(fileName))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(transactions);
            }
        }

    }

}