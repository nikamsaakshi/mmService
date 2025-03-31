using Microsoft.EntityFrameworkCore;

namespace mmService.Entities
{
    public partial class DBContext : DbContext
    {
        public DBContext()
        {
        }

        public DBContext(DbContextOptions<DBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<CandidateProfile> CandidateProfile { get; set; }
        public virtual DbSet<Candidate> Candidate { get; set; }
        public virtual DbSet<CandidatePhotos> CandidatePhotos { get; set; }
        public virtual DbSet<CandidateInterest> CandidateInterest { get; set; }
        public virtual DbSet<MarriedCandidate> MarriedCandidate { get; set; }
        public virtual DbSet<CandidateChat> CandidateChat { get; set; }
        public virtual DbSet<CandidateChatMedia> CandidateChatMedia { get; set; }
        public virtual DbSet<CandidateChatconversation> CandidateChatconversation { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CandidateInterest>(entity =>
                {
                    entity.ToTable("candidateinterest").HasKey("id");

                    entity.Property(e => e.id);
                    entity.Property(e => e.candidateId)
                        .IsRequired()
                        .IsUnicode(false);

                    entity.Property(e => e.interestedCandidateId)
                        .IsUnicode(false);

                    entity.Property(e => e.isAccepted)
                        .IsUnicode(false);

                    entity.Property(e => e.isRejected)
                        .IsUnicode(false);
                });

            modelBuilder.Entity<CandidatePhotos>(entity =>
            {
                entity.ToTable("candidatephotos").HasKey("id");

                entity.Property(e => e.id);

                entity.Property(e => e.candidateId)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.photoPath)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.uploadedAt)
                    .IsRequired()
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Candidate>(entity =>
            {
                entity.ToTable("candidate").HasKey("id");

                entity.Property(e => e.id);

                entity.Property(e => e.emailId)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.password)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.aadhaarNumber)
                    .IsRequired()
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.mobileNumber)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.isPremium)
                .IsRequired()
                .IsUnicode(false);
            });


            modelBuilder.Entity<CandidateProfile>(entity =>
            {
                entity.ToTable("candidateprofile").HasKey("candidateId");

                entity.Property(e => e.candidateId);

                entity.Property(e => e.firstName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.middleName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.lastName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.DOB)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.gender)
                    .IsRequired()
                    .HasMaxLength(6)
                    .IsUnicode(false);

                entity.Property(e => e.addressLine1)

                   .HasMaxLength(100)
                   .IsUnicode(false);

                entity.Property(e => e.addressLine2)

                   .HasMaxLength(100)
                   .IsUnicode(false);

                entity.Property(e => e.taluka)
                   .IsRequired()
                   .HasMaxLength(100)
                   .IsUnicode(false);

                entity.Property(e => e.district)
                   .IsRequired()
                   .HasMaxLength(100)
                   .IsUnicode(false);

                entity.Property(e => e.pincode)
                   .IsRequired()
                   .HasMaxLength(6)
                   .IsUnicode(false);

                entity.Property(e => e.villageOrCity)
                   .IsRequired()
                   .HasMaxLength(100)
                   .IsUnicode(false);

                entity.Property(e => e.religion)
                  .IsRequired()
                  .HasMaxLength(50)
                  .IsUnicode(false);


                entity.Property(e => e.cast)
                   .IsRequired()
                   .HasMaxLength(50)
                   .IsUnicode(false);


                entity.Property(e => e.subCast)

                   .HasMaxLength(50)
                   .IsUnicode(false);


                entity.Property(e => e.familyDeity)
                   .HasMaxLength(50)
                   .IsUnicode(false);

                entity.Property(e => e.deityRepresentation)
                   .HasMaxLength(50)
                   .IsUnicode(false);

                entity.Property(e => e.constellation)
                   .HasMaxLength(50)
                   .IsUnicode(false);

                entity.Property(e => e.zodiacSign)
                  .HasMaxLength(50)
                  .IsUnicode(false);

                entity.Property(e => e.category)
                 .HasMaxLength(50)
                 .IsUnicode(false);


                entity.Property(e => e.pulse)
                 .HasMaxLength(50)
                 .IsUnicode(false);


                entity.Property(e => e.height)

                   .IsUnicode(false);

                entity.Property(e => e.weight)

                   .IsUnicode(false);

                entity.Property(e => e.complexion)

                   .IsUnicode(false);

                entity.Property(e => e.bloodGroup)
                   .IsUnicode(false);

                entity.Property(e => e.education)

                   .IsUnicode(false);

                entity.Property(e => e.profession)
                 .HasMaxLength(50)
                 .IsUnicode(false);

                entity.Property(e => e.annualIncome)

                   .HasMaxLength(50)
                   .IsUnicode(false);

                entity.Property(e => e.property)

                  .HasMaxLength(50)
                  .IsUnicode(false);

                entity.Property(e => e.familyBackground)
                  .IsUnicode(false);

            });

            modelBuilder.Entity<MarriedCandidate>(entity =>
            {
                entity.ToTable("marriedcandidate").HasKey("id");

                entity.Property(e => e.id);

                entity.Property(e => e.candidate1)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.candidate2)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.marriedDate)
                    .IsRequired()
                    .IsUnicode(false);
            });


            modelBuilder.Entity<CandidateChat>(entity =>
            {
                entity.ToTable("CandidateChat").HasKey("chatId");

                entity.Property(e => e.chatId)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.senderId)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.reciverId)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.message)
                    .IsRequired()
                    .IsUnicode(false);

                //entity.Property(e => e.messageType)
                //    .IsRequired()
                //    .IsUnicode(false);

                //entity.Property(e => e.sentAt)
                //    .IsRequired()
                //    .IsUnicode(false);

                entity.Property(e => e.isRead)
                    .IsRequired()
                    .IsUnicode(false);

            });

            modelBuilder.Entity<CandidateChatMedia>(static entity =>
            {
                entity.ToTable("CandidateChatMedia").HasKey("madiaId");

                entity.Property(e => e.madiaId)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.chatId)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.filePath)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(static e => e.fileType)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.uploadedAt)
                    .IsRequired()
                    .IsUnicode(false);
            });


            modelBuilder.Entity<CandidateChatconversation>(static entity =>
            {
                entity.ToTable("CandidateChatconversation").HasKey("conversationId");

                entity.Property(e => e.conversationId)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.candidate1Id)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.candidate2Id)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(static e => e.lastMessageId)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.lastUpdated)
                    .IsRequired()
                    .IsUnicode(false);
            });
            modelBuilder.Entity<CandidateProfile>().Ignore(p => p.doc);
            modelBuilder.Entity<CandidateProfile>().Ignore(p => p.image);
            modelBuilder.Entity<CandidateProfile>().Ignore(p => p.photoPath);
            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
