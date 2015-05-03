<?php
session_start();

include("connect.php");

$username = $_POST["username"];
$challenge = $_POST["challenge"];

if($challenge == $_SESSION["challenge"] && isset($_SESSION["challenge"]) && ($_SESSION["username"] == $username)) { 
    $_SESSION["LoggedIn"] = true; 
    echo "LoginGood"; 
} 
else { 
    echo "LoginBad"; 
} 
?>