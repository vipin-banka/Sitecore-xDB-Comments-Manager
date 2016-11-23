define(["sitecore", "jquery"], function (Sitecore, $) {
    var assignEditor = Sitecore.Definitions.App.extend({       
        initialized: function () {           
            this.btnGetTemplate.on("click", function () {               
                var myObj = {
                    Ids: this.tvSitecore.viewModel.checkedItemIds._latestValue
                };               
                if (myObj.Ids != null) {
                    $.ajax({
                        type: "POST",
                        url: "/api/sitecore/Comment/Assign",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        data: JSON.stringify(myObj),
                        success: function (data) {
                            if(data.length>0)
                            alert(data);
                        },
                        error: function (xhr, textStatus, errorThrown) {
                            alert('Failed to assign editor');
                        }
                    });
                }
            }, this);
        }
    });
    return assignEditor;
});