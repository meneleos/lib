Imports Matrix.Xml

Namespace Settings
	'   
'    This class shows how agsXMPP could also be used read and write custom xml files.
'    Here we use it for the application settings which are stored in xml files.
'     
'    This works similar to the .NET serialization, but is much easier to use.
'    All you have to do is derive from XmppXElement and write down your properties
'    
'    creating you own Xmpp extensions works exactly in the same way
'    

	Public Class Login
		Inherits XmppXElement
		Public Sub New()
			MyBase.New("ag-software:settings", "Login")
		End Sub

		Public Property User() As String
			Get
				Return GetTag("User")
			End Get
			Set
				SetTag("User", value)
			End Set
		End Property

		Public Property Server() As String
			Get
				Return GetTag("Server")
			End Get
			Set
				SetTag("Server", value)
			End Set
		End Property

		Public Property Password() As String
			Get
				Return GetTag("Password")
			End Get
			Set
				SetTag("Password", value)
			End Set
		End Property

		Public Property Resource() As String
			Get
				Return GetTag("Resource")
			End Get
			Set
				SetTag("Resource", value)
			End Set
		End Property

		Public Property Priority() As Integer
			Get
				Return GetAttributeInt("Priority")
			End Get
			Set
				SetTag("Priority", value)
			End Set
		End Property

		Public Property Port() As Integer
			Get
				Return GetTagInt("Port")
			End Get
			Set
				SetTag("Port", value)
			End Set
		End Property

		Public Property Tls() As Boolean
			Get
				Return GetTagBool("Tls")
			End Get
			Set
				SetTag("Tls", value)
			End Set
		End Property
	End Class
End Namespace