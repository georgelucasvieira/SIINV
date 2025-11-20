namespace AuthService.Api.Data.Entities;

/// <summary>
/// Entidade de usuário para autenticação
/// </summary>
public class Usuario
{
    public long Id { get; protected set; }
    public DateTime CriadoEm { get; protected set; } = DateTime.UtcNow;
    public DateTime? AtualizadoEm { get; protected set; }

    public string Nome { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string SenhaHash { get; private set; } = string.Empty;
    public string Role { get; private set; } = string.Empty;
    public bool Ativo { get; private set; }
    public DateTime? UltimoLogin { get; private set; }
    public int TentativasLogin { get; private set; }
    public DateTime? BloqueadoAte { get; private set; }

    protected Usuario() { }

    public Usuario(string nome, string email, string senhaHash, string role = "Usuario")
    {
        ValidarDados(nome, email);

        Nome = nome;
        Email = email.ToLowerInvariant();
        SenhaHash = senhaHash;
        Role = role;
        Ativo = true;
        TentativasLogin = 0;
    }

    private static void ValidarDados(string nome, string email)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome é obrigatório", nameof(nome));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email é obrigatório", nameof(email));

        if (!email.Contains('@'))
            throw new ArgumentException("Email inválido", nameof(email));
    }

    public void AtualizarNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome é obrigatório", nameof(nome));

        Nome = nome;
        AtualizadoEm = DateTime.UtcNow;
    }

    public void AlterarSenha(string novaSenhaHash)
    {
        if (string.IsNullOrWhiteSpace(novaSenhaHash))
            throw new ArgumentException("Senha é obrigatória", nameof(novaSenhaHash));

        SenhaHash = novaSenhaHash;
        AtualizadoEm = DateTime.UtcNow;
    }

    public void AlterarRole(string novaRole)
    {
        if (string.IsNullOrWhiteSpace(novaRole))
            throw new ArgumentException("Role é obrigatória", nameof(novaRole));

        Role = novaRole;
        AtualizadoEm = DateTime.UtcNow;
    }

    public void RegistrarLogin()
    {
        UltimoLogin = DateTime.UtcNow;
        TentativasLogin = 0;
        BloqueadoAte = null;
    }

    public void RegistrarTentativaFalha()
    {
        TentativasLogin++;

        if (TentativasLogin >= 5)
        {
            BloqueadoAte = DateTime.UtcNow.AddMinutes(15);
        }
    }

    public bool EstaBloqueado()
    {
        if (BloqueadoAte == null) return false;
        if (BloqueadoAte <= DateTime.UtcNow)
        {
            BloqueadoAte = null;
            TentativasLogin = 0;
            return false;
        }
        return true;
    }

    public void Ativar()
    {
        Ativo = true;
        AtualizadoEm = DateTime.UtcNow;
    }

    public void Desativar()
    {
        Ativo = false;
        AtualizadoEm = DateTime.UtcNow;
    }
}

/// <summary>
/// Constantes de roles para autorização
/// </summary>
public static class Roles
{
    public const string Admin = "Admin";
    public const string Gerente = "Gerente";
    public const string Usuario = "Usuario";

    public static readonly string[] TodosRoles = [Admin, Gerente, Usuario];
}
