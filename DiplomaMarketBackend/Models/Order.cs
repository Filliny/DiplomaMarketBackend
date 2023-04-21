namespace DiplomaMarketBackend.Models
{
    public class Order
    {
        public int id { get; set; }
        public int  user_id{ get; set; }

        /// <summary>
        /// goods dictionary, where key - article id, value - quantity
        /// </summary>
        public Dictionary<int , int> goods { get;} = new Dictionary<int , int>();

    }
}
