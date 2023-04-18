using AutoMapper;
using gBanker.Data.CodeFirstMigration;
using gBanker.Data.CodeFirstMigration.Db;
using gBanker.Data.DBDetailModels;
using gBanker.Web.Models;
using gBanker.Web.ViewModels;
using gBankerCodeFirstMigration.Db;
using System;
using System.Collections.Generic;

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
                .ForMember(dest => dest.PhoneNo, source => source.MapFrom(src => src.Phone))
                .ForMember(dest => dest.PortalMemberId, option => option.MapFrom(src => src.Id));
            Mapper.CreateMap<PortalMember, MemberViewModel>()
                .ForMember(dest => dest.IsPortalMember, source => source.MapFrom(s => true))
                .ForMember(dest => dest.PortalMemberId, option => option.MapFrom(src => src.Id))
                .ForMember(dest => dest.BirthDate, option => option.MapFrom(src => src.DOB))
                .ForMember(dest => dest.PhoneNo, option => option.MapFrom(src => src.Phone))
                .ForMember(dest => dest.ZipCode, option => option.MapFrom(src => src.PostCode))
                .ForMember(dest => dest.PerDivisionCode, option => option.MapFrom(src => src.DivisionCode))
                .ForMember(dest => dest.PerDistrictCode, option => option.MapFrom(src => src.DistrictCode))
                .ForMember(dest => dest.PerUpozillaCode, option => option.MapFrom(src => src.UpozillaCode))
                .ForMember(dest => dest.PerUnionCode, option => option.MapFrom(src => src.UnionCode))
                .ForMember(dest => dest.PerVillageCode, option => option.MapFrom(src => src.VillageCode))
                .ForMember(dest => dest.Education, option => option.MapFrom(src => src.EducationQualification))
                .ForMember(dest => dest.PerZipCode, option => option.MapFrom(src => src.PostCode))
                .ForMember(dest => dest.EconomicActivity, option => option.MapFrom(src => src.Occupation))
                .ForMember(dest => dest.AsOnDateAge, option => option.MapFrom(src => src.MemberAge))
                .ForMember(dest => dest.PlaceOfBirth, option => option.MapFrom(src => src.DistrictCode))
                .ForMember(dest => dest.RefereeName, option => option.MapFrom(src => src.SpouseName))
                .ForMember(dest => dest.PerAddressLine1, option => option.MapFrom(src => src.Address));

            Mapper.CreateMap<PortalLoanSummary, LoanApprovalViewModel>()
                .ForMember(dest => dest.MemberCode, option => option.MapFrom(
                    src =>
                    src.MemberID.ToString() +'-' + src.Member.FirstName + '_' + src.Member.LastName))
                .ForMember(dest => dest.OfficeCode, option => option.MapFrom( src =>src.OfficeID.ToString() + '-' + src.Office.OfficeName ))
                .ForMember(dest => dest.CenterCode, option => option.MapFrom( src =>src.CenterID.ToString() + '-' + src.Center.CenterName ))
                .ForMember(dest => dest.ProductCode, option => option.MapFrom( src =>src.ProductID.ToString() + '-' + src.Product.ProductName ))
                .ForMember(dest => dest.PortalLoanSummaryID, option => option.MapFrom(src => src.PortalLoanSummaryID));

            Mapper.CreateMap<LoanAccReschedule, LoanAccRescheduleViewModel>()
                .ForMember(dest => dest.MemberCode, option => option.MapFrom(src => src.MemberID.ToString() + '-' + src.Member.FirstName + '-' + src.Member.LastName));
                //.ForMember(dest => dest.OfficeCode, option => option.MapFrom(src => src.OfficeID.ToString() + '-' + src.Office.OfficeName));
                 
            Mapper.CreateMap<SavingsAccClose, SavingsAccCloseViewModel>()
                .ForMember(dest => dest.MemberCode, option => option.MapFrom(src => src.MemberID.ToString() + '-' + src.Member.FirstName + '-' + src.Member.LastName));
            //.ForMember(dest => dest.OfficeCode, option => option.MapFrom(src => src.OfficeID.ToString() + '-' + src.Office.OfficeName));

            Mapper.CreateMap<PortalSavingSummary, SpecialSavingCollectionViewModel>()
                .ForMember(dest => dest.OfficeCode, option => option.MapFrom(src => src.OfficeID))
                .ForMember(dest => dest.MemberCode, option => option.MapFrom(src => src.Member.MemberCode))
                .ForMember(dest => dest.MemberName, option => option.MapFrom(src => src.Member.FirstName + " " + src.Member.LastName))
                .ForMember(dest => dest.ProductCode, option => option.MapFrom(src => src.Product.ProductCode))
                .ForMember(dest => dest.ProductName, option => option.MapFrom(src => src.Product.ProductName))
                .ForMember(dest => dest.CenterCode, option => option.MapFrom(src => src.Center.CenterCode));

            Mapper.CreateMap<PortalLoanSummary, SpecialLoanCollectionViewModel>()
                .ForMember(dest => dest.GuarantorNID, option => option.MapFrom(src => src.GuarantorNIDId))
                .ForMember(dest => dest.GuarantorImg, option => option.MapFrom(src => src.GuarantorImgId));


            Mapper.CreateMap<Member, PortalMember>()
                .ForMember(dest => dest.Address, option => option.MapFrom(src => src.AddressLine1))
                .ForMember(dest => dest.DOB, option => option.MapFrom(src => src.BirthDate))
                .ForMember(dest => dest.EducationQualification, option => option.MapFrom(src => src.Education))
                .ForMember(dest => dest.MemberAge, option => option.MapFrom(src => Int16.Parse(src.AsOnDateAge.Substring(0,2))))
                .ForMember(dest => dest.Occupation, option => option.MapFrom(src => src.EconomicActivity))
                .ForMember(dest => dest.Phone, option => option.MapFrom(src => src.PhoneNo))
                .ForMember(dest => dest.Photo, option => option.MapFrom(src => src.Image))
                .ForMember(dest => dest.PostCode, option => option.MapFrom(src => src.ZipCode))
                .ForMember(dest => dest.SpouseName, option => option.MapFrom(src => src.SpouseName));

            //Mapper.CreateMap<LoanAccReschedule, LoanAccRescheduleViewModel>()
            //    .ForMember(dest => dest.MemberCode, option => option
            //    .MapFrom(src => src.MemberID.ToString() + '-' + src.Member.MemberCode + '_' + src.Member.LastName));

            //.ForMember(dest => dest.OfficeCode, option => option
            //.MapFrom(src => src.OfficeID.ToString() + '-' + src.Office.OfficeName));

        }
    }
}