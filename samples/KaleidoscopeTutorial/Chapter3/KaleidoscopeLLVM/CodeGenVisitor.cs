using System;
using System.Collections.Generic;
using Kaleidoscope.AST;
using LLVMSharp;
using LLVMSharp.Interop;

namespace KaleidoscopeLLVM
{
    internal sealed class CodeGenVisitor : ExprVisitor
    {        
        private readonly LLVMModuleRef module;

        private readonly LLVMBuilderRef builder;

        private readonly Dictionary<string, LLVMValueRef> namedValues = new Dictionary<string, LLVMValueRef>();

        private readonly Stack<LLVMValueRef> valueStack = new Stack<LLVMValueRef>();

        public CodeGenVisitor(LLVMModuleRef module, LLVMBuilderRef builder)
        {
            this.module = module;
            this.builder = builder;
        }

        public Stack<LLVMValueRef> ResultStack { get { return valueStack; } }

        public void ClearResultStack()
        {
            this.valueStack.Clear();
        }

        protected override ExprAST VisitNumberExprAST(NumberExprAST node)
        {            
            this.valueStack.Push(LLVMValueRef.CreateConstReal(module.Context.DoubleType, node.Value));
            return node;
        }

        protected override ExprAST VisitVariableExprAST(VariableExprAST node)
        {
            LLVMValueRef value;

            // Look this variable up in the function.
            if (this.namedValues.TryGetValue(node.Name, out value))
            {
                this.valueStack.Push(value);
            }
            else
            {
                throw new Exception("Unknown variable name");
            }

            return node;
        }

        protected override ExprAST VisitBinaryExprAST(BinaryExprAST node)
        {
            this.Visit(node.Lhs);
            this.Visit(node.Rhs);

            LLVMValueRef r = this.valueStack.Pop();
            LLVMValueRef l = this.valueStack.Pop();

            LLVMValueRef n;

            switch (node.NodeType)
            {
                case ExprType.AddExpr:                    
                    n = builder.BuildFAdd(l, r, "addtmp");
                    break;
                case ExprType.SubtractExpr:
                    n = builder.BuildFSub(l, r, "subtmp");
                    break;
                case ExprType.MultiplyExpr:
                    n = builder.BuildFMul(l, r, "multmp");
                    break;
                case ExprType.LessThanExpr:
                    // Convert bool 0/1 to double 0.0 or 1.0                    
                    n = builder.BuildUIToFP(builder.BuildFCmp(LLVMRealPredicate.LLVMRealULT, l, r, "cmptmp"), module.Context.DoubleType, "booltmp");
                    break;
                default:
                    throw new Exception("invalid binary operator");
            }

            this.valueStack.Push(n);
            return node;
        }

        protected override ExprAST VisitCallExprAST(CallExprAST node)
        {

            var calleeF = module.GetNamedFunction(node.Callee);
            if (calleeF.Handle == IntPtr.Zero)
            {
                throw new Exception("Unknown function referenced");
            }            

            if (calleeF.ParamsCount != node.Arguments.Count)
            {
                throw new Exception("Incorrect # arguments passed");
            }

            var argumentCount = (uint)node.Arguments.Count;
            var argsV = new LLVMValueRef[Math.Max(argumentCount, 1)];
            for (int i = 0; i < argumentCount; ++i)
            {
                this.Visit(node.Arguments[i]);
                argsV[i] = this.valueStack.Pop();
            }

            valueStack.Push(builder.BuildCall(calleeF, argsV, "calltmp"));

            return node;
        }

        protected override ExprAST VisitPrototypeAST(PrototypeAST node)
        {
            // Make the function type:  double(double,double) etc.
            var argumentCount = (uint)node.Arguments.Count;
            var arguments = new LLVMTypeRef[argumentCount];

            var function = module.GetNamedFunction(node.Name);

            // If F conflicted, there was already something named 'Name'.  If it has a
            // body, don't allow redefinition or reextern.
            if (function.Handle != IntPtr.Zero)
            {
                // If F already has a body, reject this.                
                if (function.BasicBlocksCount != 0)
                {
                    throw new Exception("redefinition of function.");
                }

                // If F took a different number of args, reject.
                if (function.ParamsCount != argumentCount)
                {
                    throw new Exception("redefinition of function with different # args");
                }
            }
            else
            {
                for (int i = 0; i < argumentCount; ++i)
                {
                    arguments[i] = module.Context.DoubleType;
                }                
                function = module.AddFunction(node.Name, LLVMTypeRef.CreateFunction(module.Context.DoubleType, arguments, false));
                function.Linkage = LLVMLinkage.LLVMExternalLinkage;
            }

            for (int i = 0; i < argumentCount; ++i)
            {
                string argumentName = node.Arguments[i];

                LLVMValueRef param = function.GetParam((uint)i);
                param.Name = argumentName;                

                this.namedValues[argumentName] = param;
            }

            this.valueStack.Push(function);
            return node;
        }

        protected override ExprAST VisitFunctionAST(FunctionAST node)
        {
            this.namedValues.Clear();

            this.Visit(node.Proto);

            LLVMValueRef function = this.valueStack.Pop();

            // Create a new basic block to start insertion into.
            builder.PositionAtEnd(function.AppendBasicBlock("entry"));            

            try
            {
                this.Visit(node.Body);
            }
            catch (Exception)
            {
                function.DeleteFunction();
                throw;
            }

            // Finish off the function.
            builder.BuildRet(this.valueStack.Pop());

            // Validate the generated code, checking for consistency.
            function.VerifyFunction(LLVMVerifierFailureAction.LLVMPrintMessageAction);

            this.valueStack.Push(function);

            return node;
        }
    }
}
