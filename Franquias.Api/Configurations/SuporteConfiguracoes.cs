using Franquias.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Franquias.Api.Configurations;

/// <summary>
/// Mapeamento dos chamados abertos pelas unidades.
/// </summary>
public sealed class ChamadoSuporteConfiguracao : IEntityTypeConfiguration<ChamadoSuporte>
{
    public void Configure(EntityTypeBuilder<ChamadoSuporte> entidade)
    {
        entidade.ToTable("ChamadosSuporte");

        entidade.HasKey(chamado => chamado.Id);
        entidade.Property(chamado => chamado.Id).ValueGeneratedOnAdd();

        entidade.Property(chamado => chamado.Protocolo).HasMaxLength(20).IsRequired();
        entidade.Property(chamado => chamado.Titulo).HasMaxLength(150).IsRequired();
        entidade.Property(chamado => chamado.Descricao).HasMaxLength(2000).IsRequired();
        entidade.Property(chamado => chamado.SolucaoAplicada).HasMaxLength(2000);

        entidade.Property(chamado => chamado.Categoria)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        entidade.Property(chamado => chamado.Prioridade)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        entidade.Property(chamado => chamado.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        entidade.Property(chamado => chamado.AbertoEm).IsRequired();

        entidade.HasIndex(chamado => chamado.Protocolo).IsUnique();
        entidade.HasIndex(chamado => chamado.Status);
        entidade.HasIndex(chamado => chamado.Prioridade);

        entidade.HasOne(chamado => chamado.Unidade)
            .WithMany(unidade => unidade.Chamados)
            .HasForeignKey(chamado => chamado.UnidadeFranqueadaId)
            .OnDelete(DeleteBehavior.Cascade);

        entidade.HasOne(chamado => chamado.UsuarioAbertura)
            .WithMany()
            .HasForeignKey(chamado => chamado.UsuarioAberturaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

/// <summary>
/// Mapeamento do histórico de interações dos chamados.
/// </summary>
public sealed class ChamadoInteracaoConfiguracao : IEntityTypeConfiguration<ChamadoInteracao>
{
    public void Configure(EntityTypeBuilder<ChamadoInteracao> entidade)
    {
        entidade.ToTable("ChamadosInteracoes");

        entidade.HasKey(interacao => interacao.Id);
        entidade.Property(interacao => interacao.Id).ValueGeneratedOnAdd();

        entidade.Property(interacao => interacao.Mensagem).HasMaxLength(2000).IsRequired();

        entidade.Property(interacao => interacao.StatusRegistrado)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        entidade.Property(interacao => interacao.RegistradaEm).IsRequired();

        entidade.HasOne(interacao => interacao.Chamado)
            .WithMany(chamado => chamado.Interacoes)
            .HasForeignKey(interacao => interacao.ChamadoSuporteId)
            .OnDelete(DeleteBehavior.Cascade);

        entidade.HasOne(interacao => interacao.Usuario)
            .WithMany()
            .HasForeignKey(interacao => interacao.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
