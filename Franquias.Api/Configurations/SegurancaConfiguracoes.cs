using Franquias.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Franquias.Api.Configurations;

/// <summary>
/// Mapeamento da entidade <see cref="Usuario"/> para a tabela Usuarios.
/// </summary>
public sealed class UsuarioConfiguracao : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> entidade)
    {
        entidade.ToTable("Usuarios");

        entidade.HasKey(usuario => usuario.Id);
        entidade.Property(usuario => usuario.Id).ValueGeneratedOnAdd();

        entidade.Property(usuario => usuario.Nome)
            .HasMaxLength(150)
            .IsRequired();

        entidade.Property(usuario => usuario.Email)
            .HasMaxLength(150)
            .IsRequired();

        entidade.Property(usuario => usuario.SenhaHash)
            .HasMaxLength(200)
            .IsRequired();

        entidade.Property(usuario => usuario.SenhaSalt)
            .HasMaxLength(200)
            .IsRequired();

        entidade.Property(usuario => usuario.Perfil)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        entidade.Property(usuario => usuario.Ativo).IsRequired();
        entidade.Property(usuario => usuario.CriadoEm).IsRequired();

        // Regra de negócio: não é permitido cadastrar dois usuários com o mesmo e-mail.
        entidade.HasIndex(usuario => usuario.Email).IsUnique();

        entidade.HasOne(usuario => usuario.Unidade)
            .WithMany(unidade => unidade.Usuarios)
            .HasForeignKey(usuario => usuario.UnidadeFranqueadaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
