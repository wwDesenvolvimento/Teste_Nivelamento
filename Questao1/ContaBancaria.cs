using System.Globalization;

namespace Questao1
{
    class ContaBancaria {
        // Propriedades
        public int Numero { get; private set; }
        public string Titular { get; set; }
        private double _saldo;

        // Construtor com depósito inicial
        public ContaBancaria(int numero, string titular, double depositoInicial)
        {
            Numero = numero;
            Titular = titular;
            Deposito(depositoInicial);
        }

        // Construtor sem depósito inicial
        public ContaBancaria(int numero, string titular)
        {
            Numero = numero;
            Titular = titular;
            _saldo = 0.0;
        }

        // Método para realizar depósito
        public void Deposito(double quantia)
        {
            _saldo += quantia;
        }

        // Método para realizar saque, incluindo a taxa de $3.50
        public void Saque(double quantia)
        {
            _saldo -= quantia + 3.50;
        }

        // Sobrescrita do método ToString para exibir as informações da conta
        public override string ToString()
        {
            return "Conta "
                + Numero
                + ", Titular: "
                + Titular
                + ", Saldo: $ "
                + _saldo.ToString("F2", CultureInfo.InvariantCulture);
        }

    }
}
