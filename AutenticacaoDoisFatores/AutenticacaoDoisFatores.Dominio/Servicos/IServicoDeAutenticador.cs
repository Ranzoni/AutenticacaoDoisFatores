namespace AutenticacaoDoisFatores.Dominio.Servicos
{
    public interface IServicoDeAutenticador
    {
        string GerarQrCode(string email, string chaveSecreta);
        bool CodigoEhValido(string codigo, string chaveSecreta);
    }
}
