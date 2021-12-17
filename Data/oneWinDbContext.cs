using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using oneWin.Models.baseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace oneWin.Data
{
    public class oneWinDbContext: DbContext
    {
        public oneWinDbContext(DbContextOptions<oneWinDbContext> options)
           : base(options)
        {
            
        }
        public DbSet<registrationModel> Registration { get; set; }

        public DbSet<SuspendedDocRegistrieModel> SuspendedDocRegistries { get; set; }

        public DbSet<areaModel> Areas { get; set; }

        public DbSet<curatorModel> Curators { get; set; }

        public DbSet<departmentModel> Departments { get; set; }

        public DbSet<performerModel> Performers { get; set; }

        public DbSet<headModel> Heads { get; set; }

        public DbSet<sectionsModel> Sections { get; set; }

        public DbSet<docRegModel> DocRegistry { get; set; }

        public DbSet<siteCostModel> ViewSiteCost { get; set; }

        public DbSet<siteValidatyModel> ViewSiteValidaty { get; set; }

        public DbSet<siteSectionsModel> ViewSiteSections { get; set; }

        public DbSet<siteInssueModel> ViewSiteInssue { get; set; }

        public DbSet<siteDocRegComentModel> ViewSiteDocRegComent { get; set; }

        public DbSet<zaprDocModel> ZaprDocs { get; set; }

        public DbSet<tempModel> Temp { get; set; }
                
        public DbSet<streetModel> StreetNames { get; set; }
                
        public DbSet<solutionModel> Solutions { get; set; }

        public DbSet<sogOrgModel> SoglOrg { get; set; }

        public DbSet<soglasovaniyaModel> Soglasovaniya { get; set; }

        public DbSet<settingModel> Settings { get; set; }

        public DbSet<selectZaprDocsModel> SelectZaprDocs { get; set; }

        public DbSet<selectDocsModel> SelectDocs { get; set; }

        public DbSet<rvcSuvictipModel> RVC_SULICTIP { get; set; }

        public DbSet<rvcSulicModel> RVC_SULIC { get; set; }
                
        public DbSet<responsibleDocRegistryModel> ResponsibleDocRegistry { get; set; }

        public DbSet<performersDocRegistryModel> PerformersDocRegistry { get; set; }

        public DbSet<paymentAccountModel> PaymentAccount { get; set; }

        public DbSet<orgsZaprModel> OrgsZapr { get; set; }

        public DbSet<normDocModel> NormDoc { get; set; }

        public DbSet<msgModel> MSG { get; set; }

        public DbSet<mfosModel> MFOS { get; set; }

        public DbSet<infoFlatModel> InfoFlat { get; set; }

        public DbSet<familyModel> Family { get; set; }
                
        public DbSet<documentAcceptModel> DocumentAccept { get; set; }

        public DbSet<docsModel> Docs { get; set; }
                
        public DbSet<bufSoglDocModel> BufSoglDoc { get; set; }

        public DbSet<bufPaymentAccountModel> BufPaymentAccount { get; set; }

        public DbSet<bufOrgsZAprModel> BufOrgsZApr { get; set; }


        public DbSet<bufNormDocModel> BufNormDoc { get; set; }

        public DbSet<bufDocRegistryModel> BufDocRegistry { get; set; }

        public DbSet<attachedFileModel> AttachedFile { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);


            //добовляем генерацию ключа guid
            builder.Entity<registrationModel>().Property(b => b.RegistrationID).HasDefaultValueSql("newid()");
            builder.Entity<SuspendedDocRegistrieModel>().Property(b => b.Id).HasDefaultValueSql("newid()");
            builder.Entity<areaModel>().Property(b => b.Id).HasDefaultValueSql("newid()");
            builder.Entity<curatorModel>().Property(b => b.Id).HasDefaultValueSql("newid()");
            builder.Entity<departmentModel>().Property(b => b.Id).HasDefaultValueSql("newid()");
            builder.Entity<performerModel>().Property(b => b.Id).HasDefaultValueSql("newid()");
            builder.Entity<headModel>().Property(b => b.HedID).HasDefaultValueSql("newid()");
            builder.Entity<sectionsModel>().Property(b => b.SectionID).HasDefaultValueSql("newid()");
            builder.Entity<docRegModel>().Property(b => b.RegID).HasDefaultValueSql("newid()");
            builder.Entity<zaprDocModel>().Property(b => b.ZaprDocID).HasDefaultValueSql("newid()");
            builder.Entity<solutionModel>().Property(b => b.Id).HasDefaultValueSql("newid()");
            builder.Entity<sogOrgModel>().Property(b => b.SoglOrgID).HasDefaultValueSql("newid()");
            builder.Entity<soglasovaniyaModel>().Property(b => b.SoglID).HasDefaultValueSql("newid()");
            builder.Entity<selectZaprDocsModel>().Property(b => b.Id).HasDefaultValueSql("newid()");
            builder.Entity<selectDocsModel>().Property(b => b.Id).HasDefaultValueSql("newid()");
             builder.Entity<orgsZaprModel>().Property(b => b.OrgZaprID).HasDefaultValueSql("newid()");
            builder.Entity<normDocModel>().Property(b => b.NormID).HasDefaultValueSql("newid()");
            builder.Entity<msgModel>().Property(b => b.MsgId).HasDefaultValueSql("newid()");
            builder.Entity<mfosModel>().Property(b => b.Id).HasDefaultValueSql("newid()");
            builder.Entity<infoFlatModel>().Property(b => b.Id).HasDefaultValueSql("newid()");
            builder.Entity<familyModel>().Property(b => b.FamilyID).HasDefaultValueSql("newid()");
            builder.Entity<docsModel>().Property(b => b.DocID).HasDefaultValueSql("newid()");           
            builder.Entity<attachedFileModel>().Property(b => b.Id).HasDefaultValueSql("newid()");
            builder.Entity<bufDocRegistryModel>().HasKey(b =>new { b.DocID, b.RegID } );
            builder.Entity<bufOrgsZAprModel>().HasKey(b => new { b.ZaprDocID, b.RegID });
            builder.Entity<bufNormDocModel>().HasKey(b => new { b.NormID, b.RegID });
            builder.Entity<bufSoglDocModel>().HasKey(b => new { b.SoglID, b.RegID });
            builder.Entity<responsibleDocRegistryModel>().HasKey(b => new { b.DocRegistry_Id, b.Performers_Id });
            builder.Entity<performersDocRegistryModel>().HasKey(b => new { b.DocRegistry_Id, b.Performers_Id });
            builder.Entity<documentAcceptModel>().HasKey(b => new { b.DocRegistry_Id, b.Performers_Id });
            builder.Entity<bufPaymentAccountModel>().HasKey(b => new { b.IdDoc, b.IdPayment });
        }

    }
}
