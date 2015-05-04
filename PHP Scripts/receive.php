<?php

session_start();

include("connect.php");
include('Crypt/RSA.php');

$EncRecieveCounter = $_POST["RecieveCounter"];

if(!isset($_SESSION['username'])) 
   { 
   echo "Bitte erst login";
   exit; 
   } 	
	
if (!isset($EncRecieveCounter)) {
	echo "Counter missing.";
	exit;
}

$rsa = new Crypt_RSA();
$rsa->loadKey($_SESSION["ServerPriv"]);
$RecieveCounter = $rsa->decrypt(base64_decode($EncRecieveCounter));
if ($RecieveCounter <= $_SESSION["RecieveCounter"] || $RecieveCounter > $_SESSION["RecieveCounter"] + 10) {
	echo "Wrong counter";
	exit;
}

$_SESSION["RecieveCounter"] = $RecieveCounter;

$Recipient = $_SESSION['username'];

$stmt = $conn->prepare("SELECT Sender, Message FROM Messages WHERE Recipient = ?");
$stmt->bind_param("s", $Recipient);
$stmt->execute();
	
$stmt->bind_result($sender, $message);
while($row = $stmt->fetch())
   {
		echo "$sender<br />";
		echo "$message<br />";
		$stmt->bind_result($sender, $message);
   }

$stmt = $conn->prepare("DELETE FROM Messages WHERE Recipient = ?");
$stmt->bind_param("s", $Recipient);
$stmt->execute();
?>