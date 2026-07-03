using AutoMapper;
using DesafioControleGastos.Core.DTOs;
using DesafioControleGastos.Core.Models;

namespace DesafioControleGastos.Core.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Pessoa, PessoaResponseDTO>()
                .ForMember(dest => dest.IsMenorDeIdade, 
                    opt => opt.MapFrom(src => src.Idade < 18))
                .ForMember(dest => dest.TotalReceitas, 
                    opt => opt.MapFrom(src => 
                        src.Transacoes.Where(t => t.Tipo == TipoTransacao.Receita)
                                      .Sum(t => t.Valor)))
                .ForMember(dest => dest.TotalDespesas, 
                    opt => opt.MapFrom(src => 
                        src.Transacoes.Where(t => t.Tipo == TipoTransacao.Despesa)
                                      .Sum(t => t.Valor)));

            CreateMap<PessoaCreateDTO, Pessoa>();

            CreateMap<Transacao, TransacaoResponseDTO>()
                .ForMember(dest => dest.NomePessoa, 
                    opt => opt.MapFrom(src => src.Pessoa.Nome));

            CreateMap<TransacaoCreateDTO, Transacao>();

            CreateMap<Pessoa, TotalPorPessoaDTO>()
                .ForMember(dest => dest.PessoaId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.NomePessoa, opt => opt.MapFrom(src => src.Nome))
                .ForMember(dest => dest.TotalReceitas, opt => opt.MapFrom(src =>
                    src.Transacoes.Where(t => t.Tipo == TipoTransacao.Receita)
                                  .Sum(t => t.Valor)))
                .ForMember(dest => dest.TotalDespesas, opt => opt.MapFrom(src =>
                    src.Transacoes.Where(t => t.Tipo == TipoTransacao.Despesa)
                                  .Sum(t => t.Valor)));
        }
    }
}