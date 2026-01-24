namespace Infra.Exceptions
{
    /// <summary>
    /// Exceção base para operações de repositório
    /// </summary>
    public class RepositoryException : Exception
    {
        public RepositoryException(string message) : base(message) { }
        public RepositoryException(string message, Exception innerException)
            : base(message, innerException) { }
    }

    /// <summary>
    /// Exceção quando uma entidade não é encontrada
    /// </summary>
    public class EntityNotFoundException : RepositoryException
    {
        public EntityNotFoundException(string entityType, Guid id)
            : base($"{entityType} com id:{id} não encontrado!") { }

        public EntityNotFoundException(string entityType, string identifier)
            : base($"{entityType} '{identifier}' não encontrado!") { }
    }

    /// <summary>
    /// Exceção para violação de regras de negócio
    /// </summary>
    public class BusinessRuleViolationException : RepositoryException
    {
        public BusinessRuleViolationException(string message) : base(message) { }
    }

    /// <summary>
    /// Exceção para operações inválidas
    /// </summary>
    public class InvalidOperationException : RepositoryException
    {
        public InvalidOperationException(string message) : base(message) { }
    }
}