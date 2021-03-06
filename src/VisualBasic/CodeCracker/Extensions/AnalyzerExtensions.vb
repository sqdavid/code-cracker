﻿Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Diagnostics
Imports Microsoft.CodeAnalysis.VisualBasic

Public Module AnalyzerExtensions
    <Extension> Public Sub RegisterSyntaxNodeAction(Of TLanguageKindEnum As Structure)(context As AnalysisContext, languageVersion As LanguageVersion, action As Action(Of SyntaxNodeAnalysisContext), ParamArray syntaxKinds As TLanguageKindEnum())
        context.RegisterCompilationStartAction(languageVersion, Sub(compilationContext) compilationContext.RegisterSyntaxNodeAction(action, syntaxKinds))
    End Sub

    <Extension> Public Sub RegisterCompilationStartAction(context As AnalysisContext, languageVersion As LanguageVersion, registrationAction As Action(Of CompilationStartAnalysisContext))
        context.RegisterCompilationStartAction(Sub(compilationContext) compilationContext.RunIfVB14OrGreater(Sub() registrationAction?.Invoke(compilationContext)))
    End Sub

    <Extension> Private Sub RunIfVB14OrGreater(context As CompilationStartAnalysisContext, action As Action)
        context.Compilation.RunIfVB14OrGreater(action)
    End Sub

    <Extension> Private Sub RunIfVB14OrGreater(compilation As Compilation, action As Action)
        Dim vbCompilation = TryCast(compilation, VisualBasicCompilation)
        If vbCompilation Is Nothing Then
            Return
        End If
        vbCompilation.LanguageVersion.RunWithVB14OrGreater(action)
    End Sub

    <Extension> Public Sub RunWithVB14OrGreater(languageVersion As LanguageVersion, action As Action)
        If languageVersion >= LanguageVersion.VisualBasic14 Then action?.Invoke()
    End Sub

    <Extension> Public Function WithSameTriviaAs(target As SyntaxNode, source As SyntaxNode) As SyntaxNode
        If target Is Nothing Then
            Throw New ArgumentNullException(NameOf(target))
        End If
        If source Is Nothing Then
            Throw New ArgumentNullException(NameOf(target))
        End If

        Return target.WithLeadingTrivia(source.GetLeadingTrivia()).WithTrailingTrivia(source.GetTrailingTrivia())
    End Function

    <Extension>
    Public Function FirstAncestorOfType(node As SyntaxNode, ParamArray types() As Type) As SyntaxNode
        Dim currentNode = node
        While (True)
            Dim parent = currentNode.Parent
            If parent Is Nothing Then Exit While
            For Each targetType In types
                If parent.GetType Is targetType Then Return parent
            Next
            currentNode = parent
        End While
        Return Nothing
    End Function

    <Extension>
    Public Function FirstAncestorOrSelfOfType(node As SyntaxNode, ParamArray types() As Type) As SyntaxNode
        Dim currentNode = node
        While True
            If currentNode Is Nothing Then Exit While
            For Each targetType In types
                If currentNode.GetType Is targetType Then Return currentNode
            Next
            currentNode = currentNode.Parent
        End While
        Return Nothing
    End Function
    <Extension>
    Public Function FirstAncestorOfType(Of T As SyntaxNode)(node As SyntaxNode) As T
        Return DirectCast(node.FirstAncestorOfType(GetType(T)), T)
    End Function
End Module