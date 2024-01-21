namespace Winmd;

public interface IVisitor<in TIn, out TResult>
{
    TResult Visit(TIn input);
}
