namespace AutenticacaoDoisFatores.Dominio.Servicos
{
    public interface IServicoDeEmail
    {
        public void Enviar(string para, string titulo, string mensagem);
    }
}
