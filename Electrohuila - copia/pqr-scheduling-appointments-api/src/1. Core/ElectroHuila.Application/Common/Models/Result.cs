namespace ElectroHuila.Application.Common.Models;

/// <summary>
/// Representa el resultado de una operación que puede ser exitosa o fallida
/// </summary>
public class Result
{
    /// <summary>
    /// Indica si la operación fue exitosa
    /// </summary>
    public bool IsSuccess { get; protected set; }

    /// <summary>
    /// Mensaje de error en caso de fallo (null si es exitoso)
    /// </summary>
    public string? Error { get; protected set; }

    /// <summary>
    /// Indica si la operación falló
    /// </summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// Constructor protegido que valida la consistencia del resultado
    /// </summary>
    /// <param name="isSuccess">Indica si es exitoso</param>
    /// <param name="error">Mensaje de error (debe ser null si isSuccess es true)</param>
    /// <exception cref="InvalidOperationException">Si un resultado exitoso tiene error o uno fallido no lo tiene</exception>
    protected Result(bool isSuccess, string? error)
    {
        if (isSuccess && !string.IsNullOrEmpty(error))
            throw new InvalidOperationException("Success result cannot have an error");

        if (!isSuccess && string.IsNullOrEmpty(error))
            throw new InvalidOperationException("Failure result must have an error");

        IsSuccess = isSuccess;
        Error = error;
    }

    /// <summary>
    /// Crea un resultado exitoso sin datos
    /// </summary>
    /// <returns>Resultado exitoso</returns>
    public static Result Success() => new(true, null);

    /// <summary>
    /// Crea un resultado fallido con mensaje de error
    /// </summary>
    /// <param name="error">Mensaje de error</param>
    /// <returns>Resultado fallido</returns>
    public static Result Failure(string error) => new(false, error);

    /// <summary>
    /// Crea un resultado exitoso con datos
    /// </summary>
    /// <typeparam name="T">Tipo de datos del resultado</typeparam>
    /// <param name="data">Datos del resultado exitoso</param>
    /// <returns>Resultado exitoso con datos</returns>
    public static Result<T> Success<T>(T data) => new(data, true, null);

    /// <summary>
    /// Crea un resultado fallido con mensaje de error
    /// </summary>
    /// <typeparam name="T">Tipo de datos del resultado</typeparam>
    /// <param name="error">Mensaje de error</param>
    /// <returns>Resultado fallido sin datos</returns>
    public static Result<T> Failure<T>(string error) => new(default, false, error);
}

/// <summary>
/// Representa el resultado de una operación con datos tipados
/// </summary>
/// <typeparam name="T">Tipo de datos del resultado</typeparam>
public class Result<T> : Result
{
    /// <summary>
    /// Datos del resultado (null si es fallido)
    /// </summary>
    public T? Data { get; private set; }

    /// <summary>
    /// Constructor interno que inicializa el resultado con datos
    /// </summary>
    /// <param name="data">Datos del resultado</param>
    /// <param name="isSuccess">Indica si es exitoso</param>
    /// <param name="error">Mensaje de error (solo si es fallido)</param>
    internal Result(T? data, bool isSuccess, string? error) : base(isSuccess, error)
    {
        Data = data;
    }
}