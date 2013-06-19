namespace DDDStudyCase.Finance.Domain
{
    public class Despesa
    {
        public decimal Valor { get; private set; }
        public int NumeroPessoasCompartilhar { get; private set; }

        public Despesa(decimal valor, int numeroPessoasCompartilhar)
        {
            Valor = valor;
            NumeroPessoasCompartilhar = numeroPessoasCompartilhar;
        }

        public Despesa(decimal valor)
        {
            Valor = valor;
        }
    }
}