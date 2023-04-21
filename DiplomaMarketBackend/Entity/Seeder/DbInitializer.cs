using DiplomaMarketBackend.Entity.Models.Delivery;
using DiplomaMarketBackend.Helpers;
using Microsoft.EntityFrameworkCore;

namespace DiplomaMarketBackend.Entity.Seeder
{
    public class DbInitializer
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {

            using (var context = new BaseContext(serviceProvider.GetRequiredService<DbContextOptions<BaseContext>>()))
            {
                await InitDeliveries(context);
            }
        }

        private static async Task InitDeliveries(BaseContext context)
        {
            if (context != null)
            {

                if (context.Deliveries.Count() == 0)
                {

                    var np = CreateDelivery(context, "Нова пошта", "Новая почта");
                    context.Deliveries.Add(np);


                    var mist = CreateDelivery(context, "Meest", "Meest");
                    context.Deliveries.Add(mist);


                    var post = CreateDelivery(context, "Укрпошта", "Укрпочта");
                    context.Deliveries.Add(post);

                    await context.SaveChangesAsync();
                }

            }
        }

        private static DeliveryModel CreateDelivery(BaseContext context, string name_uk, string name_ru)
        {
            var content = TextContentHelper.CreateTextContent(context, name_uk, "UK");
            context.SaveChanges();
            TextContentHelper.UpdateTextContent(context, name_ru, content.Id, "RU");

            var delivery = new DeliveryModel()
            {
                Name = content
            };

            return delivery;

        }

    }
}
