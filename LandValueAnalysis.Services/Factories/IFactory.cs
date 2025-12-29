namespace LandValueAnalysis.Services.Factories;

public interface IFactory<T, U>
{
    Task<U> BuildAsync(T specs);
}