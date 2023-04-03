namespace DiplomaMarketBackend.Entity.Models
{
    public class CharacteristicGroupModel
    {
        public int Id { get; set; }
        public int group_order { get; set; }
        public TextContent? groupTitle { get; set; }
        public List<ArticleCharacteristic> options { get; set; }

        public CharacteristicGroupModel()
        {
            options = new();
        }

        public int rztk_grp_id { get; set; }
    }
}
