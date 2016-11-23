<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CommentEditor.aspx.cs" Inherits="Web.sitecore.shell.client.YourApps.CommentEditor" %>

<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">
    <title>Assign Editor</title>

    <%--Java script for confirm before delete any comment.--%>
    <script type="text/javascript">
        function ConfirmDelete() {
            if (confirm("Are you sure you want to Delete?") == true)
                return true;
            else
                return false;
        }
    </script>

    <%--styles for comment editor.--%>
    <style type="text/css">
        .button {
            background: #E27575;
            border: none;
            padding: 10px 25px 10px 25px;
            color: #FFF;
            box-shadow: 1px 1px 5px #B6B6B6;
            border-radius: 3px;
            text-shadow: 1px 1px 1px #9E3F3F;
            cursor: pointer;
            margin-top: 10px;
        }

            .button:hover {
                background: #CF7A7A;
            }
    </style>

</head>
<body>
    <%--Assign editor window.--%>
    <form id="frmCommentEditor" runat="server">
        <div class="comment-editor-container">
            <asp:Label ID="lblcommentstatus" runat="server"></asp:Label>
            <asp:GridView ID="GridComments" runat="server" Width="700px"
                AutoGenerateColumns="false" HeaderStyle-BackColor="#c9c9c9" HeaderStyle-ForeColor="WhiteSmoke" AllowPaging="True" OnPageIndexChanging="GridComments_OnPageIndexChanging">
                <Columns>
                    <asp:BoundField DataField="Body" HeaderText="Comment" HtmlEncode="true" />

                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:Label ID="Publish" runat="server" Text="Approve" />
                            <asp:CheckBox ID="chkboxSelectAll" runat="server" OnCheckedChanged="CheckAll" AutoPostBack="True" />
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:CheckBox ID="chkPublish" runat="server" AutoPostBack="False" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:Label ID="Delete" runat="server" Text="Delete" />
                            <asp:CheckBox ID="chkboxDeleteAll" runat="server" OnCheckedChanged="CheckAll" AutoPostBack="True" />
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:CheckBox ID="chkDelete" runat="server" AutoPostBack="False" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <asp:Button ID="btnPublish" runat="server" Text="Approve" OnClick="ApproveSelectedComment" Visible="False" CssClass="button" Width="100px" />
            <asp:Button ID="btnDelete" runat="server" Text="Delete" OnClientClick="return ConfirmDelete();" OnClick="DeleteSelectedComment" Visible="False" CssClass="button" Width="100px" />

        </div>
    </form>
</body>
</html>
