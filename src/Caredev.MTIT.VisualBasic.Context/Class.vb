Imports System
Imports System.Collections.Generic
$if$ ($targetframeworkversion$ >= 3.5)Imports System.Linq
$endif$Imports Caredev.Mego
Imports Caredev.Mego.DataAnnotations
Imports mego = Caredev.Mego.DataAnnotations

Namespace $rootnamespace$
    Public Class $safeitemname$
        Inherits DbContext

$if$ ($netcore$ == true)        Shared Sub New()
            Caredev.Mego.Resolve.Providers.DbAccessProvider.SetGetFactory(
                Function(providerName As String)
                    Select Case providerName
                        Case "System.Data.SqlClient"
                            Return System.Data.SqlClient.SqlClientFactory.Instance
                            'Write get DbProviderFactory logical.
                    End Select
                    Throw New NotImplementedException()
                End Function)
        End Sub

$endif$        Public Sub New()
            MyBase.New($megoargu$)
        End Sub
$megodbset$
    End Class
$megoclassdef$
End Namespace
