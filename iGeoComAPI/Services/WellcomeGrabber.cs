using iGeoComAPI.Models;
using iGeoComAPI.Options;
using iGeoComAPI.Utilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace iGeoComAPI.Services
{
    public class WellcomeGrabber
    {
        private readonly PuppeteerConnection _puppeteerConnection;
        private readonly IOptions<WellcomeOptions> _options;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<WellcomeGrabber> _logger;
        private readonly string infoCode = @"() =>{"+
            @"const selectors = Array.from(document.querySelectorAll('.table-responsive > .table-striped > tbody > tr'));"+
            @"return selectors.map(v => {return {Address: v.querySelector('.views-field-field-address').textContent.trim(), Name: v.querySelector("+
            @"'.views-field-title > .store-title',).textContent, LatLng: v.querySelector('.views-field-title > .store-title').getAttribute('data-latlng'),"+
            @"Phone: v.querySelector('.views-field-field-store-telephone').textContent.trim()"+
            @"}});}";
        private string waitSelector = ".table-responsive";
        private string _regLagLngRegex = "([^|]*)";

        //public WellcomeGrabber(PuppeteerConnection puppeteerConnection, IOptions<WellcomeOptions> options, IMemoryCache memoryCache, ILogger<WellcomeGrabber> logger,
        //    IOptions<NorthEastOptions> absOptions, ConnectClient httpClient, JsonFunction json) : base(httpClient, absOptions, json)
        //{
        //    _puppeteerConnection = puppeteerConnection;
        //    _options = options;
        //    _memoryCache = memoryCache;
        //    _logger = logger;
        //}
        public WellcomeGrabber(PuppeteerConnection puppeteerConnection, IOptions<WellcomeOptions> options, IMemoryCache memoryCache, ILogger<WellcomeGrabber> logger,
    IOptions<NorthEastOptions> absOptions, ConnectClient httpClient, JsonFunction json) 
        {
            _puppeteerConnection = puppeteerConnection;
            _options = options;
            _memoryCache = memoryCache;
            _logger = logger;
        }

        public async Task<List<IGeoComGrabModel>?> GetWebSiteItems()
        {
           var enResult = await _puppeteerConnection.PuppeteerGrabber<WellcomeModel[]>(_options.Value.EnUrl, infoCode, waitSelector);
           var zhResult = await _puppeteerConnection.PuppeteerGrabber<WellcomeModel[]>(_options.Value.ZhUrl, infoCode, waitSelector);
           var enResultList = enResult.ToList();
           var zhResultList = zhResult.ToList();
           var mergeResult = await MergeEnAndZh(enResultList, zhResultList);
           //_memoryCache.Set("iGeoCom", mergeResult, TimeSpan.FromHours(2));
            return mergeResult;

        }

        public async Task<List<IGeoComGrabModel>> MergeEnAndZh(List<WellcomeModel> enResult, List<WellcomeModel> zhResult)
        {
            try
            {
                _logger.LogInformation("Merge Wellcome En and Zh");
                var _rgx = Regexs.ExtractInfo(_regLagLngRegex);
                List<IGeoComGrabModel> WellcomeIGeoComList = new List<IGeoComGrabModel>();
                foreach (var item in enResult.Select((value, i) => new { i, value }))
                {
                    var shopEn = item.value;
                    var index = item.i;
                    IGeoComGrabModel WellcomeIGeoCom = new IGeoComGrabModel();
                    WellcomeIGeoCom.E_Address = shopEn.Address!;
                    WellcomeIGeoCom.EnglishName = $"Wellcome Supermarket-{shopEn.Name}";
                    var matchesEn = _rgx.Matches(shopEn.LatLng!);
                    WellcomeIGeoCom.Latitude = Convert.ToDouble(matchesEn[0].Value);
                    WellcomeIGeoCom.Longitude = Convert.ToDouble(matchesEn[2].Value);
                    WellcomeIGeoCom.Tel_No = shopEn.Phone!;
                    WellcomeIGeoCom.Web_Site = _options.Value.BaseUrl!;
                    WellcomeIGeoCom.Class = "CMF";
                    WellcomeIGeoCom.Type = "SMK";
                    WellcomeIGeoCom.Source = "27";
                    WellcomeIGeoCom.GrabId = $"wellcome_{shopEn.LatLng}{shopEn.Phone}_{index}".Replace(" ", "").Replace("|", "").Replace(".", "");
                    foreach (var shopZh in zhResult)
                    {
                        var matchesZh = _rgx.Matches(shopZh.LatLng!);
                        if (matchesZh.Count > 0 && matchesZh != null)
                        {
                            if (matchesEn[0].Value == matchesZh[0].Value && matchesEn[2].Value == matchesZh[2].Value && WellcomeIGeoCom.Tel_No == shopZh.Phone)
                            {
                                WellcomeIGeoCom.C_Address = shopZh.Address!.Replace(" ", "");
                                var cFloor = Regexs.ExtractC_Floor().Matches(WellcomeIGeoCom.C_Address);
                                if (cFloor.Count > 0 && cFloor != null)
                                {
                                    WellcomeIGeoCom.C_floor = cFloor[0].Value;
                                }
                                WellcomeIGeoCom.ChineseName = $"惠康超級市場-{shopZh.Name}";
                                continue;
                            }
                        }
                    }
                    WellcomeIGeoComList.Add(WellcomeIGeoCom);
                }
                return WellcomeIGeoComList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "fail to merge Wellcome En and Zh");
                throw;
            }
        }


        public List<IGeoComDeltaModel> FindAdded(List<IGeoComGrabModel> newData, List<IGeoComModel> previousData)
        {
            int newDataLength = newData.Count;
            int previousDataLength = previousData.Count;
            List<IGeoComDeltaModel> AddedWellcomeIGeoComList = new List<IGeoComDeltaModel>();

            for (int i = 0; i < newDataLength; i++)
            {
                int j;
                for (j = 0; j < previousDataLength; j++)

                    if (newData[i].E_Address?.Replace(" ", "") == previousData[j].E_Address?.Replace(" ", "") |
                       newData[i].Tel_No?.Replace(" ", "") == previousData[j].Tel_No?.Replace(" ", "") |
                       newData[i].C_Address?.Replace(" ", "") == previousData[j].C_Address?.Replace(" ", "")
                        )
                        break;
                if (j == previousDataLength)
                {
                    IGeoComDeltaModel AddedShop = new IGeoComDeltaModel();
                    AddedShop.status = "added";
                    AddedShop.EnglishName = newData[i].EnglishName;
                    AddedShop.ChineseName = newData[i].ChineseName;
                    AddedShop.Class = newData[i].Class;
                    AddedShop.Type = newData[i].Type;
                    AddedShop.E_Address = newData[i].E_Address;
                    AddedShop.C_Address = newData[i].C_Address;
                    AddedShop.Tel_No = newData[i].Tel_No;
                    AddedShop.Web_Site = newData[i].Web_Site;
                    AddedShop.Latitude = newData[i].Latitude;
                    AddedShop.Longitude = newData[i].Longitude;

                    AddedWellcomeIGeoComList.Add(AddedShop);
                }

            }
            Console.WriteLine(AddedWellcomeIGeoComList.Count);
            return AddedWellcomeIGeoComList;
        }

        public List<IGeoComDeltaModel> FindRemoved(List<IGeoComModel> previousData, List<IGeoComGrabModel> newData)
        {
            int newDataLength = newData.Count;
            int previousDataLength = previousData.Count;
            List<IGeoComDeltaModel> RemovedWellcomeIGeoComList = new List<IGeoComDeltaModel>();

            for (int i = 0; i < previousDataLength; i++)
            {
                int j;
                for (j = 0; j < newDataLength; j++)

                    if (previousData[i].E_Address?.Replace(" ", "") == newData[j].E_Address?.Replace(" ", "") |
                       previousData[i].Tel_No?.Replace(" ", "") == newData[j].Tel_No?.Replace(" ", "") |
                       previousData[i].C_Address?.Replace(" ", "") == newData[j].C_Address?.Replace(" ", "")
                        )
                        break;
                if (j == newDataLength)
                {
                    IGeoComDeltaModel RemovedShop = new IGeoComDeltaModel();
                    RemovedShop.status = "removed";
                    RemovedShop.GeoNameId = previousData[i].GeoNameId;
                    RemovedShop.EnglishName = previousData[i].EnglishName;
                    RemovedShop.ChineseName = previousData[i].ChineseName;
                    RemovedShop.Class = previousData[i].Class;
                    RemovedShop.Type = previousData[i].Type;
                    RemovedShop.Subcat = previousData[i].Subcat;
                    RemovedShop.Easting = previousData[i].Easting;
                    RemovedShop.Northing = previousData[i].Northing;
                    RemovedShop.Source = previousData[i].Source;
                    RemovedShop.E_floor = previousData[i].E_floor;
                    RemovedShop.C_floor = previousData[i].C_floor;
                    RemovedShop.E_sitename = previousData[i].E_sitename;
                    RemovedShop.C_sitename = previousData[i].C_sitename;
                    RemovedShop.E_area = previousData[i].E_area;
                    RemovedShop.C_area = previousData[i].C_area;
                    RemovedShop.C_District = previousData[i].C_District;
                    RemovedShop.E_District = previousData[i].E_District;
                    RemovedShop.E_Region = previousData[i].E_Region;
                    RemovedShop.C_Region = previousData[i].C_Region;
                    RemovedShop.E_Address = previousData[i].E_Address;
                    RemovedShop.C_Address = previousData[i].C_Address;
                    RemovedShop.Tel_No = previousData[i].Tel_No;
                    RemovedShop.Fax_No = previousData[i].Fax_No;
                    RemovedShop.Web_Site = previousData[i].Web_Site;
                    RemovedShop.Rev_Date = previousData[i].Rev_Date;

                    RemovedWellcomeIGeoComList.Add(RemovedShop);
                }


            }
            Console.WriteLine(RemovedWellcomeIGeoComList.Count);
            return RemovedWellcomeIGeoComList;
        }

        public List<IGeoComGrabModel> LeftIntersection(List<IGeoComGrabModel> newData, List<IGeoComDeltaModel> added)
        {
            int newDataLength = newData.Count;
            int addedLength = added.Count;
            List<IGeoComGrabModel> LeftIntersectionIGeoComList = new List<IGeoComGrabModel>();

            for (int i = 0; i < newDataLength; i++)
            {
                int j;
                for (j = 0; j < addedLength; j++)

                    if (newData[i].E_Address?.Replace(" ", "") == added[j].E_Address?.Replace(" ", "") |
                       newData[i].Tel_No?.Replace(" ", "") == added[j].Tel_No?.Replace(" ", "") |
                       newData[i].C_Address?.Replace(" ", "") == added[j].C_Address?.Replace(" ", "")
                        )
                        break;
                if (j == addedLength)
                {

                    LeftIntersectionIGeoComList.Add(newData[i]);
                }

            }
            return LeftIntersectionIGeoComList;
        }

        public List<IGeoComModel> RightIntersection(List<IGeoComModel> previousData, List<IGeoComDeltaModel> removed)
        {
            int previousLength = previousData.Count;
            int removedLength = removed.Count;
            List<IGeoComModel> RightIntersectionIGeoComList = new List<IGeoComModel>();

            for (int i = 0; i < previousLength; i++)
            {
                int j;
                for (j = 0; j < removedLength; j++)

                    if (previousData[i].E_Address?.Replace(" ", "") == removed[j].E_Address?.Replace(" ", "") |
                       previousData[i].Tel_No?.Replace(" ", "") == removed[j].Tel_No?.Replace(" ", "") |
                       previousData[i].C_Address?.Replace(" ", "") == removed[j].C_Address?.Replace(" ", "")
                        )
                        break;
                if (j == removedLength)
                {

                    RightIntersectionIGeoComList.Add(previousData[i]);
                }

            }
            return RightIntersectionIGeoComList;
        }

        public List<IGeoComDeltaModel> newModified(List<IGeoComGrabModel> left, List<IGeoComModel> right)
        {
            int leftLength = left.Count;
            int rightLength = right.Count;
            List<IGeoComDeltaModel> ModifiedIGeoComList = new List<IGeoComDeltaModel>();

            for (int i = 0; i < leftLength; i++)
            {
                int j;
                for (j = 0; j < rightLength; j++)

                    if (left[i].E_Address?.Replace(" ", "") == right[j].E_Address?.Replace(" ", "") &&
                       left[i].Tel_No?.Replace(" ", "") == right[j].Tel_No?.Replace(" ", "") &&
                       left[i].C_Address?.Replace(" ", "") == right[j].C_Address?.Replace(" ", "")
                        )
                        break;
                if (j == rightLength)
                {
                    IGeoComDeltaModel AddedShop = new IGeoComDeltaModel();
                    AddedShop.status = "new_modified";
                    AddedShop.EnglishName = left[i].EnglishName;
                    AddedShop.ChineseName = left[i].ChineseName;
                    AddedShop.Class = left[i].Class;
                    AddedShop.Type = left[i].Type;
                    AddedShop.E_Address = left[i].E_Address;
                    AddedShop.C_Address = left[i].C_Address;
                    AddedShop.Tel_No = left[i].Tel_No;
                    AddedShop.Web_Site = left[i].Web_Site;
                    AddedShop.Latitude = left[i].Latitude;
                    AddedShop.Longitude = left[i].Longitude;
                    ModifiedIGeoComList.Add(AddedShop);
                }

            }
            return ModifiedIGeoComList;
        }

        public List<IGeoComDeltaModel> orgModified(List<IGeoComModel> previousData, List<IGeoComGrabModel> right)
        {
            int leftLength = previousData.Count;
            int rightLength = right.Count;
            List<IGeoComDeltaModel> ModifiedIGeoComList = new List<IGeoComDeltaModel>();

            for (int i = 0; i < leftLength; i++)
            {
                int j;
                for (j = 0; j < rightLength; j++)

                    if (previousData[i].E_Address?.Replace(" ", "") == right[j].E_Address?.Replace(" ", "") &&
                       previousData[i].Tel_No?.Replace(" ", "") == right[j].Tel_No?.Replace(" ", "") &&
                       previousData[i].C_Address?.Replace(" ", "") == right[j].C_Address?.Replace(" ", "")
                        )
                        break;
                if (j == rightLength)
                {
                    IGeoComDeltaModel RemovedShop = new IGeoComDeltaModel();
                    RemovedShop.status = "org_modified";
                    RemovedShop.GeoNameId = previousData[i].GeoNameId;
                    RemovedShop.EnglishName = previousData[i].EnglishName;
                    RemovedShop.ChineseName = previousData[i].ChineseName;
                    RemovedShop.Class = previousData[i].Class;
                    RemovedShop.Type = previousData[i].Type;
                    RemovedShop.Subcat = previousData[i].Subcat;
                    RemovedShop.Easting = previousData[i].Easting;
                    RemovedShop.Northing = previousData[i].Northing;
                    RemovedShop.Source = previousData[i].Source;
                    RemovedShop.E_floor = previousData[i].E_floor;
                    RemovedShop.C_floor = previousData[i].C_floor;
                    RemovedShop.E_sitename = previousData[i].E_sitename;
                    RemovedShop.C_sitename = previousData[i].C_sitename;
                    RemovedShop.E_area = previousData[i].E_area;
                    RemovedShop.C_area = previousData[i].C_area;
                    RemovedShop.C_District = previousData[i].C_District;
                    RemovedShop.E_District = previousData[i].E_District;
                    RemovedShop.E_Region = previousData[i].E_Region;
                    RemovedShop.C_Region = previousData[i].C_Region;
                    RemovedShop.E_Address = previousData[i].E_Address;
                    RemovedShop.C_Address = previousData[i].C_Address;
                    RemovedShop.Tel_No = previousData[i].Tel_No;
                    RemovedShop.Fax_No = previousData[i].Fax_No;
                    RemovedShop.Web_Site = previousData[i].Web_Site;
                    RemovedShop.Rev_Date = previousData[i].Rev_Date;

                    ModifiedIGeoComList.Add(RemovedShop);
                }

            }
            return ModifiedIGeoComList;
        }

        public List<IGeoComDeltaModel> MergeResults(List<IGeoComDeltaModel> added, List<IGeoComDeltaModel> removed, List<IGeoComDeltaModel> newDelta, List<IGeoComDeltaModel> orgDelta)
        {
            List<IGeoComDeltaModel> DeltaChange = new List<IGeoComDeltaModel>();
            List<IGeoComDeltaModel> mergeAddedAndRemoved = added.Concat(removed).ToList();

            foreach (var (v, i) in newDelta.Select((v, i) => (v, i)))
            {
                DeltaChange.Add(v);
                foreach (var item in orgDelta)
                {
                    if (v.E_Address?.Replace(" ", "") == item.E_Address?.Replace(" ", "") |
                       v.Tel_No?.Replace(" ", "") == item.Tel_No?.Replace(" ", "") |
                       v.C_Address?.Replace(" ", "") == item.C_Address?.Replace(" ", "")
                        )
                    {
                        DeltaChange.Add(item);
                    }
                }
            }
            var results = mergeAddedAndRemoved.Concat(DeltaChange).ToList();
            /*
            foreach(var result in results)
            {
                result?.E_Address?.Replace(",", "");
                result?.C_Address?.Replace(",", "");
            }
            */
            Console.WriteLine(results.Count);
            return results;
        }

    }

}
