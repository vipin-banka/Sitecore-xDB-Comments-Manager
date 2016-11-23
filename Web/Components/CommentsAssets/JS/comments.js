
$(document).ready(function () {
    var pagenumber = 1;
    var $remaingitems = $('#hftotalRecords').val() - $('#hfresultOnPageLoad').val();
    if ($remaingitems <= 0) {
        $('#Btn-More').hide();
    }
    $('#Btn-More').text($remaingitems + " More comments");
    $('#Btn-More').on("click", function () {
        var itemId = $('#hfGUID').val();
        var resultsPerClick = $('#hfResultsPerClick').val();
        var itemCount = $('.blog-container ul > li:visible').length;
        pagenumber = pagenumber + 1;
        var myObj = {
            itemID: itemId,
            itemsperpage: resultsPerClick,
            totalcount: itemCount,
            pagenumber: pagenumber
        };
        $.ajax({
            type: "POST",
            url: '/api/sitecore/Comment/GetLoadMoreData',
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(myObj),
            dataType: "json",
            success: function (data) {
                if (data.length > 0) {
                    appendRecords(data);
                    $remaingitems = $remaingitems - data.length;
                    $('#Btn-More').text($remaingitems + " More comments");
                    if ($remaingitems === 0 || $remaingitems <= 0) {
                        $('#Btn-More').hide();
                    }
                }
                else {
                    $('#Btn-More').hide();
                }
            },
            error: function () {
                alert("Error");
            }
        });
    });
});

// Load more comment function
function appendRecords(records) {
    var $el = $('.blog-container .comment-Container');
    var content = "";

    for (var i = 0; i < records.length; i++) {       
        var dateString = records[i].Date.substr(6);
        var currentTime = new Date(parseInt(dateString));
        var month = currentTime.getMonth() + 1;
        var day = currentTime.getDate();
        var year = currentTime.getFullYear();
        var hours = currentTime.getHours();
        var minutes = currentTime.getMinutes();
        var seconds = currentTime.getSeconds();
        var date = month + "/" + day + "/" + year + " " + hours + ":" + minutes + ":" + seconds;
        content += " <li class='comment-content'> <h5 class='comment-author-name'> By " + records[i].Author + " on  <span class='comment-time'>" + date + "</span></h5><span class='comment-body'>" + records[i].Body + "</span></li>";
    }
    $el.append(content);
}



