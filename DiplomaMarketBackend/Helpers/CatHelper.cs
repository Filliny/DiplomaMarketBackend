using DiplomaMarketBackend.Entity;

namespace DiplomaMarketBackend.Helpers
{
    public static class CatHelper
    {
        public static async Task<int?> GetTopCat(BaseContext db, int? cat_id)
        {
            do
            {
                var cat = await db.Categories.FindAsync(cat_id);
                if (cat != null) {

                    cat_id = cat.ParentCategoryId;
                    if (cat.ParentCategoryId == null) return cat.Id;
                }

            } while (cat_id != null);

            return null;
        }
    }
}
