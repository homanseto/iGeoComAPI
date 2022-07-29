using iGeoComAPI.Options;
using Microsoft.Extensions.Configuration;

namespace iGeoComAPI

{
    public static class MyConfigServiceCollection
    {
        public static IServiceCollection AddConfig(
           this IServiceCollection services, IConfiguration config )
        {
            services.Configure<NorthEastOptions>(config.GetSection(NorthEastOptions.SectionName));
            services.Configure<GoogleMapOptions>(config.GetSection(GoogleMapOptions.SectionName));
            services.Configure<ConnectionStringsOptions>(config.GetSection(ConnectionStringsOptions.SectionName));
            services.Configure<AeonOptions>(config.GetSection(AeonOptions.SectionName));
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
            services.Configure<BloodDonorCentreOptions>(config.GetSection(BloodDonorCentreOptions.SectionName));
            services.Configure<BmcpcOptions>(config.GetSection(BmcpcOptions.SectionName));
            services.Configure<CatholicOrgOptions>(config.GetSection(CatholicOrgOptions.SectionName));
            services.Configure<CheungKongOptions>(config.GetSection(CheungKongOptions.SectionName));
            services.Configure<LinkHkOptions>(config.GetSection(LinkHkOptions.SectionName));
            services.Configure<MarketPlaceOptions>(config.GetSection(MarketPlaceOptions.SectionName));
            services.Configure<EssoOptions>(config.GetSection(EssoOptions.SectionName));
            services.Configure<ShellOptions>(config.GetSection(ShellOptions.SectionName));
            services.Configure<SinopecOptions>(config.GetSection(SinopecOptions.SectionName));
            services.Configure<EMSDOptions>(config.GetSection(EMSDOptions.SectionName));
            services.Configure<PetroChinaOptions>(config.GetSection(PetroChinaOptions.SectionName));
            services.Configure<HkMarketOptions>(config.GetSection(HkMarketOptions.SectionName));
            services.Configure<CitySuperOptions>(config.GetSection(CitySuperOptions.SectionName));
            services.Configure<FEHDOptions>(config.GetSection(FEHDOptions.SectionName));
            services.Configure<YataOptions>(config.GetSection(YataOptions.SectionName));
            services.Configure<ThreesixtyhkOptions>(config.GetSection(ThreesixtyhkOptions.SectionName));
            services.Configure<PeoplesPlaceOptions>(config.GetSection(PeoplesPlaceOptions.SectionName));
            services.Configure<WilsonParkingOptions>(config.GetSection(WilsonParkingOptions.SectionName));
            services.Configure<ChurchOptions>(config.GetSection(ChurchOptions.SectionName));
            services.Configure<ConsularOptions>(config.GetSection(ConsularOptions.SectionName));
            services.Configure<HousingOptions>(config.GetSection(HousingOptions.SectionName));
            services.Configure<FortuneMallsOptions>(config.GetSection(FortuneMallsOptions.SectionName));
            services.Configure<CorrectionalInstitutionOptions>(config.GetSection(CorrectionalInstitutionOptions.SectionName));
            return services;
        }
    }
}
