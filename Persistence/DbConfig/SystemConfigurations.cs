using Domain.Entities.System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DbConfig;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("user","System");
        builder.HasKey(x=>x.Id);
        
        builder.Property(x=>x.Id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(x=>x.UserName).HasColumnName("username").IsRequired();
        builder.Property(x=>x.PasswordHash).HasColumnName("password_hash").IsRequired();
        builder.Property(x=>x.Email).HasColumnName("email");
        builder.Property(x=>x.DisplayName).HasColumnName("display_name");

        builder.HasOne(x => x.UserRole)
            .WithOne(x => x.User);
    }
}

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("role","System");
        builder.HasKey(x=>x.Id);
        
        builder.Property(x=>x.Id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(x=>x.Name).HasColumnName("name").IsRequired();
        builder.Property(x=>x.ParentId).HasColumnName("parent_id");


        builder.HasOne(x => x.Parent);
        builder.HasMany(x => x.RolePermissions)
            .WithOne(x => x.Role)
            .HasForeignKey(x => x.RoleId);
    }
}
public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("permission","System");
        builder.HasKey(x=>x.Id);
        
        builder.Property(x=>x.Id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(x=>x.Key).HasColumnName("key").IsRequired();
        builder.Property(x=>x.Description).HasColumnName("description");
        builder.Property(x=>x.ParentId).HasColumnName("parent_id");
        
        builder.HasOne(x => x.Parent);
        
        builder.HasMany(x=>x.RolePermissions)
            .WithOne(x=>x.Permission)
            .HasForeignKey(x=>x.PermissionId);
    }
}
public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable("role_permission","System");
        builder.HasKey(x=> new {x.RoleId,x.PermissionId});
        
        builder.Property(x=>x.RoleId).HasColumnName("role_id").IsRequired();
        builder.Property(x=>x.PermissionId).HasColumnName("permission_id").IsRequired();
        
    }
}
public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("user_role","System");
        builder.HasKey(x=>new {x.RoleId, x.UserId});
        
        builder.Property(x=>x.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(x=>x.RoleId).HasColumnName("role_id").IsRequired();
        
        builder.HasOne(x=>x.Role)
            .WithMany(x=>x.UserRoles)
            .HasForeignKey(x=>x.RoleId);
        
        builder.HasOne(x=>x.User)
            .WithOne(x=>x.UserRole);
        
    }
}