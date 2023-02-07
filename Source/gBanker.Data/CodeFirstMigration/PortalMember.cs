

namespace gBanker.Data.CodeFirstMigration
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    
    [Table("PortalMember")]
    public partial class PortalMember
    {
        public long Id { get; set; }

        [StringLength(20)]
        public string MemberCode { get; set; }

        public int OfficeID { get; set; }

        public int? CenterID { get; set; }

        public short? GroupID { get; set; }

        public DateTime? JoinDate { get; set; }

        [Required]
        [StringLength(7)]
        public string Gender { get; set; }

        public byte? MemberCategoryID { get; set; }

        [StringLength(2)]
        public string MemberStatus { get; set; }

        public int OrgID { get; set; }

        [StringLength(15)]
        public string CreateUser { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime CreateDate { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        [StringLength(200)]
        public string FatherName { get; set; }

        [StringLength(200)]
        public string MotherName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        [StringLength(50)]
        public string Occupation { get; set; }

        [Required]
        public string Address { get; set; }

        [StringLength(10)]
        public string Photo { get; set; }

        [Required]
        [StringLength(55)]
        public string Phone { get; set; }

        [StringLength(50)]
        public string DistrictCode { get; set; }

        [StringLength(50)]
        public string DivisionCode { get; set; }

        [StringLength(50)]
        public string UpozillaCode { get; set; }
        public string UnionCode { get; set; }
        public string VillageCode { get; set; }

        public int? CountryID { get; set; }

        public DateTime? DOB { get; set; }

        [StringLength(50)]
        public string PostCode { get; set; }

        [StringLength(1)]
        public string Status { get; set; }

        [StringLength(50)]
        public string UpdateUser { get; set; }

        public DateTime? UpdateDate { get; set; }

        public bool? ApprovalStatus { get; set; }

        public int? MemberAge { get; set; }

        [StringLength(50)]
        public string EducationQualification { get; set; }

        // ADDED NationalID
        [StringLength(20)]
        public string NationalID { get; set; }

        public long? MemberNID { get; set; }
        public long? Image { get; set; }
    }
}
