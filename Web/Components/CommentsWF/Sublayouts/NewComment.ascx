<%@ Control Language="C#" AutoEventWireup="true" CodeFile="NewComment.ascx.cs" Inherits="Web.Components.CommentsWF.Sublayouts.NewComment" Debug="true" %>
<script src='https://www.google.com/recaptcha/api.js'></script>
<!--Insert new comment form-->
<div class="basic-grey">
    <div class="blog-container">
        <h1>
            <asp:Label ID="lblInfoText" runat="server">Write a Comment</asp:Label>
        </h1>
        <asp:Panel CssClass="write-commment-container" ID="commentsContainer" runat="server">
            <label>
                <span>Your Name :</span>
                <asp:TextBox CssClass="txt-author-name" ID="txtauthorname" runat="server" placeholder="Your Full Name" required="true"></asp:TextBox>
            </label>

            <label>
                <span>Your Email :</span>

                <asp:TextBox CssClass="txt-email" ID="txtEmail" TextMode="Email" required="true" placeholder="Valid Email Address" runat="server"></asp:TextBox>
            </label>

            <label>
                <span>Comment :</span>
                <asp:TextBox CssClass="txt-comment-body" ID="txtComment" TextMode="MultiLine" required="true" Columns="20" Rows="5" placeholder="Your Comment" runat="server"></asp:TextBox>
            </label>
            <asp:Panel ID="recaptchaPanel" CssClass="g-recaptcha" runat="server"></asp:Panel>
            <div class="clear"></div>
            <div class="submit-container">
                <label>
                    <span>&nbsp;</span>
                    <asp:Button CssClass="button" Text="Submit" OnClick="ValidateCaptchaKey" runat="server" />
                </label>
            </div>
        </asp:Panel>
    </div>

</div>
