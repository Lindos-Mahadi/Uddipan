using AutoMapper;
using gBanker.Data.CodeFirstMigration;
using gBanker.Data.CodeFirstMigration.Db;
using gBanker.Data.DBDetailModels;
using gBanker.Web.Models;
using gBanker.Web.ViewModels;
using gBankerCodeFirstMigration.Db;

namespace gBanker.Web.Mappings
{
    public class MemberPortalModelToUddipanModel : Profile
    {
        public override string ProfileName
        {
            get { return "ViewModelToDomainMappings"; }
        }

        protected override void Configure()
        {
            Mapper.CreateMap<PortalMember, Member>()
                .ForMember(dest => dest.IsPortalMember, source => source.NullSubstitute(true))
                .ForMember(dest => dest.PortalMemberId, option => option.MapFrom(src => src.Id));
            Mapper.CreateMap<PortalMember, DBMemberDetailModel>()
                .ForMember(dest => dest.IsPortalMember, source => source.MapFrom(s => true))
                .ForMember(dest => dest.PortalMemberId, option => option.MapFrom(src => src.Id));
            Mapper.CreateMap<PortalMember , MemberViewModel>()
                .ForMember(dest => dest.IsPortalMember, source => source.MapFrom(s => true))
                .ForMember(dest => dest.PortalMemberId, option => option.MapFrom(src => src.Id));

        }
    }
}