﻿using CodeCracker.Refactoring;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using TestHelper;
using Xunit;
namespace CodeCracker.Test.Refactoring
{
    public class AllowMembersOrderingAnalyzerTests : CodeFixVerifier
    {
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new AllowMembersOrderingAnalyzer();
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async void AllowMembersOrderingForEmptyTypeShouldNotTiggerDiagnostic(string typeDeclaration)
        {
            var test = @"
            " + typeDeclaration + @" Foo
            {
            }";

            await VerifyCSharpHasNoDiagnosticsAsync(test);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async void AllowMembersOrderingForOneMemberShouldNotTiggerDiagnostic(string typeDeclaration)
        {
            var test = @"
            " + typeDeclaration + @" Foo
            {
                int bar() { return 0; }
            }";

            await VerifyCSharpHasNoDiagnosticsAsync(test);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async void AllowMembersOrderingForMoreThanOneMemberShouldTiggerDiagnostic(string typeDeclaration)
        {
            var test = @"
            " + typeDeclaration + @" Foo
            {
                int bar() { return 0; }
                void car() { }
            }";

            var expected = new DiagnosticResult
            {
                Id = AllowMembersOrderingAnalyzer.DiagnosticId,
                Message = AllowMembersOrderingAnalyzer.MessageFormat,
                Severity = DiagnosticSeverity.Hidden,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 2, 14 + typeDeclaration.Length) }
            };

            await VerifyCSharpDiagnosticAsync(test, expected);
        }
    }
}