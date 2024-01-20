namespace Winmd;

public interface IVisitor<in TIn>
{
    void Visit(TIn input);
}

public interface IVisitor<in TIn, out TResult>
{
    TResult Visit(TIn input);
}
