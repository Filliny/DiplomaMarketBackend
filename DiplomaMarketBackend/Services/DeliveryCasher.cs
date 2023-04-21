using DiplomaMarketBackend.Abstract;
using DiplomaMarketBackend.Entity;
using DiplomaMarketBackend.Entity.Models.Delivery;
using DiplomaMarketBackend.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace DiplomaMarketBackend.Services
{
    public class DeliveryCasher : IDeliveryCasher
    {
        private readonly IServiceScopeFactory scopeFactory;
        private readonly BaseContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private string? _np_api_key;
        private string? _np_api_url;
        private string? _mist_api_url;
        private Task? task;

        public DeliveryCasher(IServiceScopeFactory scopeFactory, IConfiguration configuration)
        {

            this.scopeFactory = scopeFactory;

            var scope = scopeFactory.CreateScope();

            _context = scope.ServiceProvider.GetRequiredService<BaseContext>();
            _logger = scope.ServiceProvider.GetRequiredService<ILogger<DeliveryCasher>>();


            _configuration = configuration;
            _np_api_key = _configuration.GetValue("NpApiKey", "");
            _np_api_url = _configuration.GetValue("NpApiUrl", "");
            _mist_api_url = _configuration.GetValue("MistApiUrl", "");
        }


        public void Run()
        {
            if (_np_api_key.IsNullOrEmpty() || _np_api_url.IsNullOrEmpty())
            {
                throw new Exception("Configuration value 'NpApiKey' or 'NpApiUrl' not found - please specify your api key and url");

            }

            if (_mist_api_url.IsNullOrEmpty())
            {
                throw new Exception("Configuration value 'MistApiUrl' not found - please specify api url");

            }


            if (task == null || task.IsCompleted)
            {

                task = Task.Factory.StartNew(() =>
                {

                    _logger.LogWarning("Delivery Casher started");

                    //updateAreas();
                    //updateNpCities();
                    UpdateMistBranches();

                });
            }

        }


        private async void UpdateMistBranches()
        {
            try
            {
                var resp_branches = await GetMistResponseAsync();

                var branches = JsonConvert.DeserializeObject<Parser.Mist.Branches.Root>(resp_branches);

                if (branches != null)
                {

                    foreach (var branch in branches.result)
                    {
                       //if (_context.Branches.Any(b => b.DeliveryBranchId == branch.br_id)) continue;

                        var resp_branch = await GetMistResponseAsync(branch.br_id);

                        var branch_data = JsonConvert.DeserializeObject<Parser.Mist.OneBranch.Root>(resp_branch);

                        if (branch_data != null)
                        {
                            var br = branch_data.result.First();

                            ///check city for branch

                            var city = _context.Cities.
                                Include(c => c.Area).
                                Include(c => c.Name).
                                FirstOrDefault(c => c.Name.OriginalText.ToUpper().Equals(br.city.ua.ToUpper()) && c.Area.Description.ToUpper().Equals(br.region.ua));

                            if(city == null)
                            {
                                city = _context.Cities.FirstOrDefault(c => c.NpCityRef == br.city_id);
                            }


                            if (city == null)
                            {
                                var city_resp = await GetMistCities(br.district_id, br.city.ua);

                                if (city_resp != null)
                                {
                                    var city_data = JsonConvert.DeserializeObject<Parser.Mist.City.Root>(city_resp);

                                    if (city_data != null)
                                    {
                                        var fcity = city_data.result.First().data;

                                        var area = _context.Areas.FirstOrDefault(a => fcity.reg.Contains(a.Description.ToUpper()));

                                        //create city of not exist

                                        var name_content = TextContentHelper.CreateTextContent(_context, fcity.n_ua, "UK");
                                        _context.SaveChanges();
                                        TextContentHelper.UpdateTextContent(_context, fcity.n_ru, name_content.Id, "RU");

                                        city = new CityModel()
                                        {
                                            Area = area,
                                            Name = name_content,
                                            NpCityRef = fcity.city_id,
                                            CoatsuCode = fcity.kt,

                                        };

                                        _context.Cities.Add(city);
                                        _context.SaveChanges();

                                    }

                                }

                            }

                            var new_branch = _context.Branches.FirstOrDefault(b => b.DeliveryBranchId == br.br_id);

                            if (new_branch != null)
                            {
                                new_branch.Updated = DateTime.Now;
                                _context.Branches.Update(new_branch);

                            }
                            else
                            {
                                new_branch = new BranchModel();
                                _context.Branches.Add(new_branch);
                                var desc_content = TextContentHelper.CreateFull(_context, br.location_description, br.location_description);
                                var street_content = TextContentHelper.CreateFull(_context, br.street.ua + " " + br.street_number, br.street.ru + " " + br.street_number);
                                new_branch.Address = street_content;
                                new_branch.Description = desc_content;
                                new_branch.Updated = DateTime.Now;
                            }

                            new_branch.DeliveryBranchId = br.br_id;
                            new_branch.DeliveryId = 2;
                            new_branch.LocalBranchNumber = br.num_showcase;
                            new_branch.BranchCity = city;
                            new_branch.Long = br.lng;
                            new_branch.Lat = br.lat;
                            new_branch.WorkHours = br.working_hours;

                            _context.SaveChanges();

                        }

                    }

                }

            }
            catch (Exception e)
            {

                _logger.LogWarning("Mist branches updating error : " + e.Message, e);
            }

        }

        private async void updateAreas()
        {
            try
            {
                string responce;

                using (var client = new HttpClient())
                {

                    var data = new
                    {
                        apiKey = _np_api_key,
                        modelName = "Address",
                        calledMethod = "getAreas",
                        methodProperties = new { }
                    };

                    var jdata = JsonConvert.SerializeObject(data);

                    var content = new StringContent(jdata, System.Text.Encoding.UTF8, "application/json");
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, _np_api_url);

                    var result = await client.PostAsync(_np_api_url, content);

                    responce = await result.Content.ReadAsStringAsync();

                }

                var responce_data = JsonConvert.DeserializeObject<Parser.NewPost.Areas.Root>(responce);

                if (responce_data != null)
                {

                    foreach (var area in responce_data.data)
                    {

                        if (_context.Areas.Any(a => a.NpAreaCenterRef == area.AreasCenter)) { continue; }

                        var new_area = new AreaModel()
                        {
                            NpRef = area.Ref,
                            NpAreaCenterRef = area.AreasCenter,
                            Description = area.Description,
                            DescriptionRu = area.DescriptionRu,
                        };

                        _context.Areas.Add(new_area);
                        await _context.SaveChangesAsync();

                        _logger.LogInformation($"Area {new_area.Description} added");
                    }
                }


            }
            catch (Exception e)
            {
                _logger.LogWarning($"Delivery casher Updating areas error: {e.Message}");
            }

        }

        private async void updateNpCities()
        {
            //var areas = _context.Areas.ToList(); ///not needed - NP areas codes is corrupted
            var total = 0;

            try
            {
                var data = new
                {
                    apiKey = _np_api_key,
                    modelName = "Address",
                    calledMethod = "getSettlements",
                    methodProperties = new
                    {
                        Limit = 1
                    }
                };

                var response = await GetNpResponseAsync(data);


                var responce_data = JsonConvert.DeserializeObject<Parser.NewPost.Cities.Root>(response);
                if (responce_data != null)
                    total = responce_data.info.totalCount;
            }
            catch (Exception e)
            {

                _logger.LogWarning($"Delivery casher Updating cities error get total : {e.Message}");
            }

            int pages = (int)Math.Ceiling((decimal)total / 100);


            for (int i = 1; i <= pages; i++)
            {
                try
                {
                    var data = new
                    {
                        apiKey = _np_api_key,
                        modelName = "Address",
                        calledMethod = "getSettlements",
                        methodProperties = new
                        {
                            Limit = 100,
                            Page = i,
                        }
                    };

                    var response = await GetNpResponseAsync(data);

                    var responce_data = JsonConvert.DeserializeObject<Parser.NewPost.Cities.Root>(response);

                    foreach (var city in responce_data.data)
                    {
                        if (city == null) continue;

                        var area = _context.Areas.FirstOrDefault(a => city.AreaDescription.Contains(a.Description));

                        ///update areas to np real areas codes and add translations
                        if (area != null && area.NameId == null)
                        {
                            var content = TextContentHelper.CreateTextContent(_context, city.AreaDescription, "UK");
                            _context.SaveChanges();
                            TextContentHelper.UpdateTextContent(_context, city.AreaDescriptionRu, content.Id, "RU");
                            area.Name = content;
                            area.NpRef = city.Area;
                            _context.SaveChanges();

                        }


                        //create city of not exist
                        if (!_context.Cities.Any(c => c.NpCityRef == city.Ref))
                        {
                            var name_content = TextContentHelper.CreateTextContent(_context, city.Description, "UK");
                            _context.SaveChanges();
                            TextContentHelper.UpdateTextContent(_context, city.DescriptionRu, name_content.Id, "RU");

                            var new_city = new CityModel()
                            {
                                Area = area,
                                Name = name_content,
                                Lat = city.Latitude,
                                Long = city.Longitude,
                                NpCityRef = city.Ref,
                                CoatsuCode = city.IndexCOATSU1,
                                Index1 = city.Index1,
                                Index2 = city.Index2,
                            };

                            _context.Cities.Add(new_city);
                            _context.SaveChanges();

                        }


                    }


                }
                catch (Exception e)
                {

                    _logger.LogWarning($"Delivery casher Updating cities error: {e.Message}");
                }
            }

        }

        private async void updateNpBranches()
        {
            //var settlements = _context.
        }


        private async Task<string> GetNpResponseAsync(dynamic data)
        {
            string response;

            using (var client = new HttpClient())
            {

                var jdata = JsonConvert.SerializeObject(data);

                var content = new StringContent(jdata, System.Text.Encoding.UTF8, "application/json");

                var result = await client.PostAsync(_np_api_url, content);

                response = await result.Content.ReadAsStringAsync();

            }

            return response;
        }

        private async Task<string> GetMistResponseAsync(string? branch = null)
        {
            string? response;

            using (var client = new HttpClient())
            {
                var url = _mist_api_url;
                client.DefaultRequestHeaders.Clear();

                if (branch != null)
                {
                    url = url + "branches/" + branch;
                }
                else
                {
                    url = url + "branches";
                }
                var result = await client.GetAsync(url);

                using (var sr = new StreamReader(await result.Content.ReadAsStreamAsync()))
                {
                    response = sr.ReadToEnd();
                }

            }

            return response;
        }

        private async Task<string> GetMistCities(string? district_id = null, string? city_name = null)
        {
            string? response;

            using (var client = new HttpClient())
            {
                var url = _mist_api_url;
                client.DefaultRequestHeaders.Clear();

                if (district_id != null && city_name != null)
                {
                    url = url + "geo_localities?region_id=" + district_id + "&search_full=" + city_name;
                }
                var result = await client.GetAsync(url);

                using (var sr = new StreamReader(await result.Content.ReadAsStreamAsync()))
                {
                    response = sr.ReadToEnd();
                }

            }

            return response;
        }

    }
}
