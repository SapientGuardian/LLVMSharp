// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from https://github.com/llvm/llvm-project/tree/llvmorg-12.0.0/llvm/include/llvm-c
// Original source is Copyright (c) the LLVM Project and Contributors. Licensed under the Apache License v2.0 with LLVM Exceptions. See NOTICE.txt in the project root for license information.

using NUnit.Framework;
using System.Runtime.InteropServices;

namespace LLVMSharp.Interop.UnitTests
{
    /// <summary>Provides validation of the <see cref="LLVMRemarkOpaqueEntry" /> struct.</summary>
    public static unsafe class LLVMRemarkOpaqueEntryTests
    {
        /// <summary>Validates that the <see cref="LLVMRemarkOpaqueEntry" /> struct is blittable.</summary>
        [Test]
        public static void IsBlittableTest()
        {
            Assert.That(Marshal.SizeOf<LLVMRemarkOpaqueEntry>(), Is.EqualTo(sizeof(LLVMRemarkOpaqueEntry)));
        }

        /// <summary>Validates that the <see cref="LLVMRemarkOpaqueEntry" /> struct has the right <see cref="LayoutKind" />.</summary>
        [Test]
        public static void IsLayoutSequentialTest()
        {
            Assert.That(typeof(LLVMRemarkOpaqueEntry).IsLayoutSequential, Is.True);
        }

        /// <summary>Validates that the <see cref="LLVMRemarkOpaqueEntry" /> struct has the correct size.</summary>
        [Test]
        public static void SizeOfTest()
        {
            Assert.That(sizeof(LLVMRemarkOpaqueEntry), Is.EqualTo(1));
        }
    }
}
