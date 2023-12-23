namespace ValidationAndSegregated
{
    public class Transaction
    {
        public int? GoodID { get; set; }
        public int? TransactionID { get; set; }
        public DateTime? TransactionDate { get; set; }
        public int? Amount { get; set; }
        public Status? Direction { get; set; }
        public string Comments { get; set; }
    }

}