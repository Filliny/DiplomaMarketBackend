using DiplomaMarketBackend.Abstract;
using DiplomaMarketBackend.Entity;
using DiplomaMarketBackend.Entity.Models.Delivery;
using DiplomaMarketBackend.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace DiplomaMarketBackend.Services
{
    public class DeliveryCasher : BackgroundService, IDeliveryCasher
    {
        private const int generalDelay = 24 * 60 * 60 * 1000; // 30 days seconds
        private const int daysDelay = 30;
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

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (!_context.Cities.Any())
                {
                    await Task.Delay(60 * 1000);
                    Run();
                }

                for (int i = 0; i < daysDelay; i++)
                {
                    _logger.LogInformation($"{30 - i} days left to update branches");
                    await Task.Delay(generalDelay);
                }

                Run();

            }

        }

        /// <summary>
        /// For manual or automated run delivery branches updating task.
        /// </summary>
        /// <exception cref="Exception">throws exception if no api keys in configuration</exception>
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

                task = Task.Factory.StartNew(async () =>
                {

                    _logger.LogInformation("Delivery Casher started");

                    await updateAreas();
                    await updateNpCities();
                    await UpdateMistBranches();
                    await updateNpBranches();
                    //updateUkrpost();

                });
            }
            else
            {
                _logger.LogWarning("Delivery Casher already started - check where its stuck!");
            }

        }


        private async Task UpdateMistBranches()
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

                            //check city for branch
                            //try find by name and area
                            var city = _context.Cities.
                                Include(c => c.Area).
                                Include(c => c.Name).
                                FirstOrDefault(c => c.Name.OriginalText.ToUpper().Equals(br.city.ua.ToUpper()) && c.Area.Description.ToUpper().Equals(br.region.ua));

                            //try find by mist city id
                            if (city == null)
                            {
                                city = _context.Cities.FirstOrDefault(c => c.NpCityRef == br.city_id);
                            }

                            //Try get by koatsu
                            if (city == null)
                            {
                                var city_resp = await GetMistCities(br.district_id, br.city.ua);

                                if (city_resp != null)
                                {
                                    var city_data = JsonConvert.DeserializeObject<Parser.Mist.City.Root>(city_resp);
                                    var fcity = city_data.result.First().data;
                                    city = _context.Cities.FirstOrDefault(c => c.CoatsuCode == fcity.kt);
                                }
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
                                            Creator = "mist_casher",
                                            Created = DateTime.Now,

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

        private async Task updateAreas()
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

        private async Task updateNpCities()
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


                        //create city if not exist
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

        private async Task updateNpBranches()
        {
            var cities = await _context.Cities.ToListAsync();
            string response = "";

            try
            {
                foreach (var city in cities)
                {

                    var data = new
                    {
                        apiKey = _np_api_key,
                        modelName = "Address",
                        calledMethod = "getWarehouses",
                        methodProperties = new
                        {
                            SettlementRef = city.NpCityRef
                        }
                    };

                    do
                    {
                        response = await GetNpResponseAsync(data);

                        if (response.Contains("\"success\":false"))
                        {
                            _logger.LogInformation(response);
                            Thread.Sleep(1000);
                        }

                    } while (response.Contains("\"success\":false"));


                    if (response.Contains("\"info\":{\"totalCount\":0}"))
                    {
                        Thread.Sleep(500);
                        continue;
                    }


                    if (response != null)
                    {

                        var responce_data = JsonConvert.DeserializeObject<Parser.NewPost.Branches.Root>(response);

                        if (responce_data != null && responce_data.success && responce_data.data.Count > 0)
                        {
                            foreach (var branch in responce_data.data)
                            {
                                var new_branch = _context.Branches.FirstOrDefault(b => b.DeliveryBranchId == branch.Ref);

                                if (new_branch != null)
                                {
                                    _context.Branches.Update(new_branch);
                                    new_branch.Updated = DateTime.Now;
                                }
                                else
                                {
                                    new_branch = new BranchModel();
                                    _context.Branches.Add(new_branch);

                                    var desc_content = TextContentHelper.CreateFull(_context, branch.Description, branch.DescriptionRu);

                                    var street_content = TextContentHelper.CreateFull(_context, branch.ShortAddress, branch.ShortAddressRu);
                                    new_branch.Address = street_content;
                                    new_branch.Description = desc_content;
                                    new_branch.Updated = DateTime.Now;
                                }

                                new_branch.DeliveryBranchId = branch.Ref;
                                new_branch.DeliveryId = 1;
                                new_branch.LocalBranchNumber = branch.Number;
                                new_branch.BranchCity = city;
                                new_branch.Long = branch.Longitude;
                                new_branch.Lat = branch.Latitude;
                                new_branch.WorkHours = $"Пн:{branch.Schedule.Monday}," +
                                    $"Вт:{branch.Schedule.Tuesday}," +
                                    $"Ср:{branch.Schedule.Wednesday}," +
                                    $"Чт:{branch.Schedule.Thursday}," +
                                    $"Пт:{branch.Schedule.Friday}," +
                                    $"Сб:{branch.Schedule.Saturday}," +
                                    $"Вс:{branch.Schedule.Sunday} ";

                                _context.SaveChanges();


                            }


                        }

                    }
                }
            }
            catch (Exception e)
            {

                _logger.LogWarning($"Delivery casher Updating NewPost branches error  : {e.Message}");

            }



        }

        private async void updateUkrpost()
        {

            var kuiv_dist_uid = "333f9e71-a1fd-46cc-8f6a-371420e3e280";
            var delengine_key = "4fcih5kffnwoobpjeel83t73vvj5a0wq";

            try
            {
                using (var client = new HttpClient())
                {
                    var result = await client.GetAsync("https://api.delengine.com/v1.0/settlements?page=1&region_uuid=333f9e71-a1fd-46cc-8f6a-371420e3e280&token=" + delengine_key);

                    if (result != null)
                    {
                        var resp = await result.Content.ReadAsStringAsync();
                        var cities = JsonConvert.DeserializeObject<Parser.Delengine.Cities.Root>(resp);

                        if (cities != null)
                        {
                            foreach (var city in cities.data)
                            {

                                var city_base = _context.Cities.Include(c => c.Area).Include(c => c.Name).
                                    FirstOrDefault(c => c.Name.OriginalText.ToUpper().Equals(city.name_uk.ToUpper()) && c.Area.Description.ToUpper().Equals(city.region.name_uk.ToUpper()));

                                if (city_base != null)
                                {
                                    var url = $"https://api.delengine.com/v1.0/departments?page=1&settlement_uuid=" + city.uuid + "&company_uuid=64054fa0-8584-4492-8425-142156ce3110&token=" + delengine_key;

                                    var city_res = await client.GetAsync(url);
                                    var city_str = await city_res.Content.ReadAsStringAsync();

                                    var branches = JsonConvert.DeserializeObject<Parser.Delengine.Branches.Root>(city_str);

                                    if (branches != null)
                                    {
                                        foreach (var branch in branches.data)
                                        {
                                            var new_branch = _context.Branches.FirstOrDefault(b => b.DeliveryBranchId == branch.uuid);

                                            if (new_branch != null)
                                            {
                                                new_branch.Updated = DateTime.Now;
                                                _context.Branches.Update(new_branch);

                                            }
                                            else
                                            {
                                                new_branch = new BranchModel();
                                                _context.Branches.Add(new_branch);
                                                var desc_content = TextContentHelper.CreateFull(_context, branch.department_type.name_uk, branch.department_type.name_uk);
                                                var street_content = TextContentHelper.CreateFull(_context, branch.address_uk, branch.address_uk);
                                                new_branch.Address = street_content;
                                                new_branch.Description = desc_content;
                                                new_branch.Updated = DateTime.Now;
                                            }

                                            new_branch.DeliveryBranchId = branch.uuid;
                                            new_branch.DeliveryId = 3;
                                            new_branch.LocalBranchNumber = branch.number.ToString();
                                            new_branch.BranchCity = city_base;
                                            new_branch.Long = (branch.longitude ?? (double)0).ToString();
                                            new_branch.Lat = (branch.latitude ?? (double)0).ToString();
                                            new_branch.WorkHours = branch.schedules ?? "";

                                            _context.SaveChanges();
                                        }
                                    }
                                }



                            }

                        }


                    }



                }
            }
            catch (Exception e)
            {
                _logger.LogWarning($"Delivery casher Updating Ukrpost branches error  : {e.Message}");
            }

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
