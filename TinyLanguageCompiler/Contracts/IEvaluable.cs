namespace TinyLanguageCompiler.Contracts;

public interface IEvaluable
{
    string Evaluate(bool force);
}