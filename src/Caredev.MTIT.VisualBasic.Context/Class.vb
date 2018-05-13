Imports System
Imports System.Collections.Generic
$if$ ($targetframeworkversion$ >= 3.5)Imports System.Linq
$endif$Imports Caredev.Mego
Imports Caredev.Mego.DataAnnotations

Namespace $rootnamespace$
    Public Class $safeitemname$
        Inherits DbContext

        Public Sub New()
            MyBase.New($megoargu$)
        End Sub
$megodbset$
    End Class
$megoclassdef$
End Namespace
