<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewConfiguration.aspx.cs" Inherits="mlwlt_web_test.ViewConfiguration" %>

<!DOCTYPE html>

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

    <title>MLW-LT Testing Site (XLIFF/MT service)</title>
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
		span.defaultEngine {font-size: 9px; font-weight:bold; margin-left: 5px; padding: 1px 5px 1px 4px; position:relative; top:-1px; background-color:#FFCC00;-moz-border-radius: 3px;  -webkit-border-radius: 3px; -khtml-border-radius: 3px; border-radius: 3px;}
    </style>
</head>
<body class="modern-ui" onload="prettyPrint()">


<div class="page">

<!--#include file="Header.htm"-->

    <div class="page-region">
        <div class="page-region-content">

            <h1>MLW-LT Testing Site</h1>
            <h2>XLIFF/MT service -- Configuration</h2>

            <div style="margin:30px 0 40px 0;">
            <asp:PlaceHolder id="phOut" runat="server" />
            </div>

            <h2>Raw XML Configuration Data</h2>
            <p>
                This XML code is actually obtained from the web-service call of 'mlwlt_web_service_information' function:
            </p>

            <div style="margin:30px 0 40px 30px; padding: 20px; background-color:#F8F8F8; width:90%; border:solid 1px #606060;">
            <asp:PlaceHolder id="phOutXML" runat="server" />
            </div>

        </div>
    </div>

    <!--#include file="Footer.htm"-->

</div>

</body>
</html>
