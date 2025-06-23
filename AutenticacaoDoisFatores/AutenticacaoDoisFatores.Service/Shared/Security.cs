using AutenticacaoDoisFatores.Domain.Shared;
using AutenticacaoDoisFatores.Domain.Shared.Permissions;
using AutenticacaoDoisFatores.Domain.Domains;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security;
using System.Security.Claims;
using System.Text;
using AutenticacaoDoisFatores.Domain.Entities;

namespace AutenticacaoDoisFatores.Service.Shared
{
    public static partial class Security
    {
        public static byte[] AuthKey()
        {
            var authKey = Environment.GetEnvironmentVariable("ADF_CHAVE_AUTENTICACAO");
            if (authKey is null || authKey.IsNullOrEmptyOrWhiteSpaces())
                throw new ApplicationException("A chave de autenticação não foi encontrada");

            var bytes = Encoding.ASCII.GetBytes(authKey);
            return bytes;
        }

        public static (string key, string encryptedKey) GenerateEncryptedAuthCode()
        {
            var key = Guid.NewGuid().ToString();
            var encryptedKey = Encrypt.EncryptWithSha512(key);
            return (key, encryptedKey);
        }

        public static bool IsPasswordValid(string password)
        {
            return !password.IsNullOrEmptyOrWhiteSpaces() && password.AnyUpperCase() && password.AnyLowerCase() && password.AnyNumber() && password.AnySpecialCharactersOrPontuations() && password.Length >= 7 && password.Length <= 50;
        }

        public static string GenerateAuthCode()
        {
            var randomico = new Random();
            var codigo = randomico.Next(90000, 999999);
            return codigo.ToString().PadLeft(6, '0');
        }

        public static string GetIssuer()
        {
            var issuer = Environment.GetEnvironmentVariable("ADF_EMISSOR_TOKEN");
            if (string.IsNullOrEmpty(issuer))
                throw new ApplicationException("O emissor do token não foi encontrado");

            return issuer;
        }

        public static string GetAudience()
        {
            var audience = Environment.GetEnvironmentVariable("ADF_DESTINATARIO_TOKEN");
            if (string.IsNullOrEmpty(audience))
                throw new SecurityException("O destinatário do token não foi encontrado");

            return audience;
        }
    }

    #region Token generators

    public static partial class Security
    {
        #region Claims

        private static readonly string _entityIdentifierClaim = "entityIdentifier";
        private static readonly string _securityClaim = ClaimTypes.Role;

        #endregion

        #region Security roles

        private static readonly string _clientConfirmation = "clientConfirmation";
        private static readonly string _newClientKeyGeneration = "newClientKeyGeneration";
        private static readonly string _authenticatedUser = "authenticatedUser";
        private static readonly string _createUser = "createUser";
        private static readonly string _activateUser = "activateUser";
        private static readonly string _inactivateUser = "inactivateUser";
        private static readonly string _changeUserPassword = "changeUserPassword";
        private static readonly string _setPermissions = "setPermissions";
        private static readonly string _removeUser = "removeUser";
        private static readonly string _viewUsers = "viewUsers";
        private static readonly string _changeUserEmail = "changeUserEmail";
        private static readonly string _authCodeEmailSender = "authCodeEmailSender";
        private static readonly string _lastDataChange = "lastDataChange";

        private static readonly Dictionary<PermissionType, string> _permissionsRoles = new()
        {
            { PermissionType.CreateUser, _createUser },
            { PermissionType.ActivateUser, _activateUser },
            { PermissionType.InactivateUser, _inactivateUser },
            { PermissionType.ChangeUserPassword, _changeUserPassword },
            { PermissionType.SetPermissions, _setPermissions },
            { PermissionType.RemoveUser, _removeUser },
            { PermissionType.ViewUsers, _viewUsers },
            { PermissionType.ChangeUserEmail, _changeUserEmail },
        };

        public static string AuthenticatedUser
        {
            get
            {
                return _authenticatedUser;
            }
        }

        public static string ClientConfirmationRole
        {
            get
            {
                return _clientConfirmation;
            }
        }

        public static string NewClientKeyGenerationRole
        {
            get
            {
                return _newClientKeyGeneration;
            }
        }

        public static string CreateUserRole
        {
            get
            {
                return _createUser;
            }
        }

        public static string AcivateUserRole
        {
            get
            {
                return _activateUser;
            }
        }

        public static string InactivateUserRole
        {
            get
            {
                return _inactivateUser;
            }
        }

        public static string ChangeUserPasswordRole
        {
            get
            {
                return _changeUserPassword;
            }
        }

        public static string SetPermissionsRole
        {
            get
            {
                return _setPermissions;
            }
        }

        public static string RemoveUserRole
        {
            get
            {
                return _removeUser;
            }
        }

        public static string ViewUsersRole
        {
            get
            {
                return _viewUsers;
            }
        }

        public static string ChangeUserEmailRole
        {
            get
            {
                return _changeUserEmail;
            }
        }

        public static string AuthCodeEmailSenderRole
        {
            get
            {
                return _authCodeEmailSender;
            }
        }

        public static string LastDataChange
        {
            get
            {
                return _lastDataChange;
            }
        }

        #endregion

        public static string GenerateUserAuthToken(User user, IEnumerable<PermissionType>? permissions)
        {
            var claims = new List<Claim>()
            {
                new(type: _entityIdentifierClaim, user.Id.ToString()),
                new(type: _securityClaim, _authenticatedUser),
                new(type: _lastDataChange, user.LastDataChange.ToString() ?? "")
            };

            foreach (var permission in permissions ?? [])
            {
                var permissionClaim = _permissionsRoles[permission];
                claims.Add(new(type: _securityClaim, permissionClaim));
            }

            return GenerateToken(claims);
        }

        public static string GenerateClientConfirmationToken(Guid clientId)
        {
            return GenerateToken([
                new(type: _entityIdentifierClaim, clientId.ToString()),
                new(type: _securityClaim, _clientConfirmation)
            ]);
        }

        public static string GenerateNewAccessKeyToken(Guid clientId)
        {
            return GenerateToken([
                new(type: _entityIdentifierClaim, clientId.ToString()),
                new(type: _securityClaim, _newClientKeyGeneration)
            ]);
        }

        public static string GenerateUserAuthCodeToken(Guid userId)
        {
            return GenerateToken([
                new(type: _entityIdentifierClaim, userId.ToString()),
                new(type: _securityClaim, _authCodeEmailSender)
            ]);
        }

        private static string GenerateToken(IEnumerable<Claim?> claims)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var claimIdentity = new ClaimsIdentity();
            claimIdentity.AddClaims(claims);

            var authKey = AuthKey();
            var symmetricSecurityKey = new SymmetricSecurityKey(authKey);
            var credentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature);

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = claimIdentity,
                SigningCredentials = credentials,
                NotBefore = DateTime.Now,
                Expires = DateTime.Now.AddHours(2),
                Issuer = GetIssuer(),
                Audience = GetAudience()
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }

    #endregion

    #region Token readers

    public static partial class Security
    {
        public static Guid GetIdFromToken(string token)
        {
            var tokenClaims = ReadToken(token);
            var identifierClaim = tokenClaims.FirstOrDefault(perfil => perfil.Type.Equals(_entityIdentifierClaim));
            if (identifierClaim is null)
                return Guid.Empty;

            var id = Guid.Parse(identifierClaim.Value);
            return id;
        }

        private static IEnumerable<Claim> ReadToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var key = AuthKey();
            var symmetricSecurityKey = new SymmetricSecurityKey(key);
            var tokenParams = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = symmetricSecurityKey,
                ValidateIssuer = true,
                ValidIssuer = GetIssuer(),
                ValidateAudience = true,
                ValidAudience = GetAudience(),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(token, tokenParams, out _);
            return principal.Claims;
        }
    }

    #endregion
}
