// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from https://github.com/llvm/llvm-project/tree/llvmorg-12.0.0/llvm/include/llvm-c
// Original source is Copyright (c) the LLVM Project and Contributors. Licensed under the Apache License v2.0 with LLVM Exceptions. See NOTICE.txt in the project root for license information.

using NUnit.Framework;
using System.Runtime.InteropServices;

namespace LLVMSharp.Interop.UnitTests
{
    /// <summary>Provides validation of the <see cref="LLVMOrcOpaqueJITTargetMachineBuilder" /> struct.</summary>
    public static unsafe class LLVMOrcOpaqueJITTargetMachineBuilderTests
    {
        /// <summary>Validates that the <see cref="LLVMOrcOpaqueJITTargetMachineBuilder" /> struct is blittable.</summary>
        [Test]
        public static void IsBlittableTest()
        {
            Assert.That(Marshal.SizeOf<LLVMOrcOpaqueJITTargetMachineBuilder>(), Is.EqualTo(sizeof(LLVMOrcOpaqueJITTargetMachineBuilder)));
        }

        /// <summary>Validates that the <see cref="LLVMOrcOpaqueJITTargetMachineBuilder" /> struct has the right <see cref="LayoutKind" />.</summary>
        [Test]
        public static void IsLayoutSequentialTest()
        {
            Assert.That(typeof(LLVMOrcOpaqueJITTargetMachineBuilder).IsLayoutSequential, Is.True);
        }

        /// <summary>Validates that the <see cref="LLVMOrcOpaqueJITTargetMachineBuilder" /> struct has the correct size.</summary>
        [Test]
        public static void SizeOfTest()
        {
            Assert.That(sizeof(LLVMOrcOpaqueJITTargetMachineBuilder), Is.EqualTo(1));
        }
    }
}
