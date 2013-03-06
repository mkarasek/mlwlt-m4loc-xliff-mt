<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Log.aspx.cs" Inherits="mlwlt_web_test.Log" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8">
    <meta name="viewport" content="target-densitydpi=device-dpi, width=device-width, initial-scale=1.0, maximum-scale=1">
    <meta name="description" content="MLW-LT Testing Site (XLIFF/MT service)">
    <meta name="author" content="Milan Karásek, Moravia">
    <meta name="keywords" content="MLW-LT, Machine Translation, MT, XLIFF, Web service">
    <link href="css/modern.css" rel="stylesheet">
    <link href="css/modern-responsive.css" rel="stylesheet">
    <link href="css/site.css" rel="stylesheet" type="text/css">
    <link href="js/google-code-prettify/prettify.css" rel="stylesheet" type="text/css">

    <script type="text/javascript" src="js/assets/jquery-1.9.0.min.js"></script>
    <script type="text/javascript" src="js/assets/jquery.mousewheel.min.js"></script>

    <script type="text/javascript" src="js/modern/dropdown.js"></script>
    <script type="text/javascript" src="js/modern/accordion.js"></script>
    <script type="text/javascript" src="js/modern/buttonset.js"></script>
    <script type="text/javascript" src="js/modern/carousel.js"></script>
    <script type="text/javascript" src="js/modern/input-control.js"></script>
    <script type="text/javascript" src="js/modern/pagecontrol.js"></script>
    <script type="text/javascript" src="js/modern/rating.js"></script>
    <script type="text/javascript" src="js/modern/slider.js"></script>
    <script type="text/javascript" src="js/modern/tile-slider.js"></script>
    <script type="text/javascript" src="js/modern/tile-drag.js"></script>

    <title>MLW-LT Testing Site (XLIFF/MT service log)</title>
    <style>
        html {overflow-y: scroll;}
        h3, h4 {margin: 30px 0 10px 0;}
		div.eleContainer {color:#000000; font-family:Lucida Console, Courier New; font-size: 10pt;}
		div.eleBegin {color:#000000;display:inline;}
		span.eleName {color:#0000FF; display:inline;}
		span.eleAttrib {color:#FF0000; display:inline;}
		div.eleBody { margin-left: 20px;}
		div.eleBodyInLine { display: inline;}
		div.eleEnd {color:#000000;display:inline;}
		.tableHead th {background-color: #FEE78B; font-weight:bold; padding-left:10px;}
    </style>
</head>
<body>
<div class="page">

<script language="javascript">
    function displayLog(strLogName, o) 
    {
        $.ajax({
            type: "GET", url: "LogDetail.aspx?log=" + strLogName,
            cache: false, success: function (data) {
                document.getElementById("content").innerHTML = data;
                //$("#listToggleVisibleColumns").attr("style", "");
            }
        });
    }
</script>

<!--#include file="Header.htm"-->

    <div class="page secondary with-sidebar">
        <div class="page-header" style="padding:10px 0 10px 0;">
            <h1>MLW-LT Testing Site</h1>
            <h2>XLIFF/MT service -- Log</h2>
        </div>

                    

        <div class="page-sidebar">
            <ul>
                <asp:PlaceHolder id="phLogList"  runat="server" />
            </ul>
        </div>


        <div class="page-region">
            <div class="page-region-content">
                <div class="span9" id="content">
                    
                </div>
            </div>
        </div>


        <form id="form1" runat="server">
            <div>
            </div>
        </form>

<!--#include file="Footer.htm"-->

    </div>
</body>
</html>
