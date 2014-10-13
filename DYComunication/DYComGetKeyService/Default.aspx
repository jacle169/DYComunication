<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="DYComGetKeyService._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <asp:Label ID="Label3" runat="server"></asp:Label>
        <br />
    
        <asp:Label ID="Label1" runat="server" Text="户名"></asp:Label>
        <asp:TextBox ID="TextBox1" runat="server"></asp:TextBox>
        <br />
        <asp:Label ID="Label2" runat="server" Text="邮箱"></asp:Label>
        <asp:TextBox ID="TextBox2" runat="server"></asp:TextBox>
        <br />
        <asp:Button ID="Button2" runat="server" onclick="Button2_Click" Text="查询密钥" />
        <asp:Button ID="Button1" runat="server" onclick="Button1_Click" Text="申请密钥" />
    
        <br />
        <asp:Image ID="Image1" runat="server" ImageUrl="~/imgs/logo.png" />
        <br />
        <br />
    
    </div>
    </form>
</body>
</html>
