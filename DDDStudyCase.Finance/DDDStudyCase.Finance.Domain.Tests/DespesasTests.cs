using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;

namespace DDDStudyCase.Finance.Domain.Tests
{
    [TestFixture]
    public class DespesasTests
    {
        private Mock<IServicoReceita> _servicoReceitaMoq;
        private IServicoReceita _servicoReceita;
        private static Despesa _despesaComBebedeira;
        private static Despesa _despesaCompartilhada;
        private const int DUAS_PESSOAS_COMPARTILHAR = 2;
        private const decimal MIL_REAIS = 1000;
        private const decimal DUZENTOS_REAIS = 200;

        [SetUp]
        public void Inicializar()
        {
            _servicoReceitaMoq = new Mock<IServicoReceita>();
            var receita = new Receita(valorTotal: MIL_REAIS);
            
            _servicoReceitaMoq.Setup(s => s.ObterReceitaAtual()).Returns(() => receita);

            _servicoReceita = _servicoReceitaMoq.Object;

            _despesaComBebedeira = new Despesa(valor: DUZENTOS_REAIS);
            _despesaCompartilhada = new Despesa(valor: DUZENTOS_REAIS, numeroPessoasCompartilhar: DUAS_PESSOAS_COMPARTILHAR);
        }

        [Test]
        public void Deve_obter_receita_com_valorTotal()
        {
            var receita = _servicoReceita.ObterReceitaAtual();
            Assert.That(receita.ValorTotalReceita, Is.EqualTo(MIL_REAIS));
        }

        [Test]
        public void Deve_inserir_despesa_na_receita()
        {
            var receita = _servicoReceita.ObterReceitaAtual();
            var despesa = new Despesa(valor: DUZENTOS_REAIS, numeroPessoasCompartilhar: DUAS_PESSOAS_COMPARTILHAR);
            receita.InserirDespesa(despesa);

            Assert.That(receita.Despesas.First().Valor, Is.EqualTo(DUZENTOS_REAIS));
        }

        [TestCase(-200)]
        [TestCase(0)]
        public void Nao_deve_inserir_despesa_com_valor_invalido(decimal valorDespesa)
        {
            var receita = _servicoReceita.ObterReceitaAtual();
            var despesa = new Despesa(valor: valorDespesa);
            Assert.Throws<ArgumentOutOfRangeException>(() => receita.InserirDespesa(despesa));
        }

        [Test]
        public void Deve_inserir_despesa_simples_em_receita_e_obter_saldo_despesas_nao_compartilhadas()
        {
            var receita = _servicoReceita.ObterReceitaAtual();

            receita.InserirDespesa(_despesaComBebedeira);
            var saldo = receita.ObterSaldoNaoCompartilhado();

            Assert.That(saldo, Is.EqualTo((MIL_REAIS - DUZENTOS_REAIS)));
        }

        [Test]
        public void Deve_inserir_despesa_compartilhada_em_receita_e_obter_saldo_compartilhado()
        {
            var receita = _servicoReceita.ObterReceitaAtual();

            receita.InserirDespesa(_despesaCompartilhada);

            var saldo = receita.ObterSaldoCompartilhado();

            Assert.That(saldo, Is.EqualTo((MIL_REAIS - (DUZENTOS_REAIS / DUAS_PESSOAS_COMPARTILHAR))));
        }


        public IEnumerable<TestCaseData> DespesasParaObterSaldoTotalTestCase
        {
            get
            {
                Inicializar();
                yield return new TestCaseData(new List<Despesa>() { _despesaComBebedeira, _despesaCompartilhada }, MIL_REAIS - DUZENTOS_REAIS - (DUZENTOS_REAIS / DUAS_PESSOAS_COMPARTILHAR));
                yield return new TestCaseData(new List<Despesa>() { new Despesa(400), new Despesa(200, 2) }, 500m);
                yield return new TestCaseData(new List<Despesa>() { new Despesa(150), new Despesa(600, 3) }, 650m);
            }
        }

        [TestCaseSource("DespesasParaObterSaldoTotalTestCase")]
        public void Deve_inserir_despesas_compartilhada_e_naoCompartilhada_em_receita_e_obter_saldo_total(List<Despesa> despesas, decimal expectedResult)
        {
            var receita = _servicoReceita.ObterReceitaAtual();

            foreach (var despesa in despesas)
            {
                receita.InserirDespesa(despesa);    
            }

            var saldo = receita.ObterSaldoTotal();

            Assert.That(saldo, Is.EqualTo(expectedResult));
        }
    
    }
}
