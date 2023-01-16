

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

        [Required]
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

        [Required]
        public string Email { get; set; }

        [Required]
        [StringLength(50)]
        public string Occupation { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        [StringLength(10)]
        public string Photo { get; set; }

        [Required]
        [StringLength(55)]
        public string Phone { get; set; }

        [StringLength(1)]
        public string Status { get; set; }

        [StringLength(50)]
        public string UpdateUser { get; set; }

        public DateTime? UpdateDate { get; set; }
        public bool? ApprovalStatus { get; set; }
        public int? MemberAge{ get; set; }
        public string EducationQualification { get; set; }
    }
}
