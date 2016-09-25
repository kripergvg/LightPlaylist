var $j = jQuery.noConflict();

$j(function () {
    var template =
        "<div class=\"lightplaylist_btn_sync\" data-id=\"%id\"  style=\"background-image: url(" + chrome.extension.getURL("sync2.png") + ");\"></div>";

    function scan() {
        $j(".audio_row")
            .each(function () {
                insertSyncBtn($j(this));
            });
    }

    function insertSyncBtn(jAudioRow) {
        if (jAudioRow.find(".lightplaylist_btn_sync").length === 0) {

            var id = jAudioRow.attr("data-full-id");
            var formatedTemplate = template.replace("%id", id);

            jAudioRow.prepend(formatedTemplate);
            adaSyncHandler(jAudioRow.find(".lightplaylist_btn_sync"));
        }
    }

    function adaSyncHandler(jBtn) {
        jBtn.click(
            function(e) {
                e.stopPropagation();

                var key = jBtn.attr("data-id");
                $j.ajax({
                    url: "//kriperplaylist.azurewebsites.net/api/songs",
                    data: { key: key },
                    method: "POST"
                });
            });
    }

    setInterval(scan, 1000);
});