<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Main.aspx.cs" Inherits="CommboxAssignment.Views.Main" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <title>Top 10 NBA Players</title>
    <script type="text/javascript" src="/scripts/jquery-3.5.1.min.js"></script>
    <script type="text/javascript" src="/scripts/jQueryUI-1.9.1.js"></script>
    <link rel="stylesheet" href="/scripts/jQueryUI-1.9.1.css">
    <script type="text/javascript" src="/scripts/main.js"></script>
    <link rel="stylesheet" href="/style/main.css?g"></head>
<body>
    <div class="spninner-container" style="display: none">
        <div class="lds-spinner">
            <div></div>
            <div></div>
            <div></div>
            <div></div>
            <div></div>
            <div></div>
            <div></div>
            <div></div>
            <div></div>
            <div></div>
            <div></div>
            <div></div>
        </div>
    </div>
    <div class="content">
        <div class="header">
            <span class="year-lbl">Year</span>
            <input class="year-input" type="text" id="datepicker" autocomplete="off" />
            <button class="apply" onclick="apply()">Apply</button>
        </div>
        <div id="error"></div>
        <div id="result">
        </div>

        <div class="card" id="clone" style="display: none">
            <div class="card-header">
                <div class="container-full-name">
                    <span class="full-name"></span>
                    <span class="player-height"></span>
                    <div class="card-position"></div>
                </div>
            </div>
            <div class="card-bottom">
                <div class="team-data">
                    <span class="team-name"></span>
                    <span class="conference-name"></span>
                    <span class="country"></span>
                </div>
                <div>
                    <span class="date-of-birth"></span>
                </div>
            </div>
        </div>
    </div>
</body>
</html>
