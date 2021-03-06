﻿@Code
    ViewData("Title") = "Home Page"
End Code

<script type="text/javascript">
    $(document).ready(function () {
        $('ul.tabs li').click(function () {
            var current = $(this);
            var contentBox = $('#' + current.attr('data-container'));
            $('ul.tabs li').removeClass('active');
            current.addClass('active');
            $('.content').hide();
            contentBox.show();
        });
    });	
</script>

<script type="text/javascript">
    $(function () {
        var matrixHub = $.connection.matrixHub;

        $("#loginButton").click(function () {
            $("#roster").empty();
            matrixHub.server.connect(
                $("#txtUsername").val(),
                $("#txtPassword").val(),
                $("#txtXmppDomain").val());
        });

        $("#logoutButton").click(function () {
            matrixHub.server.close();
        });

        // signalR callback for outgoing xml debug
        matrixHub.client.sendXml = function (message) {
            $("#log").append("<span class='log send'>" + message + "</span><br/>");
        };

        // signalR callback for incoming xml debug
        matrixHub.client.receiveXml = function (message) {
            $("#log").append("<span class='log recv'>" + message + "</span><br/>");
        };

        // signalR callbck for displaying matrix event (infos only)
        matrixHub.client.displayEvent = function (event) {
            $("#events").append("<span class='event'>" + event + "</span><br/>");
        };

        // signalR callback for contacts
        matrixHub.client.onRosterItem = function (uniqueid, jid, name) {
            var statusId = "status_" + uniqueid;
            var showId = "show_" + uniqueid;
            $("#roster").append(
                "<li id='" + uniqueid + "'>" +
                    "<span class='jid'>" + jid + "</span>" +
                    "<span class='name'>" + name + "</span>" +
                    "<span id='" + showId + "' class='show'></span>" +
                    "<span id='" + statusId + "' class='status'>Offline</span>" +
                "</li>");
        };

        // signalR callbck for incoming presence
        matrixHub.client.onPresence = function (uniqueid, jid, show, status) {
            var showId = "show_" + uniqueid;
            var statusId = "status_" + uniqueid;
            $("#" + showId).text(show);
            $("#" + statusId).text(status);
        };

        matrixHub.client.onMessage = function (msg) {
            $("#messages").append(
                    "<span class='from'>" + msg.From + ":</span>" +
                    "<span class='message'>" + msg.Body + ":</span>" +
                    "</br>"
            );
        };

        $.connection.hub.start().done(function () { });
    });

</script>

<div id="container">
    <div id="login">
        <h2>Login</h2>
        <label>Username
            <span class="small">your username</span>
        </label>
        <input id="txtUsername"/>
        
        <label>Password
            <span class="small">your password</span>
        </label>
        <input id="txtPassword" />
        
        <label>Domain
            <span class="small">your xmpp domain</span>
        </label>
        <input id="txtXmppDomain" />
      
        <button id="loginButton" type="button">Login</button>
        <button id="logoutButton" type="button">Logout</button>
        <br style="clear:left;"> 
    </div>

    <div id="contacts">
        <h2>Contact list</h2>
        <ul id="roster"/>
    </div>
</div>


    
<div id="tabcontainer">        
    <ul class="tabs">
        <li data-container="messages_container" class="active">Messages</li>  
        <li data-container="xml_log_container">Xml Log</li>  
        <li data-container="events_container">Events</li>  
    </ul>
    
    <span class="clear"></span>
    
    <div class="content" id="messages_container">
        <div id="messages"></div>
    </div>

    <div style="display:none;" class="content" id="xml_log_container">
	    <div id="log"></div>
    </div>
    
    <div style="display:none;" class="content" id="events_container">
	    <div id="events"></div>
    </div>
</div>