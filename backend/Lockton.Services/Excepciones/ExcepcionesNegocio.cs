namespace Lockton.Services.Excepciones;

// Excepciones de negocio.

/// <summary>Datos inválidos según reglas de negocio → 400.</summary>
public class ValidacionNegocioException : Exception
{
    public ValidacionNegocioException(string mensaje) : base(mensaje) { }
}

/// <summary>Credenciales incorrectas → 401.</summary>
public class CredencialesInvalidasException : Exception
{
    public CredencialesInvalidasException() : base("Usuario o contraseña incorrectos.") { }
}

/// <summary>Autenticado pero sin permiso sobre el recurso → 403.</summary>
public class AccesoDenegadoException : Exception
{
    public AccesoDenegadoException(string mensaje) : base(mensaje) { }
}

/// <summary>Recurso inexistente → 404.</summary>
public class NoEncontradoException : Exception
{
    public NoEncontradoException(string mensaje) : base(mensaje) { }
}

/// <summary>Conflicto con datos existentes (ej: usuario duplicado) → 409.</summary>
public class ConflictoException : Exception
{
    public ConflictoException(string mensaje) : base(mensaje) { }
}
