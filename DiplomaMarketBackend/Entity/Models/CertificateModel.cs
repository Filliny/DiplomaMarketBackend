namespace DiplomaMarketBackend.Entity.Models
{
    public class CertificateModel
    {
        public int Id { get; set; } 

        /// <summary>
        /// issuer or buyer id
        /// </summary>
        public string UserId { get; set; } = string.Empty;
        public string CertificateCode { get; set; } = string.Empty;
        public decimal Summ { get; set; }
        public DateTime Issued { get; set; }
        public DateTime ValidUntil { get; set; }
        public DateTime? Closed { get; set; } = null;
        public bool Unused { get; set; }

        public int? OrderId { get; set; }
        public OrderModel? Order { get; set; }


    }
}
