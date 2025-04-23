namespace AutenticacaoDoisFatores.Dominio.Servicos
{
    public interface IServicoDeAutenticador
    {
        string GerarQrCode(string email);
        bool CodigoEhValido(string codigo);
    }
}
