using iGeoComAPI.Options;
using Microsoft.Extensions.Configuration;

namespace iGeoComAPI

{
    public static class MyConfigServiceCollection
    {
        public static IServiceCollection AddConfig(
           this IServiceCollection services, IConfiguration config )
        {
            services.Configure<AeonOptions>(config.GetSection(AeonOptions.SectionName));
            services.Configure<ConnectionStringsOptions>(config.GetSection(ConnectionStringsOptions.SectionName));
            services.Configure<SevenElevenOptions>(config.GetSection(SevenElevenOptions.SectionName));
            services.Configure<WellcomeOptions>(config.GetSection(WellcomeOptions.SectionName));
            services.Configure<CaltexOptions>(config.GetSection(CaltexOptions.SectionName));
            services.Configure<ParknShopOptions>(config.GetSection(ParknShopOptions.SectionName));
            services.Configure<AeonOptions>(config.GetSection(AeonOptions.SectionName));
            services.Configure<VangoOptions>(config.GetSection(VangoOptions.SectionName));
            services.Configure<USelectOptions>(config.GetSection(USelectOptions.SectionName));
            services.Configure<CircleKOptions>(config.GetSection(CircleKOptions.SectionName));
            services.Configure<WmoovOptions>(config.GetSection(WmoovOptions.SectionName));
            services.Configure<AmbulanceDepotOptions>(config.GetSection(AmbulanceDepotOptions.SectionName));
            services.Configure<AromeNMaximsCakesOptions>(config.GetSection(AromeNMaximsCakesOptions.SectionName));
            services.Configure<BloodDonorCentreOptions>(config.GetSection(BloodDonorCentreOptions.SectionName));
            services.Configure<BmcpcOptions>(config.GetSection(BmcpcOptions.SectionName));
            services.Configure<CSLOptions>(config.GetSection(CSLOptions.SectionName));
            services.Configure<CatholicOrgOptions>(config.GetSection(CatholicOrgOptions.SectionName));
            services.Configure<CheungKongOptions>(config.GetSection(CheungKongOptions.SectionName));
            services.Configure<ChinaMobileOptions>(config.GetSection(ChinaMobileOptions.SectionName));
            services.Configure<NorthEastOptions>(config.GetSection(NorthEastOptions.SectionName));
            services.Configure<GoogleMapOptions>(config.GetSection(GoogleMapOptions.SectionName));
            services.Configure<LinkHkOptions>(config.GetSection(LinkHkOptions.SectionName));
            services.Configure<MarketPlaceOptions>(config.GetSection(MarketPlaceOptions.SectionName));
            return services;
        }
    }
}
