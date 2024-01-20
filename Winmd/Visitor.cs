namespace Winmd;

public interface IVisitor<in I>
{
    void Visit(I input);
}

public interface IVisitor<in I, out O>
{
    O Visit(I input);
}
