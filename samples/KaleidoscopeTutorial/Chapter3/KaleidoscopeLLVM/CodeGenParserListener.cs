using LLVMSharp;

namespace KaleidoscopeLLVM
{
    using Kaleidoscope;
    using Kaleidoscope.AST;

    internal sealed class CodeGenParserListener : IParserListener
    {
        private readonly CodeGenVisitor visitor;

        public CodeGenParserListener(CodeGenVisitor visitor)
        {
            this.visitor = visitor;
        }

        public void EnterHandleDefinition(FunctionAST data)
        {
        }

        public void ExitHandleDefinition(FunctionAST data)
        {
            this.visitor.Visit(data);
            this.visitor.ResultStack.Pop().Dump();
        }

        public void EnterHandleExtern(PrototypeAST data)
        {
        }

        public void ExitHandleExtern(PrototypeAST data)
        {
            this.visitor.Visit(data);
            this.visitor.ResultStack.Pop().Dump();
        }

        public void EnterHandleTopLevelExpression(FunctionAST data)
        {
        }

        public void ExitHandleTopLevelExpression(FunctionAST data)
        {
            this.visitor.Visit(data);
            this.visitor.ResultStack.Pop().Dump();
        }
    }
}
