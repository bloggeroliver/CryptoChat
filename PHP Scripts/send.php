<?php
session_start();

include("connect.php");

$Recipient = $_POST["Recipient"];
$Message = $_POST["Message"];
$Sender = $_SESSION['username'];

if(!isset($_SESSION['LoggedIn'])) { 
	echo "Login first.";
	exit; 
} 	

$stmt = $conn->prepare("INSERT INTO Messages () VALUES (?, ?, ?);");
$stmt->bind_param("sss", $Message, $Sender, $Recipient);
$stmt->execute();
	
?>