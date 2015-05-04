<?php
session_start();

include("connect.php");
include('Crypt/RSA.php');

$EncSendCounter = $_POST["SendCounter"];

$Recipient = $_POST["Recipient"];
$Message = $_POST["Message"];
$Sender = $_SESSION['username'];

if(!isset($_SESSION['LoggedIn'])) { 
	echo "Login first.";
	exit; 
} 	

if (!isset($EncSendCounter)) {
	echo "Counter missing.";
	exit;
}

$rsa = new Crypt_RSA();
$rsa->loadKey($_SESSION["ServerPriv"]);
$SendCounter = $rsa->decrypt(base64_decode($EncSendCounter));
if ($SendCounter <= $_SESSION["SendCounter"] || $SendCounter > $_SESSION["SendCounter"] + 10) {
	echo "Wrong counter";
	exit;
}

$_SESSION["SendCounter"] = $SendCounter;

$stmt = $conn->prepare("INSERT INTO Messages () VALUES (?, ?, ?);");
$stmt->bind_param("sss", $Message, $Sender, $Recipient);
$stmt->execute();
	
?>