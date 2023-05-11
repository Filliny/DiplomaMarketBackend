namespace DiplomaMarketBackend.Models
{
    public class SliderValues
    {
        public int characteristic_id {  get; set; }
        public int lower_value { get; set; }
        public int upper_value { get; set; }
    }

    public class Filter
    {
        /// <summary>
        /// List of values ids for filtering by characteristics from checkboxes
        /// </summary>
        public List<int> values_id {get; set;} = new List<int>();

        /// <summary>
        /// List of brands id for filtering
        /// </summary>
        public List<int?> brands_id { get; set;} = new List<int?>();

        /// <summary>
        /// List of sliders values for filtering
        /// </summary>
        public List<SliderValues> slider_values { get; set;} = new List<SliderValues>();

        /// <summary>
        /// Lower price bound
        /// </summary>
        public decimal price_low { get; set;}

        /// <summary>
        /// Higer price bound
        /// </summary>
        public decimal price_high { get; set;}
    }
}
