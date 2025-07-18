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
        builder.Property(x=>x.Username).HasColumnName("username");
        builder.Property(x=>x.PasswordHash).HasColumnName("password_hash");

        builder.HasMany(x => x.UserRoles)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId);
    }
}

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("role","System");
        builder.HasKey(x=>x.Id);
        
        builder.Property(x=>x.Id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(x=>x.Name).HasColumnName("name");

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
        builder.Property(x=>x.Key).HasColumnName("key");
        builder.Property(x=>x.Description).HasColumnName("description");
        
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
        
        builder.Property(x=>x.RoleId).HasColumnName("role_id");
        builder.Property(x=>x.PermissionId).HasColumnName("permission_id");
        
        builder.HasOne(x=>x.Role)
            .WithMany(x=>x.RolePermissions)
            .HasForeignKey(x=>x.RoleId);
        
        builder.HasOne(x=>x.Permission)
            .WithMany(x=>x.RolePermissions)
            .HasForeignKey(x=>x.PermissionId);
    }
}
public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("user_role","System");
        builder.HasKey(x=>new {x.RoleId, x.UserId});
        
        builder.Property(x=>x.UserId).HasColumnName("user_id");
        builder.Property(x=>x.RoleId).HasColumnName("role_id");
        
        builder.HasOne(x=>x.Role)
            .WithMany(x=>x.UserRoles)
            .HasForeignKey(x=>x.RoleId);
        
        builder.HasOne(x=>x.User)
            .WithMany(x=>x.UserRoles)
            .HasForeignKey(x=>x.UserId);
        
    }
}