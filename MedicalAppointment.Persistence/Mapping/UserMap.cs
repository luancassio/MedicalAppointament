using MedicalAppointment.Domain.Entity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalAppointment.Persistence.Mapping;
public class UserMap : IEntityTypeConfiguration<User> {
    public void Configure(EntityTypeBuilder<User> builder) {
        builder.ToTable("user");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).IsRequired();
        builder.Property(c => c.Create_At).IsRequired();
        builder.Property(c => c.Update_At).IsRequired(false);
        builder.Property(c => c.Email).HasMaxLength(128).IsRequired();
        builder.Property(c => c.Password).IsRequired();
        builder.Property(c => c.UserName).IsRequired();
        builder.Property(c => c.Name).IsRequired();



        //builder.HasMany(tr => tr.Transformers)
        //       .WithOne(u => u.User)
        //       .HasForeignKey(tr => tr.UserId);
        //.OnDelete(DeleteBehavior.Cascade);

    }
}
