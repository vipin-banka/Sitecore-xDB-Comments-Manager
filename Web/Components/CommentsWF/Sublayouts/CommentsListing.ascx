<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CommentsListing.ascx.cs" Inherits="Web.Components.CommentsWF.Sublayouts.CommentEditor" %>

<!--Display the comment with user name and date time of comment-->
<div class="basic-grey">
    <div class="blog-container">
        <asp:Repeater runat="server" ID="rptComments" OnItemDataBound="rptComments_OnItemDataBound">
            <HeaderTemplate>
                <h3 class="comment-header">Comments</h3>
                <ul class="comment-Container" style="list-style: none">
            </HeaderTemplate>
            <ItemTemplate>
                <li class="comment-content">
                    <h5 class="comment-author-name">By
                        <asp:Literal ID="ltAuthor" runat="server"></asp:Literal>
                        on  <span class="comment-time">
                            <asp:Literal ID="ltDate" runat="server"></asp:Literal>
                        </span></h5>
                    <span class="comment-body">
                        <asp:Literal ID="ltComment" runat="server"></asp:Literal></span>

                </li>
            </ItemTemplate>
            <FooterTemplate>
                </ul>
            </FooterTemplate>

        </asp:Repeater>
    </div>
    <a class="load_more-button" id="Btn-More"></a>
</div>
<asp:HiddenField runat="server" ID="hfResultsPerClick" ClientIDMode="Static" />
<asp:HiddenField runat="server" ID="hfGUID" ClientIDMode="Static" />
<asp:HiddenField runat="server" ID="hftotalRecords" ClientIDMode="Static" />
<asp:HiddenField runat="server" ID="hfresultOnPageLoad" ClientIDMode="Static" />



