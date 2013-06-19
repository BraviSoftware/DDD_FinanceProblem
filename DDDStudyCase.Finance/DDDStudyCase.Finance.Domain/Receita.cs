using System;
using System.Collections.Generic;
using System.Linq;

namespace DDDStudyCase.Finance.Domain
{
    public class Receita
    {
        public decimal ValorTotalReceita { get; private set; }
        
        private List<Despesa> _despesas;
        public IEnumerable<Despesa> Despesas
        {
            get
            {
                return _despesas;
            }
        }

        public Receita(decimal valorTotal)
        {
            _despesas = new List<Despesa>();
            ValorTotalReceita = valorTotal;
        }

        public void InserirDespesa(Despesa despesa)
        {
            if (despesa.Valor <= 0)
                throw new ArgumentOutOfRangeException("despesa", "Valor da despesa não pode ser menor que zero!");

            _despesas.Add(despesa);
        }

        public decimal ObterSaldoNaoCompartilhado()
        {
            var total = ValorTotalReceita;
            var despesasNaoCompartilhadas = ObterDespesasNaoCompartilhadas();
            total = despesasNaoCompartilhadas.Aggregate(total, (current, despesa) => current - despesa.Valor);

            return total;
        }

        private IEnumerable<Despesa> ObterDespesasCompartilhadas()
        {
            var despesasCompartilhadas = Despesas.Where(d => d.NumeroPessoasCompartilhar > 1);
            return despesasCompartilhadas;
        }

        private IEnumerable<Despesa> ObterDespesasNaoCompartilhadas()
        {
            var despesasNaoCompartilhadas = Despesas.Where(d => d.NumeroPessoasCompartilhar <= 1);
            return despesasNaoCompartilhadas;
        }

        public decimal ObterSaldoCompartilhado()
        {
            var total = ValorTotalReceita;
            var despesasCompartilhadas = ObterDespesasCompartilhadas();
            total = despesasCompartilhadas.Aggregate(total, (current, despesa) => current - (despesa.Valor/despesa.NumeroPessoasCompartilhar));

            return total;
        }

        public decimal ObterSaldoTotal()
        {
            var total = ValorTotalReceita;

            var saldoCompartilhado = ObterDespesasCompartilhadas().Sum(d => (d.Valor / d.NumeroPessoasCompartilhar));
            var saldoNaoCompartilhado = ObterDespesasNaoCompartilhadas().Sum(d => d.Valor);

            if (saldoCompartilhado >= saldoNaoCompartilhado)
                total -= saldoCompartilhado + saldoNaoCompartilhado;
            else
                total -= saldoNaoCompartilhado + saldoCompartilhado;

            return total;
        }
    }
}